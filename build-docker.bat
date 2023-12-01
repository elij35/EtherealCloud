@echo off

for /f %%i in ('cd') do set base_dir=%%i

docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=EtherealDatabaseStorage!!" -p 1433:1433 --name etherealdb --hostname etherealdb -d mcr.microsoft.com/mssql/server:2022-latest
docker start etherealdb

cd ./StorageController
docker image rm storage-controller
docker build -t storage-controller .
docker stop storage-controller
docker rm storage-controller
docker run -p 8090:8090 --name storage-controller --network host --hostname storage-controller -d storage-controller

cd %base_dir%

cd ./"Ethereal Cloud"
docker image rm ethereal-storage
docker build -t ethereal-storage .
docker stop ethereal-storage
docker rm ethereal-storage
docker run -p 8080:8080 --name ethereal-storage --hostname ethereal-storage -d ethereal-storage

cd %base_dir%

PAUSE