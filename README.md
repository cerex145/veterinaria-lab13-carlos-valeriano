# Veterinaria Lab 13
Autor: Carlos Alberto Valeriano Colan.

API REST ASP.NET Core .NET 10, EF Core SQL Server y AutoMapper. Cuatro proyectos con referencias Api → Negocio → Datos → Entidades.

## Ejecución
1. Instalar SDK .NET 10 y disponer de SQL Server 2022 o posterior.
2. Ejecutar `sql/01_veterinaria.sql` en SQL Server. Es idempotente y contiene datos de ejemplo.
3. Configurar la conexión fuera del repositorio: `$env:ConnectionStrings__Veterinaria = <conexión local>`.
4. `dotnet restore` y `dotnet build`.
5. `dotnet dev-certs https --trust` y `dotnet run --project Veterinaria.Api --launch-profile https`.
6. Abrir https://localhost:7243/swagger.

## Modelo y reglas
Un propietario tiene muchas mascotas; Mascota.PropietarioId es la FK obligatoria.
Se rechaza el DNI duplicado, eliminar propietarios con mascotas y registrar o trasladar una mascota a un propietario inexistente. Los servicios lanzan ReglaNegocioException y los controladores responden 409.

## Endpoints
GET/POST `/api/Propietarios`; GET/PUT/DELETE `/api/Propietarios/{id}`; GET `/api/Propietarios/{id}/mascotas`.
GET/POST `/api/Mascotas`; GET/PUT/DELETE `/api/Mascotas/{id}`.
Filtros: `?nombre=Ana`, `?especie=Perro&propietarioId=1`.

Las entradas tienen Data Annotations, las respuestas usan DTOs y no se aplica JsonIgnore a las entidades. El MappingProfile contiene seis mapas explícitos sin ReverseMap.

## Base de datos primero
Consultar `scripts/scaffold.ps1` y la evidencia de ejecución. El contexto se genera en Datos y las entidades se ubican en Entidades. No se almacenan conexiones ni contraseñas en el repositorio.

## Pruebas
El archivo `pruebas/endpoints.http` permite repetir las peticiones; `pruebas/resultados.json` conserva los resultados reales de la ejecución documentada.
