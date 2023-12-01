@echo off

docker start etherealdb
docker start storage-controller
docker start ethereal-storage

PAUSE