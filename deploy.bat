docker build -t upside-api-image .

docker tag upside-api-image registry.heroku.com/upside-api/web

docker push registry.heroku.com/upside-api/web

heroku container:release web -a upside-api