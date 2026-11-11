using System;
using System.Collections.Generic;

namespace Veterinaria.Entidades;

public partial class Propietario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public virtual ICollection<Mascota> Mascota { get; set; } = new List<Mascota>();
}
