docker build -t autoappmanagement-api:lastest -f api.Dockerfile .

docker build -t autoappmanagement-webapp:lastest -f webapp.Dockerfile .

docker run -d -p 8081:8080 --name autoapp-api autoappmanagement-api:lastest

docker run -d -p 8080:8080 --name autoapp-webapp autoappmanagement-webapp:lastest