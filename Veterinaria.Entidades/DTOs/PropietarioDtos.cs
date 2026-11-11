using System.ComponentModel.DataAnnotations;
namespace Veterinaria.Entidades.DTOs;

public record PropietarioDto(int Id, string Nombre, string Dni, string Telefono);
public class PropietarioCrearDto
{
    [Required, StringLength(100, MinimumLength = 2)] public string Nombre { get; set; } = "";
    [Required, StringLength(8, MinimumLength = 8), RegularExpression(@"^\d{8}$")] public string Dni { get; set; } = "";
    [Required, StringLength(20, MinimumLength = 6)] public string Telefono { get; set; } = "";
}
public class PropietarioActualizarDto : PropietarioCrearDto { }
