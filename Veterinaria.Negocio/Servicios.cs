using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.DTOs;
namespace Veterinaria.Negocio;

public class ReglaNegocioException(string mensaje) : Exception(mensaje);
public interface IPropietarioServicio
{
    Task<List<PropietarioDto>> ListarAsync(string? nombre);
    Task<PropietarioDto?> ObtenerAsync(int id);
    Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto);
    Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
public class PropietarioServicio(IPropietarioRepositorio repo, IMapper mapper) : IPropietarioServicio
{
    public async Task<List<PropietarioDto>> ListarAsync(string? nombre) => mapper.Map<List<PropietarioDto>>(await repo.ListarAsync(nombre));
    public async Task<PropietarioDto?> ObtenerAsync(int id) => mapper.Map<PropietarioDto?>(await repo.ObtenerAsync(id));
    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        if (await repo.DniExisteAsync(dto.Dni)) throw new ReglaNegocioException("Ya existe un propietario con ese DNI.");
        var entidad = mapper.Map<Propietario>(dto); await repo.CrearAsync(entidad); return mapper.Map<PropietarioDto>(entidad);
    }
    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var entidad = await repo.ObtenerAsync(id); if (entidad is null) return false;
        if (await repo.DniExisteAsync(dto.Dni, id)) throw new ReglaNegocioException("Ya existe un propietario con ese DNI.");
        mapper.Map(dto, entidad); await repo.GuardarAsync(); return true;
    }
    public async Task<bool> EliminarAsync(int id)
    {
        var entidad = await repo.ObtenerAsync(id); if (entidad is null) return false;
        if (await repo.TieneMascotasAsync(id)) throw new ReglaNegocioException("No se puede eliminar un propietario con mascotas registradas.");
        await repo.EliminarAsync(entidad); return true;
    }
}
public interface IMascotaServicio
{
    Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId);
    Task<MascotaDto?> ObtenerAsync(int id);
    Task<MascotaDto> CrearAsync(MascotaCrearDto dto);
    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
public class MascotaServicio(IMascotaRepositorio repo, IPropietarioRepositorio propietarios, IMapper mapper) : IMascotaServicio
{
    public async Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId) => mapper.Map<List<MascotaDto>>(await repo.ListarAsync(especie, propietarioId));
    public async Task<MascotaDto?> ObtenerAsync(int id) => mapper.Map<MascotaDto?>(await repo.ObtenerAsync(id));
    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        if (await propietarios.ObtenerAsync(dto.PropietarioId) is null) throw new ReglaNegocioException("El propietario indicado no existe.");
        var entidad = mapper.Map<Mascota>(dto); await repo.CrearAsync(entidad);
        return mapper.Map<MascotaDto>(await repo.ObtenerAsync(entidad.Id));
    }
    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var entidad = await repo.ObtenerAsync(id); if (entidad is null) return false;
        if (await propietarios.ObtenerAsync(dto.PropietarioId) is null) throw new ReglaNegocioException("El propietario indicado no existe.");
        mapper.Map(dto, entidad); await repo.GuardarAsync(); return true;
    }
    public async Task<bool> EliminarAsync(int id)
    {
        var entidad = await repo.ObtenerAsync(id); if (entidad is null) return false;
        await repo.EliminarAsync(entidad); return true;
    }
}
