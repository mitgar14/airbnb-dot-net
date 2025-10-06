# Script para verificar que las APIs están en ejecución

Write-Host "Verificando estado de los microservicios..." -ForegroundColor Cyan
Write-Host ""

$apis = @(
    @{Name = "Usuarios API"; Url = "http://localhost:5001/swagger/index.html"},
    @{Name = "Airbnbs API"; Url = "http://localhost:5002/swagger/index.html"},
    @{Name = "Reservas API"; Url = "http://localhost:5003/swagger/index.html"}
)

foreach ($api in $apis) {
    try {
        $response = Invoke-WebRequest -Uri $api.Url -Method Get -TimeoutSec 5 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host "✓ $($api.Name) está en ejecución" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "✗ $($api.Name) no está respondiendo" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Verificación completada" -ForegroundColor Yellow
