using System.ComponentModel.DataAnnotations;
namespace Veterinaria.Entidades.DTOs;
public class RegistroDto
{
    [Required, EmailAddress, StringLength(254)] public string Correo { get; set; } = "";
    [Required, StringLength(72, MinimumLength = 8)] public string Clave { get; set; } = "";
}
public class LoginDto : RegistroDto { }
public record TokenDto(string Token, DateTime Expiracion);
public record PerfilDto(string Correo, string Rol);
