using System.ComponentModel.DataAnnotations;
namespace Veterinaria.Entidades.DTOs;

public class MascotaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public int Edad { get; set; }
    public int PropietarioId { get; set; }
    public string PropietarioNombre { get; set; } = "";
}
public class MascotaCrearDto
{
    [Required, StringLength(80, MinimumLength = 2)] public string Nombre { get; set; } = "";
    [Required, StringLength(40)] public string Especie { get; set; } = "";
    [Range(0, 40)] public int Edad { get; set; }
    [Range(1, int.MaxValue)] public int PropietarioId { get; set; }
}
public class MascotaActualizarDto : MascotaCrearDto { }
