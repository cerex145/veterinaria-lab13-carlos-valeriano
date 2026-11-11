using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Negocio;
namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController(IMascotaServicio servicio) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MascotaDto>>> Listar([FromQuery] string? especie, [FromQuery] int? propietarioId) => Ok(await servicio.ListarAsync(especie, propietarioId));
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MascotaDto>> Obtener(int id) { var dto = await servicio.ObtenerAsync(id); return dto is null ? NotFound() : Ok(dto); }
    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Crear(MascotaCrearDto dto)
    {
        try { var creado = await servicio.CrearAsync(dto); return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado); }
        catch (ReglaNegocioException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Regla de negocio", Detail = ex.Message }); }
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, MascotaActualizarDto dto)
    {
        try { return await servicio.ActualizarAsync(id, dto) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Regla de negocio", Detail = ex.Message }); }
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try { return await servicio.EliminarAsync(id) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Regla de negocio", Detail = ex.Message }); }
    }

}
