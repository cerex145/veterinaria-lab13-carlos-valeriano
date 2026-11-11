using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;
namespace Veterinaria.Datos.Repositorios;

public interface IPropietarioRepositorio
{
    Task<List<Propietario>> ListarAsync(string? nombre);
    Task<Propietario?> ObtenerAsync(int id);
    Task<bool> DniExisteAsync(string dni, int excepto = 0);
    Task<bool> TieneMascotasAsync(int id);
    Task CrearAsync(Propietario entidad);
    Task GuardarAsync();
    Task EliminarAsync(Propietario entidad);
}
public class PropietarioRepositorio(VeterinariaContext db) : IPropietarioRepositorio
{
    public async Task<List<Propietario>> ListarAsync(string? nombre) => await db.Propietarios.AsNoTracking().Where(x => nombre == null || x.Nombre.Contains(nombre)).OrderBy(x => x.Id).ToListAsync();
    public async Task<Propietario?> ObtenerAsync(int id) => await db.Propietarios.FirstOrDefaultAsync(x => x.Id == id);
    public async Task<bool> DniExisteAsync(string dni, int excepto = 0) => await db.Propietarios.AnyAsync(x => x.Dni == dni && x.Id != excepto);
    public async Task<bool> TieneMascotasAsync(int id) => await db.Mascotas.AnyAsync(x => x.PropietarioId == id);
    public async Task CrearAsync(Propietario entidad) { await db.Propietarios.AddAsync(entidad); await db.SaveChangesAsync(); }
    public async Task GuardarAsync() => await db.SaveChangesAsync();
    public async Task EliminarAsync(Propietario entidad) { db.Propietarios.Remove(entidad); await db.SaveChangesAsync(); }
}
public interface IMascotaRepositorio
{
    Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId);
    Task<Mascota?> ObtenerAsync(int id);
    Task CrearAsync(Mascota entidad);
    Task GuardarAsync();
    Task EliminarAsync(Mascota entidad);
}
public class MascotaRepositorio(VeterinariaContext db) : IMascotaRepositorio
{
    public async Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId) => await db.Mascotas.AsNoTracking().Include(x => x.Propietario).Where(x => (especie == null || x.Especie == especie) && (propietarioId == null || x.PropietarioId == propietarioId)).OrderBy(x => x.Id).ToListAsync();
    public async Task<Mascota?> ObtenerAsync(int id) => await db.Mascotas.Include(x => x.Propietario).FirstOrDefaultAsync(x => x.Id == id);
    public async Task CrearAsync(Mascota entidad) { await db.Mascotas.AddAsync(entidad); await db.SaveChangesAsync(); }
    public async Task GuardarAsync() => await db.SaveChangesAsync();
    public async Task EliminarAsync(Mascota entidad) { db.Mascotas.Remove(entidad); await db.SaveChangesAsync(); }
}
