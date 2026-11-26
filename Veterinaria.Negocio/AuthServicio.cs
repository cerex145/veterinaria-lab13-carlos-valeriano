using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.DTOs;
namespace Veterinaria.Negocio;
public interface IAuthServicio
{
    Task<PerfilDto> RegistrarAsync(RegistroDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}
public class AuthServicio(IUsuarioRepositorio repo, IConfiguration config) : IAuthServicio
{
    public async Task<PerfilDto> RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();
        if (Encoding.UTF8.GetByteCount(dto.Clave) > 72) throw new ReglaNegocioException("La clave excede 72 bytes.");
        if (await repo.BuscarPorCorreoAsync(correo) is not null) throw new ReglaNegocioException("El correo ya esta registrado.");
        var usuario = new Usuario { Correo = correo, ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave), Rol = "Recepcionista" };
        await repo.AgregarAsync(usuario);
        try { await repo.GuardarAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" }
            or Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
        { throw new ReglaNegocioException("El correo ya esta registrado."); }
        return new PerfilDto(usuario.Correo, usuario.Rol);
    }
    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await repo.BuscarPorCorreoAsync(dto.Correo.Trim().ToLowerInvariant());
        if (usuario is null || Encoding.UTF8.GetByteCount(dto.Clave) > 72 || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash)) return null;
        var jwt = config.GetSection("Jwt");
        var minutos = int.Parse(jwt["Minutes"] ?? throw new InvalidOperationException("Configure Jwt__Minutes."));
        if (minutos <= 0) throw new InvalidOperationException("Jwt__Minutes debe ser positivo.");
        var expiracion = DateTime.UtcNow.AddMinutes(minutos);
        var claims = new[] { new Claim("sub", usuario.Id.ToString()), new Claim("email", usuario.Correo), new Claim("rol", usuario.Rol) };
        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims, DateTime.UtcNow, expiracion,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)), SecurityAlgorithms.HmacSha256));
        return new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), expiracion);
    }
}
