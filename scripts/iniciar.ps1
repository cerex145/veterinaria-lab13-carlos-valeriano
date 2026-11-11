$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
if (-not $env:ConnectionStrings__Veterinaria) {
    throw 'Configura la variable de entorno ConnectionStrings__Veterinaria con la conexión de tu base SQL Server. No la guardes en el repositorio.'
}
dotnet restore
if ($LASTEXITCODE -ne 0) { throw 'La restauración falló.' }
dotnet run --project Veterinaria.Api --launch-profile https
