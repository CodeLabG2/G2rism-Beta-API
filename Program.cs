using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Repositories;
using G2rismBeta.API.Services;
using G2rismBeta.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURACIÓN DE SERVICIOS
// ========================================

// Agregar DbContext con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


// ============================================
// CONFIGURACIÓN DE AUTOMAPPER
// ============================================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========================================
// REGISTRAR REPOSITORIES - ROLES Y PERMISOS
// ========================================

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IRolPermisoRepository, RolPermisoRepository>();

// ========================================
// REGISTRAR REPOSITORIES - USUARIOS Y AUTENTICACIÓN
// ========================================

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
builder.Services.AddScoped<ITokenRecuperacionRepository, TokenRecuperacionRepository>();

// ========================================
// REGISTRAR REPOSITORIES - CLIENTES Y EMPLEADOS (CRM)
// ========================================

builder.Services.AddScoped<ICategoriaClienteRepository, CategoriaClienteRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPreferenciaClienteRepository, PreferenciaClienteRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();

// ========================================
// REGISTRAR REPOSITORIES - PROVEEDORES
// ========================================

builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IContratoProveedorRepository, ContratoProveedorRepository>();

// ========================================
// REGISTRAR REPOSITORIES - AEROLÍNEAS
// ========================================

builder.Services.AddScoped<IAerolineaRepository, AerolineaRepository>();

// ========================================
// REGISTRAR SERVICES - ROLES Y PERMISOS
// ========================================

builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();

// ========================================
// REGISTRAR SERVICES - USUARIOS Y AUTENTICACIÓN
// ========================================
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ========================================
// REGISTRAR SERVICES - CLIENTES Y EMPLEADOS (CRM)
// ========================================

builder.Services.AddScoped<ICategoriaClienteService, CategoriaClienteService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPreferenciaClienteService, PreferenciaClienteService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

// ========================================
// REGISTRAR SERVICES - PROVEEDORES
// ========================================

builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IContratoProveedorService, ContratoProveedorService>();

// ========================================
// REGISTRAR SERVICES - AEROLÍNEAS
// ========================================

builder.Services.AddScoped<IAerolineaService, AerolineaService>();

// ========================================
// REGISTRAR FLUENTVALIDATION
// ========================================

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Agregar controladores
builder.Services.AddControllers();

// Configurar Swagger para documentación de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "G2rism Beta API - Módulo de Configuración",
        Version = "v1.0",
        Description = "API para gestión de Roles y Permisos del sistema G2rism",
        Contact = new OpenApiContact
        {
            Name = "CodeLabG2",
            Email = "codelabg2@example.com"
        }
    });
});

// Configurar CORS (importante para cuando conecten el frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========================================
// INICIALIZAR BASE DE DATOS (SEEDING)
// ========================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.Initialize(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al inicializar la base de datos");
    }
}

// ========================================
// REGISTRAR MIDDLEWARE DE ERRORES
// ========================================

// IMPORTANTE: Debe ser el PRIMERO de todos los middlewares
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// ========================================
// CONFIGURACIÓN DEL PIPELINE HTTP
// ========================================

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "G2rism Beta API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5000/)
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Mensaje de bienvenida en consola
Console.WriteLine("╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║         🚀 G2RISM BETA API - CONFIGURACIÓN           ║");
Console.WriteLine("║              CodeLabG2 - Sistema de Turismo            ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("✅ API iniciada correctamente");
Console.WriteLine("📄 Documentación: http://localhost:5000/");
Console.WriteLine("🔧 Módulos activos:");
Console.WriteLine("   • Configuración (Roles y Permisos)");
Console.WriteLine("   • Usuarios (Gestión y Autenticación)");
Console.WriteLine("   • Clientes (CRM - Categorías, Clientes, Preferencias)");
Console.WriteLine("   • Empleados (CRM - Gestión de Personal)");
Console.WriteLine("   • Proveedores (Contratos de proveedores)");
Console.WriteLine("   • Servicios (Aerolíneas)");
Console.WriteLine();

app.Run();