@echo off

set "base_dir=%CD%"

docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=EtherealDatabaseStorage!!" -p 1433:1433 --name etherealdb --hostname etherealdb -d mcr.microsoft.com/mssql/server:2022-latest
docker start etherealdb

for /f %%i in ('docker inspect -f {{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}} etherealdb') do set db_ip=%%i

set buck_id=1

cd ./Bucket
docker volume create bucket-volume-%buck_id%
docker image rm storage-bucket
docker build -t storage-bucket .
docker stop storage-bucket
docker rm storage-bucket
docker run -p 8070:8070 -e "DB_IP=%db_ip%" -e "DB_PASS=EtherealDatabaseStorage!!" -e "BUCK_ID=%buck_id%" --name storage-bucket --hostname storage-bucket -v bucket-volume-%buck_id%:/var/data -d storage-bucket

for /f %%i in ('docker inspect -f {{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}} storage-bucket') do set buck_ip=%%i

cd %base_dir%

cd ./StorageController
docker image rm storage-controller
docker build -t storage-controller .
docker stop storage-controller
docker rm storage-controller
docker run -e "DB_IP=%db_ip%" -e "DB_PASS=EtherealDatabaseStorage!!" -e "BUCK_IP=%buck_ip%" -e "BUCK_PORT=8070" -p 8090:8090 --name storage-controller --hostname storage-controller -d storage-controller

for /f %%i in ('docker inspect -f {{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}} storage-controller') do set sc_ip=%%i

cd %base_dir%

cd ./"Ethereal Cloud"/certs

docker volume create certificate
docker run --rm -v certificate:/certs -v "%CD%":/local busybox cp /local/cert.pfx /certs/

cd %base_dir%

cd ./"Ethereal Cloud"
docker image rm ethereal-storage
docker build -t ethereal-storage .
docker stop ethereal-storage
docker rm ethereal-storage
docker run -e "SC_IP=%sc_ip%" -p 8080:8080 -p 8081:8081 -v certificate:/home/app/certs/ --name ethereal-storage --hostname ethereal-storage -d ethereal-storage

cd %base_dir%

PAUSE