# Script para ejecutar todos los microservicios de Airbnb .NET
# Cada microservicio se ejecuta en una terminal separada

Write-Host "Iniciando microservicios de Airbnb .NET..." -ForegroundColor Green

# Obtener la ruta base del proyecto
$baseDir = Split-Path -Parent $PSScriptRoot

# Microservicio Usuarios - Puerto 5001
Write-Host "Iniciando Usuarios.API en puerto 5001..." -ForegroundColor Cyan
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$baseDir\src\Usuarios.API'; dotnet run --launch-profile http"

# Esperar 3 segundos antes de iniciar el siguiente
Start-Sleep -Seconds 3

# Microservicio Airbnbs - Puerto 5002
Write-Host "Iniciando Airbnbs.API en puerto 5002..." -ForegroundColor Cyan
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$baseDir\src\Airbnbs.API'; dotnet run --launch-profile http"

# Esperar 3 segundos antes de iniciar el siguiente
Start-Sleep -Seconds 3

# Microservicio Reservas - Puerto 5003
Write-Host "Iniciando Reservas.API en puerto 5003..." -ForegroundColor Cyan
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$baseDir\src\Reservas.API'; dotnet run --launch-profile http"

Write-Host ""
Write-Host "Todos los microservicios han sido iniciados!" -ForegroundColor Green
Write-Host ""
Write-Host "URLs de los microservicios:" -ForegroundColor Yellow
Write-Host "  - Usuarios API: http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  - Airbnbs API:  http://localhost:5002/swagger" -ForegroundColor White
Write-Host "  - Reservas API: http://localhost:5003/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Presiona Ctrl+C en cada terminal para detener los servicios" -ForegroundColor Yellow
