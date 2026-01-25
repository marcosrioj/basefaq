#!/usr/bin/env bash
set -e

# Step 1: Stop all running containers
if [ -n "$(docker ps -q)" ]; then
  docker stop $(docker ps -q)
fi

# Step 2: Remove all containers (running or stopped)
if [ -n "$(docker ps -aq)" ]; then
  docker rm -f $(docker ps -aq)
fi

docker builder prune -f
docker system prune -f


# Step 3: Start core services with docker compose
docker compose -p bf_baseservices -f ./docker/docker-compose.baseservices.yml up -d --force-recreate --no-build --remove-orphans --wait

username="sa"
password="Pass123$"

command="/opt/mssql-tools/bin/sqlcmd -S localhost -U $username -P $password -i /bak/check_databases.sql"

docker exec -i mssql bash -c "$command"
