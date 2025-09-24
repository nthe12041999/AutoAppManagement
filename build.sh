git pull origin

docker build -t autoappmanagement-api:lastest -f api.Dockerfile .

docker build -t autoappmanagement-webapp:lastest -f webapp.Dockerfile .

COMPOSE_FILE="docker-compose-build.yml"

docker-compose -f %COMPOSE_FILE% down --remove-orphans

docker-compose -f $COMPOSE_FILE up -d