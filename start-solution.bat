@echo off

docker start etherealdb
docker start storage-bucket
docker start storage-controller
docker start ethereal-storage

PAUSE