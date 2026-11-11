$ErrorActionPreference = 'Stop'
if (-not $env:ConnectionStrings__Veterinaria) { throw 'Configure ConnectionStrings__Veterinaria fuera del repositorio.' }
dotnet tool restore
dotnet ef dbcontext scaffold 'Name=ConnectionStrings:Veterinaria' Microsoft.EntityFrameworkCore.SqlServer --project Veterinaria.Datos --startup-project Veterinaria.Api --output-dir Models --context-dir . --context VeterinariaContext --namespace Veterinaria.Entidades --context-namespace Veterinaria.Datos --no-onconfiguring --force
if ($LASTEXITCODE -ne 0) { throw 'Scaffold fallido' }
Move-Item -LiteralPath Veterinaria.Datos/Models/Propietario.cs -Destination Veterinaria.Entidades/Propietario.cs -Force
Move-Item -LiteralPath Veterinaria.Datos/Models/Mascota.cs -Destination Veterinaria.Entidades/Mascota.cs -Force
dotnet build
