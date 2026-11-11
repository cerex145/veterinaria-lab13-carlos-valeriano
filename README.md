# Veterinaria Lab 13
Autor: Carlos Alberto Valeriano Colan.

API REST ASP.NET Core .NET 10, EF Core SQL Server y AutoMapper. Cuatro proyectos con referencias Api → Negocio → Datos → Entidades.

## Ejecución
1. Instalar SDK .NET 10 y disponer de SQL Server 2022 o posterior.
2. Ejecutar `sql/01_veterinaria.sql` en SQL Server. Es idempotente y contiene datos de ejemplo.
3. Configurar la variable de entorno `ConnectionStrings__Veterinaria` con la conexión de la base creada. Establecer su valor en la terminal local; no guardarlo ni publicarlo en archivos del repositorio. La API también admite User Secrets con la clave `ConnectionStrings:Veterinaria`.
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
Las 27 pruebas documentadas pasaron contra SQL Server 2022: 11 endpoints, filtros, ruta anidada, cabeceras Location, DTOs y códigos 200/201/204/400/404/409. Las altas de prueba se eliminan al finalizar; los datos de ejemplo permanecen.

Con la API iniciada, ejecutar `python pruebas/probar.py` (Python 3, sin dependencias externas). También se pueden ejecutar, en orden, las peticiones de `pruebas/endpoints.http` mediante REST Client de VS Code. Los identificadores de las altas se toman de las respuestas de POST.

`pruebas/resultados.json` conserva el registro original de ejecución y `pruebas/informe-pruebas.html` permite ver las respuestas sin iniciar la API. Las imágenes están en `evidencias/`; `evidencias/scaffold.txt` registra el Scaffold real y la compilación correcta.

El cliente automatizado acepta el certificado HTTPS de desarrollo de localhost. Swagger se usa en entorno Development. El aviso de licencia de AutoMapper en desarrollo es el esperado por la guía.

## Entrega
Repositorio: https://github.com/cerex145/veterinaria-lab13-carlos-valeriano

El informe conserva la plantilla original del laboratorio y contiene la explicación del tema, la sustentación de los seis mapeos, las capturas y las conclusiones. Las capturas del explorador y del código se obtuvieron del editor web GitHub1s sobre este mismo repositorio.
