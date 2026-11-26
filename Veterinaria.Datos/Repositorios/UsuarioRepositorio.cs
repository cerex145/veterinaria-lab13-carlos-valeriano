using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades;
namespace Veterinaria.Datos.Repositorios;
public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorCorreoAsync(string correo);
    Task AgregarAsync(Usuario usuario);
    Task GuardarAsync();
}
public class UsuarioRepositorio(VeterinariaContext db) : IUsuarioRepositorio
{
    public Task<Usuario?> BuscarPorCorreoAsync(string correo) => db.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.Correo == correo);
    public async Task AgregarAsync(Usuario usuario) => await db.Usuarios.AddAsync(usuario);
    public async Task GuardarAsync() => await db.SaveChangesAsync();
}
