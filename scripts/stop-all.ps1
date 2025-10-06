# Script para detener todos los procesos de dotnet que puedan estar ejecutándose

Write-Host "Deteniendo todos los microservicios..." -ForegroundColor Yellow

# Obtener todos los procesos de dotnet
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue

if ($dotnetProcesses) {
    Write-Host "Encontrados $($dotnetProcesses.Count) proceso(s) de dotnet en ejecución" -ForegroundColor Cyan
    
    foreach ($process in $dotnetProcesses) {
        try {
            Stop-Process -Id $process.Id -Force
            Write-Host "  Proceso dotnet (PID: $($process.Id)) detenido" -ForegroundColor Green
        }
        catch {
            Write-Host "  Error al detener proceso (PID: $($process.Id)): $_" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "Todos los microservicios han sido detenidos!" -ForegroundColor Green
}
else {
    Write-Host "No se encontraron procesos de dotnet en ejecución" -ForegroundColor Yellow
}
