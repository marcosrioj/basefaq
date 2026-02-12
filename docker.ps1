$ErrorActionPreference = 'Stop'

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host "Removing Docker Containers..." -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host ""

docker compose -p bf_services -f ./docker/docker-compose.yml down --remove-orphans

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host "BaseFaq Docker Images..." -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host ""

$images = docker images --format '{{.Repository}} {{.ID}}' |
  Where-Object { $_ -match '^basefaq\s+' } |
  ForEach-Object { $_.Split(' ', [System.StringSplitOptions]::RemoveEmptyEntries)[1] }

if ($images) {
  docker rmi -f $images
}

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host "Starting Docker Containers..." -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host ""

docker compose -p bf_services -f ./docker/docker-compose.yml up -d --build

Write-Host ""
Write-Host "Started services: basefaq.faq.portal.app, basefaq.faq.public.app, basefaq.tenant.backoffice.app, basefaq.tenant.portal.app" -ForegroundColor Green

Write-Host ""
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host "Cleaning Docker..." -ForegroundColor Green
Write-Host "=======================================================================" -ForegroundColor Green
Write-Host ""

docker image prune -f
