echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Removing Docker Containers..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker compose -p bf_services -f ./docker/docker-compose.yml down --remove-orphans

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "BaseFaq Docker Images..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker images --format '{{.Repository}} {{.ID}}' | awk '$1 ~ /^basefaq/ {print $2}' | xargs -r docker rmi -f

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Starting Docker Containers..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker compose -p bf_services -f ./docker/docker-compose.yml up -d --build

echo ""
printf "\e[32m%s\e[0m\n" "Started services: basefaq.faq.portal.app, basefaq.faq.public.app, basefaq.tenant.portal.app"

echo ""
printf "\e[32m%s\e[0m\n" "======================================================================="
printf "\e[32m%s\e[0m\n" "Cleaning Docker..."
printf "\e[32m%s\e[0m\n" "======================================================================="
echo ""

docker image prune -f
