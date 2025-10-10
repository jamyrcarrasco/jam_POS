using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using jam_POS.Infrastructure.Data;
using jam_POS.Infrastructure.Services;
using jam_POS.Application.Services;
using jam_POS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 🧩 CONFIGURACIÓN DE SERVICIOS
// ============================================

// Controladores + vistas (necesario para SPA)
builder.Services.AddControllersWithViews();

// Archivos estáticos del Angular compilado
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

// Entity Framework + PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias de servicios de aplicación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<JwtService>();

// Multi-Tenant Services
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// ============================================
// 🔐 JWT Authentication
// ============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// ============================================
// 🌐 CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:44425",
                "http://localhost:44425")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ============================================
// 📘 SWAGGER (Documentación de la API)
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "jam_POS API",
        Version = "v1",
        Description = "API REST para el sistema POS con arquitectura limpia y Angular embebido.",
        Contact = new OpenApiContact
        {
            Name = "Jamyr Carrasco",
            Email = "soporte@jamyr.dev",
            Url = new Uri("https://github.com/jamyrcarrasco")
        }
    });

    // 🔐 Configuración de JWT en Swagger
    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese 'Bearer {token}' para autenticar.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});

// ============================================
// 🚀 CONSTRUIR APP
// ============================================
var app = builder.Build();

// ============================================
// 🌍 PIPELINE HTTP
// ============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "jam_POS API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

// CORS
app.UseCors("AllowAngularApp");

// Seguridad
app.UseAuthentication();

// Multi-Tenant Middleware (DEBE ir después de Authentication)
app.UseTenantMiddleware();

app.UseAuthorization();

// Endpoints
app.MapControllers();

// SPA Fallback (sirve Angular compilado)
app.MapFallbackToFile("index.html");

// ============================================
// 🏁 RUN
// ============================================
app.Run();
