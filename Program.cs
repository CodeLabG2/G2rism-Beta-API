using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;
using System.Threading.RateLimiting;
using G2rismBeta.API.Data;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Repositories;
using G2rismBeta.API.Services;
using G2rismBeta.API.Middleware;
using G2rismBeta.API.Configuration;
using G2rismBeta.API.Helpers;
using G2rismBeta.API.Authorization;
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

// ========================================
// CONFIGURACIÓN DE SEGURIDAD
// ========================================
builder.Services.Configure<SecuritySettings>(
    builder.Configuration.GetSection("Security"));

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
builder.Services.AddScoped<ITokenRecuperacionRepository, TokenRecuperacionRepository>(); // LEGACY
builder.Services.AddScoped<ICodigoRecuperacionRepository, CodigoRecuperacionRepository>(); // Códigos de 6 dígitos
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

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
// REGISTRAR REPOSITORIES - AEROLÍNEAS, VUELOS, HOTELES Y SERVICIOS ADICIONALES
// ========================================

builder.Services.AddScoped<IAerolineaRepository, AerolineaRepository>();
builder.Services.AddScoped<IVueloRepository, VueloRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IServicioAdicionalRepository, ServicioAdicionalRepository>();
builder.Services.AddScoped<IPaqueteTuristicoRepository, PaqueteTuristicoRepository>();

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
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

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
// REGISTRAR SERVICES - AEROLÍNEAS, VUELOS, HOTELES Y SERVICIOS ADICIONALES
// ========================================

builder.Services.AddScoped<IAerolineaService, AerolineaService>();
builder.Services.AddScoped<IVueloService, VueloService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IServicioAdicionalService, ServicioAdicionalService>();
builder.Services.AddScoped<IPaqueteTuristicoService, PaqueteTuristicoService>();

// ========================================
// CONFIGURACIÓN DE AUTENTICACIÓN JWT
// ========================================

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no está configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // En producción, cambiar a true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // No tolerancia para expiración
    };
});

// ========================================
// CONFIGURACIÓN DE AUTORIZACIÓN Y POLICIES
// ========================================

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    // ====================================================
    // POLICIES BASADAS EN ROLES
    // ====================================================

    // Política: Solo administradores (Super Admin + Admin)
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Super Administrador", "Administrador"));

    // Política: Solo Super Administrador
    options.AddPolicy("RequireSuperAdminRole", policy =>
        policy.RequireRole("Super Administrador"));

    // Política: Empleados (Super Admin + Admin + Empleado)
    options.AddPolicy("RequireEmployeeRole", policy =>
        policy.RequireRole("Super Administrador", "Administrador", "Empleado"));

    // ====================================================
    // POLICIES BASADAS EN PERMISOS - MÓDULO ROLES
    // ====================================================

    options.AddPolicy("RequirePermission:roles.crear", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles.crear")));

    options.AddPolicy("RequirePermission:roles.leer", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles.leer")));

    options.AddPolicy("RequirePermission:roles.actualizar", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles.actualizar")));

    options.AddPolicy("RequirePermission:roles.eliminar", policy =>
        policy.Requirements.Add(new PermissionRequirement("roles.eliminar")));

    // ====================================================
    // POLICIES BASADAS EN PERMISOS - MÓDULO PERMISOS
    // ====================================================

    options.AddPolicy("RequirePermission:permisos.crear", policy =>
        policy.Requirements.Add(new PermissionRequirement("permisos.crear")));

    options.AddPolicy("RequirePermission:permisos.leer", policy =>
        policy.Requirements.Add(new PermissionRequirement("permisos.leer")));

    options.AddPolicy("RequirePermission:permisos.actualizar", policy =>
        policy.Requirements.Add(new PermissionRequirement("permisos.actualizar")));

    options.AddPolicy("RequirePermission:permisos.eliminar", policy =>
        policy.Requirements.Add(new PermissionRequirement("permisos.eliminar")));

    // ====================================================
    // NOTA: Policies adicionales pueden agregarse dinámicamente
    // o crearse on-demand usando el formato "RequirePermission:{permiso}"
    // ====================================================
});

// ========================================
// CONFIGURACIÓN DE RATE LIMITING
// ========================================

builder.Services.AddRateLimiter(options =>
{
    // Política para endpoints de autenticación (login, registro)
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5; // 5 intentos
        opt.Window = TimeSpan.FromMinutes(1); // por minuto
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // Sin cola, rechazar inmediatamente
    });

    // Política para recuperación de contraseña
    options.AddFixedWindowLimiter("password-recovery", opt =>
    {
        opt.PermitLimit = 3; // 3 intentos
        opt.Window = TimeSpan.FromHours(1); // por hora
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Política para refresh token
    options.AddFixedWindowLimiter("refresh", opt =>
    {
        opt.PermitLimit = 10; // 10 renovaciones
        opt.Window = TimeSpan.FromMinutes(1); // por minuto
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Política general para API (endpoints CRUD)
    options.AddSlidingWindowLimiter("api", opt =>
    {
        opt.PermitLimit = 100; // 100 requests
        opt.Window = TimeSpan.FromMinutes(1); // por minuto
        opt.SegmentsPerWindow = 6; // 6 segmentos de 10 segundos
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5; // Cola de 5 requests
    });

    // Limitador global basado en IP (protección general)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 200, // 200 requests totales
                Window = TimeSpan.FromMinutes(1), // por minuto
                SegmentsPerWindow = 6
            }));

    // Configurar respuesta cuando se excede el límite
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        // Calcular tiempo de espera (Retry-After)
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
        }

        var response = new
        {
            success = false,
            message = "Has excedido el límite de solicitudes. Por favor, intenta más tarde.",
            statusCode = 429,
            errorCode = "RateLimitExceeded",
            timestamp = DateTime.UtcNow
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    };
});

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

    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa SOLO el token JWT (sin 'Bearer'). Ejemplo: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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

// Middleware de Rate Limiting (antes de autenticación)
app.UseRateLimiter();

// Middleware de autenticación (debe ir antes de autorización)
app.UseAuthentication();
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