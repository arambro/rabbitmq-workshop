Write-Host "Stoping containers"
docker stop $(docker ps -a -q)
Write-Host "Removing containers"
docker rm $(docker ps -a -q)
Write-Host "Cleaning volumes"
docker volume prune -f