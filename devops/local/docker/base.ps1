$ErrorActionPreference = 'Stop'

$Reset = $false
$BasePortWaitSeconds = 5
$BaseResetPortWaitSeconds = 30
$BaseResetProxyWaitSeconds = 10

if ($env:BASE_PORT_WAIT_SECONDS) {
  $BasePortWaitSeconds = [int]$env:BASE_PORT_WAIT_SECONDS
}

if ($env:BASE_RESET_PORT_WAIT_SECONDS) {
  $BaseResetPortWaitSeconds = [int]$env:BASE_RESET_PORT_WAIT_SECONDS
}

if ($env:BASE_RESET_PROXY_WAIT_SECONDS) {
  $BaseResetProxyWaitSeconds = [int]$env:BASE_RESET_PROXY_WAIT_SECONDS
}

function Show-Usage {
  Write-Host "Usage: .\devops\local\docker\base.ps1 [-Reset|-FromScratch|--reset|--from-scratch]"
  Write-Host ""
  Write-Host "Starts the local Querify base-services stack."
  Write-Host ""
  Write-Host "Options:"
  Write-Host "  -Reset, --reset              Remove known base-service containers and volumes before starting."
  Write-Host "  -FromScratch, --from-scratch Same as -Reset."
  Write-Host "  -Help, --help                Show this help."
  Write-Host ""
  Write-Host "The reset option deletes local infrastructure data, including PostgreSQL, Redis,"
  Write-Host "RabbitMQ, MinIO, SMTP4Dev, Prometheus, and Grafana volumes."
}

foreach ($arg in $args) {
  switch ($arg) {
    '-Reset' { $Reset = $true }
    '--reset' { $Reset = $true }
    '-FromScratch' { $Reset = $true }
    '--from-scratch' { $Reset = $true }
    '-Help' {
      Show-Usage
      exit 0
    }
    '--help' {
      Show-Usage
      exit 0
    }
    '-h' {
      Show-Usage
      exit 0
    }
    default {
      Write-Host "Unknown option: $arg" -ForegroundColor Red
      Show-Usage
      exit 2
    }
  }
}

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ComposeFile = Join-Path $ScriptDir 'docker-compose.baseservices.yml'
$ComposeProject = 'qf_baseservices'

$BaseContainers = @(
  'postgres',
  'smtp',
  'rabbitmq',
  'minio',
  'minio-init',
  'rabbitmq-exporter',
  'alertmanager',
  'prometheus',
  'grafana',
  'redis',
  'jaeger'
)

$BaseVolumes = @(
  'qf_baseservices_rabbitmq',
  'qf_baseservices_cache',
  'qf_baseservices_smtp4dev-data',
  'qf_baseservices_postgres',
  'qf_baseservices_minio',
  'qf_baseservices_prometheus-data',
  'qf_baseservices_grafana-data'
)

$RequiredPorts = @(
  @{ Port = 5432; Label = 'PostgreSQL' },
  @{ Port = 4590; Label = 'SMTP4Dev UI' },
  @{ Port = 1025; Label = 'SMTP4Dev SMTP' },
  @{ Port = 15672; Label = 'RabbitMQ UI' },
  @{ Port = 5672; Label = 'RabbitMQ AMQP' },
  @{ Port = 9000; Label = 'MinIO API' },
  @{ Port = 5900; Label = 'MinIO API alias' },
  @{ Port = 5901; Label = 'MinIO Console' },
  @{ Port = 9419; Label = 'RabbitMQ exporter' },
  @{ Port = 9093; Label = 'Alertmanager' },
  @{ Port = 9090; Label = 'Prometheus' },
  @{ Port = 3000; Label = 'Grafana' },
  @{ Port = 6379; Label = 'Redis' },
  @{ Port = 16686; Label = 'Jaeger UI' },
  @{ Port = 4317; Label = 'Jaeger OTLP gRPC' },
  @{ Port = 4318; Label = 'Jaeger OTLP HTTP' }
)

function Write-Banner {
  param([string]$Message)

  Write-Host ""
  Write-Host "=======================================================================" -ForegroundColor Green
  Write-Host $Message -ForegroundColor Green
  Write-Host "=======================================================================" -ForegroundColor Green
  Write-Host ""
}

function Get-ExistingKnownBaseContainers {
  $existingNames = @(docker ps -a --format '{{.Names}}' 2>$null)
  @($BaseContainers | Where-Object { $existingNames -contains $_ })
}

function Remove-KnownBaseContainers {
  $remaining = @()

  Write-Banner "Removing known Querify base-service containers..."

  foreach ($attempt in 1..10) {
    $remaining = @(Get-ExistingKnownBaseContainers)

    if (-not $remaining) {
      return
    }

    docker rm -f $remaining 2>$null | Out-Null
    Start-Sleep -Seconds 1
  }

  $remaining = @(Get-ExistingKnownBaseContainers)

  if ($remaining) {
    Write-Host "Could not remove these known base-service containers:" -ForegroundColor Red
    Write-Host ""

    foreach ($container in $remaining) {
      Write-Host "  - $container"
    }

    exit 1
  }
}

function Remove-KnownBaseVolumes {
  Write-Banner "Removing known Querify base-service volumes..."
  docker volume rm -f $BaseVolumes 2>$null | Out-Null
}

function Confirm-KnownContainerNamesAvailable {
  $conflicts = @(Get-ExistingKnownBaseContainers)

  if (-not $conflicts) {
    return
  }

  Write-Banner "Container name conflicts detected"
  Write-Host "These container names are already present after stopping the Compose project:" -ForegroundColor Red
  Write-Host ""

  foreach ($container in $conflicts) {
    Write-Host "  - $container"
  }

  Write-Host ""
  Write-Host "Run the reset mode to remove stale Querify base-service containers before starting:"
  Write-Host ""
  Write-Host "  .\devops\local\docker\base.ps1 -Reset"
  Write-Host ""

  exit 1
}

function Test-PortInUse {
  param([int]$Port)

  try {
    $connection = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
      Select-Object -First 1

    if ($connection) {
      return $true
    }
  }
  catch {
  }

  $client = $null

  try {
    $client = [System.Net.Sockets.TcpClient]::new()
    $connect = $client.BeginConnect('127.0.0.1', $Port, $null, $null)

    if (-not $connect.AsyncWaitHandle.WaitOne(250, $false)) {
      return $false
    }

    $client.EndConnect($connect)
    return $true
  }
  catch {
    return $false
  }
  finally {
    if ($client) {
      $client.Dispose()
    }
  }
}

function Get-PortOwnerDescription {
  param([int]$Port)

  $dockerOwner = docker ps --format '{{.Names}}|{{.Ports}}' 2>$null |
    Where-Object { $_ -like "*:$Port->*" } |
    Select-Object -First 1

  if ($dockerOwner) {
    $parts = $dockerOwner -split '\|', 2
    return "Docker container $($parts[0]) ($($parts[1]))"
  }

  try {
    $connection = Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
      Select-Object -First 1

    if ($connection -and $connection.OwningProcess) {
      $process = Get-Process -Id $connection.OwningProcess -ErrorAction SilentlyContinue

      if ($process) {
        return "$($process.ProcessName) pid $($connection.OwningProcess)"
      }

      return "pid $($connection.OwningProcess)"
    }
  }
  catch {
  }

  return 'owner not detected'
}

function Get-ListeningPidsForPort {
  param([int]$Port)

  $pids = @()

  try {
    $pids += Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue |
      Where-Object { $_.OwningProcess } |
      ForEach-Object { $_.OwningProcess }
  }
  catch {
  }

  @($pids | Where-Object { $_ } | Sort-Object -Unique)
}

function Test-DockerPortProxyProcess {
  param([int]$ProcessId)

  $process = Get-Process -Id $ProcessId -ErrorAction SilentlyContinue

  if (-not $process) {
    return $false
  }

  if ($process.ProcessName -in @('docker-proxy', 'com.docker.proxy')) {
    return $true
  }

  try {
    $cimProcess = Get-CimInstance Win32_Process -Filter "ProcessId = $ProcessId" -ErrorAction SilentlyContinue

    if ($cimProcess -and $cimProcess.CommandLine -match 'docker-proxy|com\.docker\.proxy') {
      return $true
    }
  }
  catch {
  }

  return $false
}

function Remove-StaleDockerPortProxies {
  $proxyPids = @()

  foreach ($requiredPort in $RequiredPorts) {
    $pids = @(Get-ListeningPidsForPort -Port $requiredPort.Port)

    foreach ($processId in $pids) {
      if (Test-DockerPortProxyProcess -ProcessId $processId) {
        $proxyPids += $processId
      }
    }
  }

  $proxyPids = @($proxyPids | Sort-Object -Unique)

  if (-not $proxyPids) {
    return $false
  }

  Write-Banner "Removing stale Docker port proxy processes..."
  Write-Host "Stopping Docker proxy process ids: $($proxyPids -join ', ')"

  foreach ($processId in $proxyPids) {
    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
  }

  Start-Sleep -Seconds 1
  return $true
}

function Release-RequiredPortsAfterReset {
  if (Wait-RequiredPortsAvailable -WaitSeconds $BaseResetPortWaitSeconds) {
    return
  }

  Remove-StaleDockerPortProxies | Out-Null
  Wait-RequiredPortsAvailable -WaitSeconds $BaseResetProxyWaitSeconds | Out-Null
}

function Get-PortConflicts {
  $conflicts = @()

  foreach ($requiredPort in $RequiredPorts) {
    if (Test-PortInUse -Port $requiredPort.Port) {
      $conflicts += [PSCustomObject]@{
        Port = $requiredPort.Port
        Label = $requiredPort.Label
        Owner = Get-PortOwnerDescription -Port $requiredPort.Port
      }
    }
  }

  $conflicts
}

function Wait-RequiredPortsAvailable {
  param([int]$WaitSeconds)

  $waited = 0

  while ($true) {
    $conflicts = @(Get-PortConflicts)

    if (-not $conflicts) {
      return $true
    }

    if ($waited -ge $WaitSeconds) {
      return $false
    }

    if ($waited -eq 0) {
      Write-Banner "Waiting for base-service ports to be released..."
    }

    Start-Sleep -Seconds 1
    $waited += 1
  }
}

function Confirm-RequiredPortsAvailable {
  $conflicts = @(Get-PortConflicts)

  if (-not $conflicts) {
    return
  }

  Write-Banner "Port conflicts detected"
  Write-Host "The base-services stack needs host ports that are already in use." -ForegroundColor Red
  Write-Host ""

  foreach ($conflict in $conflicts) {
    Write-Host ("  - {0} ({1}): {2}" -f $conflict.Port, $conflict.Label, $conflict.Owner)
  }

  Write-Host ""
  Write-Host "Stop the process or container using the conflicting port and run this script again."
  Write-Host ""

  if ($Reset) {
    Write-Host "The reset mode already removed known Querify base-service containers. If the owner"
    Write-Host "is still docker-proxy, restart Docker and run this script again:"
    Write-Host ""
    Write-Host "  .\devops\local\docker\base.ps1 -Reset"
  }
  else {
    Write-Host "If the owner is a stale Querify base-service container, run:"
    Write-Host ""
    Write-Host "  .\devops\local\docker\base.ps1 -Reset"
  }

  Write-Host ""

  exit 1
}

if (-not $env:REDIS_PASSWORD) {
  $env:REDIS_PASSWORD = 'RedisTempPassword'
}

if ($Reset) {
  Write-Banner "Resetting Querify base services..."
  docker compose -p $ComposeProject -f $ComposeFile down --remove-orphans --volumes
  Remove-KnownBaseContainers
  Remove-KnownBaseVolumes
}
else {
  Write-Banner "Stopping Querify base services (project only)..."
  docker compose -p $ComposeProject -f $ComposeFile down --remove-orphans
  Confirm-KnownContainerNamesAvailable
}

Write-Banner "Starting base services..."

$networkExists = docker network inspect qf-network 2>$null
if (-not $networkExists) {
  docker network create qf-network
}

if ($Reset) {
  Release-RequiredPortsAfterReset
}
else {
  Wait-RequiredPortsAvailable -WaitSeconds $BasePortWaitSeconds | Out-Null
}

Confirm-RequiredPortsAvailable

docker compose -p $ComposeProject -f $ComposeFile up -d --force-recreate --no-build --remove-orphans --wait

$username = 'postgres'
$password = 'Pass123$'
$command = "PGPASSWORD=$password psql -U $username -d postgres -f /docker-entrypoint-initdb.d/create_databases.sql"

docker exec -i postgres sh -c $command
