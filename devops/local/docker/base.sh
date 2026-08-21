#!/usr/bin/env bash
set -euo pipefail

RESET=0
BASE_PORT_WAIT_SECONDS="${BASE_PORT_WAIT_SECONDS:-5}"
BASE_RESET_PORT_WAIT_SECONDS="${BASE_RESET_PORT_WAIT_SECONDS:-30}"
BASE_RESET_PROXY_WAIT_SECONDS="${BASE_RESET_PROXY_WAIT_SECONDS:-10}"

usage() {
  cat <<'USAGE'
Usage: ./devops/local/docker/base.sh [--reset|--from-scratch]

Starts the local Querify base-services stack.

Options:
  --reset, --from-scratch  Remove known base-service containers and volumes before starting.
  -h, --help              Show this help.

The reset option deletes local infrastructure data, including PostgreSQL, Redis,
RabbitMQ, MinIO, SMTP4Dev, Prometheus, and Grafana volumes.
USAGE
}

while (($#)); do
  case "$1" in
    --reset | --from-scratch)
      RESET=1
      ;;
    -h | --help)
      usage
      exit 0
      ;;
    *)
      printf "Unknown option: %s\n\n" "$1" >&2
      usage >&2
      exit 2
      ;;
  esac
  shift
done

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/docker-compose.baseservices.yml"
COMPOSE_PROJECT="qf_baseservices"
COMPOSE_ARGS=(-p "$COMPOSE_PROJECT" -f "$COMPOSE_FILE")

BASE_CONTAINERS=(
  postgres
  smtp
  rabbitmq
  minio
  minio-init
  rabbitmq-exporter
  alertmanager
  prometheus
  grafana
  redis
  jaeger
)

BASE_VOLUMES=(
  qf_baseservices_rabbitmq
  qf_baseservices_cache
  qf_baseservices_smtp4dev-data
  qf_baseservices_postgres
  qf_baseservices_minio
  qf_baseservices_prometheus-data
  qf_baseservices_grafana-data
)

REQUIRED_PORTS=(
  "5432:PostgreSQL"
  "4590:SMTP4Dev UI"
  "1025:SMTP4Dev SMTP"
  "15672:RabbitMQ UI"
  "5672:RabbitMQ AMQP"
  "9000:MinIO API"
  "5900:MinIO API alias"
  "5901:MinIO Console"
  "9419:RabbitMQ exporter"
  "9093:Alertmanager"
  "9090:Prometheus"
  "3000:Grafana"
  "6379:Redis"
  "16686:Jaeger UI"
  "4317:Jaeger OTLP gRPC"
  "4318:Jaeger OTLP HTTP"
)

print_banner() {
  echo ""
  printf "\e[32m%s\e[0m\n" "======================================================================="
  printf "\e[32m%s\e[0m\n" "$1"
  printf "\e[32m%s\e[0m\n" "======================================================================="
  echo ""
}

command_exists() {
  command -v "$1" >/dev/null 2>&1
}

systemd_unit_exists() {
  local unit="$1"
  systemctl list-unit-files "$unit" --no-legend 2>/dev/null | grep -q "^$unit"
}

restart_docker_if_systemd_available() {
  if ! command_exists systemctl || ! systemd_unit_exists docker.service; then
    return 0
  fi

  print_banner "Restarting Docker services..."

  if systemd_unit_exists docker.socket; then
    sudo systemctl stop docker.socket
  fi

  sudo systemctl stop docker.service

  if systemd_unit_exists containerd.service; then
    sudo systemctl restart containerd
  fi

  sudo systemctl start docker.service
}

remove_known_base_containers() {
  local attempt
  local remaining=()

  print_banner "Removing known Querify base-service containers..."

  for attempt in {1..10}; do
    mapfile -t remaining < <(find_existing_known_containers)

    if ((${#remaining[@]} == 0)); then
      return 0
    fi

    docker rm -f "${remaining[@]}" >/dev/null 2>&1 || true
    sleep 1
  done

  mapfile -t remaining < <(find_existing_known_containers)

  if ((${#remaining[@]} > 0)); then
    printf "Could not remove these known base-service containers:\n\n" >&2
    printf "  - %s\n" "${remaining[@]}" >&2
    exit 1
  fi
}

remove_known_base_volumes() {
  print_banner "Removing known Querify base-service volumes..."
  docker volume rm -f "${BASE_VOLUMES[@]}" >/dev/null 2>&1 || true
}

check_known_container_names() {
  local conflicts=()

  mapfile -t conflicts < <(find_existing_known_containers)

  if ((${#conflicts[@]} == 0)); then
    return 0
  fi

  print_banner "Container name conflicts detected"

  printf "These container names are already present after stopping the Compose project:\n\n" >&2

  for container in "${conflicts[@]}"; do
    printf "  - %s\n" "$container" >&2
  done

  cat >&2 <<EOF

Run the reset mode to remove stale Querify base-service containers before starting:

  $0 --reset

EOF
  exit 1
}

find_existing_known_containers() {
  local existing_names
  local container

  existing_names="$(docker ps -a --format '{{.Names}}' 2>/dev/null || true)"

  for container in "${BASE_CONTAINERS[@]}"; do
    if grep -Fxq "$container" <<<"$existing_names"; then
      printf "%s\n" "$container"
    fi
  done
}

port_is_listening() {
  local port="$1"

  if command_exists lsof && lsof -nP -iTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1; then
    return 0
  fi

  if command_exists ss && ss -H -ltn "( sport = :$port )" 2>/dev/null | grep -q .; then
    return 0
  fi

  if command_exists nc && nc -z 127.0.0.1 "$port" >/dev/null 2>&1; then
    return 0
  fi

  if command_exists timeout && timeout 1 bash -c ":</dev/tcp/127.0.0.1/$port" >/dev/null 2>&1; then
    return 0
  fi

  return 1
}

describe_port_owner() {
  local port="$1"
  local owner=""

  if command_exists docker; then
    owner="$(docker ps --format '{{.Names}}\t{{.Ports}}' 2>/dev/null | awk -v needle=":${port}->" 'index($0, needle) { print $1 " (" $0 ")"; exit }' || true)"
    if [[ -n "$owner" ]]; then
      printf "Docker container %s" "$owner"
      return 0
    fi
  fi

  if command_exists sudo && command_exists lsof; then
    owner="$(sudo -n lsof -nP -iTCP:"$port" -sTCP:LISTEN 2>/dev/null | awk 'NR == 2 { print $1 " pid " $2; exit }' || true)"
    if [[ -n "$owner" ]]; then
      printf "%s" "$owner"
      return 0
    fi
  fi

  if command_exists lsof; then
    owner="$(lsof -nP -iTCP:"$port" -sTCP:LISTEN 2>/dev/null | awk 'NR == 2 { print $1 " pid " $2; exit }' || true)"
    if [[ -n "$owner" ]]; then
      printf "%s" "$owner"
      return 0
    fi
  fi

  if command_exists sudo && command_exists ss; then
    owner="$(sudo -n ss -H -ltnp "( sport = :$port )" 2>/dev/null | head -n 1 || true)"
    if [[ -n "$owner" ]]; then
      printf "%s" "$owner"
      return 0
    fi
  fi

  if command_exists ss; then
    owner="$(ss -H -ltnp "( sport = :$port )" 2>/dev/null | head -n 1 || true)"
    if [[ -n "$owner" ]]; then
      printf "%s" "$owner"
      return 0
    fi
  fi

  printf "owner not detected"
}

list_listening_pids_for_port() {
  local port="$1"

  {
    if command_exists sudo && command_exists lsof; then
      sudo -n lsof -nP -t -iTCP:"$port" -sTCP:LISTEN 2>/dev/null || true
    fi

    if command_exists lsof; then
      lsof -nP -t -iTCP:"$port" -sTCP:LISTEN 2>/dev/null || true
    fi

    if command_exists sudo && command_exists ss; then
      sudo -n ss -H -ltnp "( sport = :$port )" 2>/dev/null | sed -nE 's/.*pid=([0-9]+).*/\1/p' || true
    fi

    if command_exists ss; then
      ss -H -ltnp "( sport = :$port )" 2>/dev/null | sed -nE 's/.*pid=([0-9]+).*/\1/p' || true
    fi

    if command_exists sudo && command_exists fuser; then
      sudo -n fuser "${port}/tcp" 2>/dev/null | tr ' ' '\n' || true
    fi
  } | awk '/^[0-9]+$/ { print }' | sort -u
}

is_docker_port_proxy_pid() {
  local pid="$1"
  local comm
  local args

  comm="$(ps -p "$pid" -o comm= 2>/dev/null | awk '{$1=$1; print}' || true)"
  args="$(ps -p "$pid" -o args= 2>/dev/null || true)"

  case "$comm" in
    docker-proxy | com.docker.proxy)
      return 0
      ;;
  esac

  case "$args" in
    *docker-proxy* | *com.docker.proxy*)
      return 0
      ;;
  esac

  return 1
}

process_exists() {
  local pid="$1"

  sudo -n kill -0 "$pid" 2>/dev/null || kill -0 "$pid" 2>/dev/null
}

remove_stale_docker_port_proxies() {
  local proxy_pids=()
  local pids=()
  local entry
  local port
  local pid
  local remaining=()

  for entry in "${REQUIRED_PORTS[@]}"; do
    port="${entry%%:*}"
    mapfile -t pids < <(list_listening_pids_for_port "$port")

    for pid in "${pids[@]}"; do
      if is_docker_port_proxy_pid "$pid"; then
        proxy_pids+=("$pid")
      fi
    done
  done

  if ((${#proxy_pids[@]} == 0)); then
    return 1
  fi

  mapfile -t proxy_pids < <(printf "%s\n" "${proxy_pids[@]}" | sort -u)

  print_banner "Removing stale Docker port proxy processes..."
  printf "Stopping Docker proxy process ids: %s\n" "${proxy_pids[*]}"

  sudo -n kill -TERM "${proxy_pids[@]}" 2>/dev/null || kill -TERM "${proxy_pids[@]}" 2>/dev/null || true
  sleep 2

  for pid in "${proxy_pids[@]}"; do
    if process_exists "$pid" && is_docker_port_proxy_pid "$pid"; then
      remaining+=("$pid")
    fi
  done

  if ((${#remaining[@]} > 0)); then
    sudo -n kill -KILL "${remaining[@]}" 2>/dev/null || kill -KILL "${remaining[@]}" 2>/dev/null || true
    sleep 1
  fi

  return 0
}

release_required_ports_after_reset() {
  if wait_for_required_ports "$BASE_RESET_PORT_WAIT_SECONDS"; then
    return 0
  fi

  remove_stale_docker_port_proxies || true
  wait_for_required_ports "$BASE_RESET_PROXY_WAIT_SECONDS" || true
}

collect_port_conflicts() {
  local conflicts=()
  local entry
  local port
  local label
  local owner

  for entry in "${REQUIRED_PORTS[@]}"; do
    port="${entry%%:*}"
    label="${entry#*:}"

    if port_is_listening "$port"; then
      owner="$(describe_port_owner "$port")"
      conflicts+=("$port|$label|$owner")
    fi
  done

  if ((${#conflicts[@]} > 0)); then
    printf "%s\n" "${conflicts[@]}"
  fi
}

wait_for_required_ports() {
  local wait_seconds="$1"
  local waited=0
  local conflicts=()

  while true; do
    mapfile -t conflicts < <(collect_port_conflicts)

    if ((${#conflicts[@]} == 0)); then
      return 0
    fi

    if ((waited >= wait_seconds)); then
      return 1
    fi

    if ((waited == 0)); then
      print_banner "Waiting for base-service ports to be released..."
    fi

    sleep 1
    waited=$((waited + 1))
  done
}

check_required_ports() {
  local conflicts=()
  local entry
  local port
  local label
  local owner

  mapfile -t conflicts < <(collect_port_conflicts)

  if ((${#conflicts[@]} == 0)); then
    return 0
  fi

  print_banner "Port conflicts detected"

  printf "The base-services stack needs these host ports, but they are already in use:\n\n" >&2

  for entry in "${conflicts[@]}"; do
    IFS='|' read -r port label owner <<<"$entry"
    printf "  - %s (%s): %s\n" "$port" "$label" "$owner" >&2
  done

  cat >&2 <<EOF

Stop the process or container using the conflicting port and run this script again.
EOF

  if [[ "$RESET" == "1" ]]; then
    cat >&2 <<EOF
The reset mode already removed known Querify base-service containers. If the owner
is still docker-proxy, restart Docker and run this script again:

  $0 --reset

EOF
  else
    cat >&2 <<EOF
If the owner is a stale Querify base-service container, run:

  $0 --reset

EOF
  fi

  exit 1
}

export REDIS_PASSWORD="${REDIS_PASSWORD:-RedisTempPassword}"

restart_docker_if_systemd_available

if [[ "$RESET" == "1" ]]; then
  print_banner "Resetting Querify base services..."
  docker compose "${COMPOSE_ARGS[@]}" down --remove-orphans --volumes
  remove_known_base_containers
  remove_known_base_volumes
  restart_docker_if_systemd_available
else
  print_banner "Stopping Querify base services (project only)..."
  docker compose "${COMPOSE_ARGS[@]}" down --remove-orphans
  check_known_container_names
fi

print_banner "Starting base services..."

docker network inspect qf-network >/dev/null 2>&1 || docker network create qf-network

if [[ "$RESET" == "1" ]]; then
  release_required_ports_after_reset
else
  wait_for_required_ports "$BASE_PORT_WAIT_SECONDS" || true
fi

check_required_ports

docker compose "${COMPOSE_ARGS[@]}" up -d --force-recreate --no-build --remove-orphans --wait

username="postgres"
password="Pass123$"
command="PGPASSWORD=$password psql -U $username -d postgres -f /docker-entrypoint-initdb.d/create_databases.sql"

docker exec -i postgres sh -c "$command"
