using System.Text;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

var jwt = builder.Configuration.GetSection("Jwt");
var key = jwt["Key"] ?? throw new InvalidOperationException("Configure Jwt__Key.");
if (Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Jwt__Key requiere al menos 32 bytes.");
var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("Configure Jwt__Issuer.");
var audience = jwt["Audience"] ?? throw new InvalidOperationException("Configure Jwt__Audience.");
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidIssuer = issuer, ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
        NameClaimType = "email", RoleClaimType = "rol", ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
var vacunas = new[]
{
    new Vacuna(1, "Antirrabica", "Perro y gato"),
    new Vacuna(2, "Triple felina", "Gato"),
    new Vacuna(3, "Quintuple canina", "Perro")
};
app.UseAuthentication();
app.UseAuthorization();
var catalogo = app.MapGroup("/api/vacunas").RequireAuthorization();
catalogo.MapGet("", () => Results.Ok(vacunas));
catalogo.MapGet("/{id:int}", (int id) => vacunas.FirstOrDefault(x => x.Id == id) is { } vacuna
    ? Results.Ok(vacuna) : Results.NotFound());
app.Run();
record Vacuna(int Id, string Nombre, string Especie);
