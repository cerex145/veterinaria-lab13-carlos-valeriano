namespace Veterinaria.Entidades;
public class Usuario
{
    public int Id { get; set; }
    public string Correo { get; set; } = "";
    public string ClaveHash { get; set; } = "";
    public string Rol { get; set; } = "Recepcionista";
}
