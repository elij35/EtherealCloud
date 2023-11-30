@echo on

for /f %%i in ('cd') do set base_dir=%%i

docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=EtherealDatabaseStorage!!" -p 1433:1433 --name etherealdb --hostname etherealdb -d mcr.microsoft.com/mssql/server:2022-latest

cd ./StorageController
docker build -t storage-controller .
docker run -p 8090:8090 --name storage-controller --hostname storage-controller -d storage-controller

cd %base_dir%

cd ./"Ethereal Cloud"
docker build -t ethereal-storage .
docker run -p 8080:8080 --name ethereal-storage --hostname ethereal-storage -d ethereal-storage

cd %base_dir%

PAUSE