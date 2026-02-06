#!/usr/bin/env bash
set -e

export REDIS_PASSWORD=RedisTempPassword

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

username="postgres"
password="Pass123$"

command="PGPASSWORD=$password psql -U $username -d postgres -f /docker-entrypoint-initdb.d/create_databases.sql"

docker exec -i postgres sh -c "$command"
