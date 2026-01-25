echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Removing Docker Containers..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker compose -f ./docker/docker-compose.yml down

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "BaseFaq Docker Images..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker images | grep '^basefaq' | awk '{print $3}' | xargs docker rmi -f

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Starting Docker Containers..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

  docker compose -f ./docker/docker-compose.yml pull
  docker compose -f ./docker/docker-compose.yml up -d

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Cleaning Docker..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker image prune -af
