using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using MiApi.Auth;
using MiApi.Data;
using MiApi.Services;
using MiApi.Entities;
using MiApi.Repositories;


var builder = WebApplication.CreateBuilder(args);

// ---------- Configuración JWT ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

// ---------- Base de datos ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// ---------- Servicios ----------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<ITipoCafeRepository, TipoCafeRepository>();
builder.Services.AddScoped<ITipoCafeService, TipoCafeService>();
builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
builder.Services.AddScoped<IEntradaService, EntradaService>();
builder.Services.AddScoped<ISalidaRepository, SalidaRepository>();
builder.Services.AddScoped<ISalidaService, SalidaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Semillas

// Semilla de usuario administrador
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.AdminUsers.Any())
    {
        var admin = new AdminUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
        };
        context.AdminUsers.Add(admin);
        context.SaveChanges();
    }
}

// Semilla de secciones
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Secciones.Any())
    {
        context.Secciones.AddRange(
            new SeccionAlmacenamiento { Nombre = "Café en grano", Descripcion = "Almacenamiento de café sin moler" },
            new SeccionAlmacenamiento { Nombre = "Café molido", Descripcion = "Almacenamiento de café molido" },
            new SeccionAlmacenamiento { Nombre = "Café instantáneo", Descripcion = "Almacenamiento de café soluble" },
            new SeccionAlmacenamiento { Nombre = "Extracto de café", Descripcion = "Almacenamiento de extracto concentrado" }
        );
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();   // <--- importante antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();