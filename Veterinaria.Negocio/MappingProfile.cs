using AutoMapper;
using Veterinaria.Entidades;
using Veterinaria.Entidades.DTOs;
namespace Veterinaria.Negocio;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Propietario, PropietarioDto>();
        CreateMap<PropietarioCrearDto, Propietario>()
         .ForMember(d => d.Id, o => o.Ignore())
         .ForMember(d => d.Mascota, o => o.Ignore());
        CreateMap<PropietarioActualizarDto, Propietario>()
         .ForMember(d => d.Id, o => o.Ignore())
         .ForMember(d => d.Mascota, o => o.Ignore());
        CreateMap<Mascota, MascotaDto>()
         .ForMember(d => d.PropietarioNombre, o => o.MapFrom(s => s.Propietario.Nombre));
        CreateMap<MascotaCrearDto, Mascota>()
         .ForMember(d => d.Id, o => o.Ignore())
         .ForMember(d => d.Propietario, o => o.Ignore());
        CreateMap<MascotaActualizarDto, Mascota>()
         .ForMember(d => d.Id, o => o.Ignore())
         .ForMember(d => d.Propietario, o => o.Ignore());
    }
}
