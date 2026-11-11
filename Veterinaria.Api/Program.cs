using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Veterinaria.Datos;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<VeterinariaContext>(options => options.UseSqlServer(
 builder.Configuration.GetConnectionString("Veterinaria") ?? throw new InvalidOperationException("Configure ConnectionStrings__Veterinaria mediante variable de entorno o User Secrets.")));
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
