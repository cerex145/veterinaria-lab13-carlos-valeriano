using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Negocio;
namespace Veterinaria.Api.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(IAuthServicio servicio) : ControllerBase
{
    [AllowAnonymous, HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroDto dto)
    {
        try { return StatusCode(201, await servicio.RegistrarAsync(dto)); }
        catch (ReglaNegocioException ex) { return Conflict(new ProblemDetails { Status = 409, Title = "Regla de negocio", Detail = ex.Message }); }
    }
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto) => await servicio.LoginAsync(dto) is { } token ? Ok(token) : Unauthorized();
    [Authorize, HttpGet("perfil")]
    public IActionResult Perfil() => Ok(new PerfilDto(User.FindFirst("email")!.Value, User.FindFirst("rol")!.Value));
}
