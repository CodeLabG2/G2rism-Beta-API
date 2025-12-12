using G2rismBeta.API.Models;
using Microsoft.EntityFrameworkCore;
using BCryptHasher = BCrypt.Net.BCrypt;

namespace G2rismBeta.API.Data;

/// <summary>
/// Clase para inicializar la base de datos con datos por defecto
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Inicializa la base de datos con roles y permisos por defecto
    /// </summary>
    public static async Task Initialize(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            // ========================================
            // APLICAR MIGRACIONES PENDIENTES
            // ========================================

            logger.LogInformation("🔄 Verificando migraciones pendientes...");

            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("⚙️  Aplicando migraciones...");
                await context.Database.MigrateAsync();
                logger.LogInformation("✅ Migraciones aplicadas correctamente");
            }
            else
            {
                logger.LogInformation("✅ Base de datos actualizada");
            }

            // ========================================
            // VERIFICAR SI YA HAY DATOS
            // ========================================

            if (await context.Roles.AnyAsync())
            {
                logger.LogInformation("ℹ️  La base de datos ya contiene datos de roles y usuarios. Omitiendo seeding principal.");

                // ========================================
                // 🆕 SEEDING: FORMAS DE PAGO (INDEPENDIENTE)
                // ========================================
                // Este seeding se ejecuta incluso si ya existen roles

                if (!await context.FormasDePago.AnyAsync())
                {
                    logger.LogInformation("💳 Creando formas de pago iniciales...");

                    var formasDePago = new List<FormaDePago>
                    {
                        new FormaDePago
                        {
                            Metodo = "Efectivo",
                            Descripcion = "Pago en efectivo al momento de la reserva",
                            RequiereVerificacion = false,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "Tarjeta de Crédito",
                            Descripcion = "Acepta Visa, Mastercard y American Express",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "Tarjeta de Débito",
                            Descripcion = "Débito de todas las entidades bancarias",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "Transferencia Bancaria",
                            Descripcion = "Transferencia a cuenta bancaria de la empresa",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "PSE",
                            Descripcion = "Pagos Seguros en Línea",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "Nequi",
                            Descripcion = "Pago mediante billetera digital Nequi",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        },
                        new FormaDePago
                        {
                            Metodo = "Daviplata",
                            Descripcion = "Pago mediante billetera digital Daviplata",
                            RequiereVerificacion = true,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        }
                    };

                    await context.FormasDePago.AddRangeAsync(formasDePago);
                    await context.SaveChangesAsync();

                    logger.LogInformation($"✅ {formasDePago.Count} formas de pago creadas correctamente");
                }
                else
                {
                    logger.LogInformation("ℹ️  Formas de pago ya existen. Omitiendo seeding de formas de pago.");
                }

                return;
            }

            logger.LogInformation("🌱 Iniciando seeding de datos iniciales...");

            // ========================================
            // CREAR ROLES INICIALES
            // ========================================

            logger.LogInformation("📝 Creando roles iniciales...");

            var roles = new List<Rol>
                {
                    new Rol
                    {
                        Nombre = "Super Administrador",
                        Descripcion = "Acceso total al sistema. Puede gestionar todo sin restricciones.",
                        NivelAcceso = 1,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    },
                    new Rol
                    {
                        Nombre = "Administrador",
                        Descripcion = "Gestión completa del sistema excepto configuraciones críticas.",
                        NivelAcceso = 2,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    },
                    new Rol
                    {
                        Nombre = "Empleado",
                        Descripcion = "Acceso a operaciones básicas del sistema.",
                        NivelAcceso = 10,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    },
                    new Rol
                    {
                        Nombre = "Cliente",
                        Descripcion = "Acceso limitado solo para consultas y reservas personales.",
                        NivelAcceso = 50,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    }
                };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ {roles.Count} roles creados correctamente");

            // ========================================
            // CREAR PERMISOS INICIALES
            // ========================================

            logger.LogInformation("📝 Creando permisos iniciales...");

            var permisos = new List<Permiso>
                {
                    // Permisos para Roles
                    new Permiso
                    {
                        Modulo = "Roles",
                        Accion = "Crear",
                        NombrePermiso = "roles.crear",
                        Descripcion = "Permite crear nuevos roles en el sistema",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Roles",
                        Accion = "Leer",
                        NombrePermiso = "roles.leer",
                        Descripcion = "Permite ver la lista de roles y sus detalles",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Roles",
                        Accion = "Actualizar",
                        NombrePermiso = "roles.actualizar",
                        Descripcion = "Permite modificar roles existentes",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Roles",
                        Accion = "Eliminar",
                        NombrePermiso = "roles.eliminar",
                        Descripcion = "Permite eliminar roles del sistema",
                        Estado = true
                    },

                    // Permisos para Permisos
                    new Permiso
                    {
                        Modulo = "Permisos",
                        Accion = "Crear",
                        NombrePermiso = "permisos.crear",
                        Descripcion = "Permite crear nuevos permisos",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Permisos",
                        Accion = "Leer",
                        NombrePermiso = "permisos.leer",
                        Descripcion = "Permite ver la lista de permisos",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Permisos",
                        Accion = "Actualizar",
                        NombrePermiso = "permisos.actualizar",
                        Descripcion = "Permite modificar permisos existentes",
                        Estado = true
                    },
                    new Permiso
                    {
                        Modulo = "Permisos",
                        Accion = "Eliminar",
                        NombrePermiso = "permisos.eliminar",
                        Descripcion = "Permite eliminar permisos",
                        Estado = true
                    }
                };

            await context.Permisos.AddRangeAsync(permisos);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ {permisos.Count} permisos creados correctamente");

            // ========================================
            // ASIGNAR PERMISOS A ROLES
            // ========================================

            logger.LogInformation("🔗 Asignando permisos a roles...");

            // Super Administrador: TODOS los permisos
            var superAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Super Administrador");
            var todosLosPermisos = await context.Permisos.ToListAsync();

            var asignacionesSuperAdmin = todosLosPermisos.Select(p => new RolPermiso
            {
                IdRol = superAdmin.IdRol,
                IdPermiso = p.IdPermiso,
                FechaAsignacion = DateTime.Now
            }).ToList();

            await context.RolesPermisos.AddRangeAsync(asignacionesSuperAdmin);

            // Administrador: Todos excepto eliminar roles
            var admin = await context.Roles.FirstAsync(r => r.Nombre == "Administrador");
            var permisosAdmin = todosLosPermisos
                .Where(p => p.NombrePermiso != "roles.eliminar")
                .ToList();

            var asignacionesAdmin = permisosAdmin.Select(p => new RolPermiso
            {
                IdRol = admin.IdRol,
                IdPermiso = p.IdPermiso,
                FechaAsignacion = DateTime.Now
            }).ToList();

            await context.RolesPermisos.AddRangeAsync(asignacionesAdmin);

            // Empleado: Solo lectura
            var empleado = await context.Roles.FirstAsync(r => r.Nombre == "Empleado");
            var permisosEmpleado = todosLosPermisos
                .Where(p => p.Accion == "Leer")
                .ToList();

            var asignacionesEmpleado = permisosEmpleado.Select(p => new RolPermiso
            {
                IdRol = empleado.IdRol,
                IdPermiso = p.IdPermiso,
                FechaAsignacion = DateTime.Now
            }).ToList();

            await context.RolesPermisos.AddRangeAsync(asignacionesEmpleado);

            // Cliente: Sin permisos de configuración
            // (Los clientes no deberían ver roles ni permisos)

            await context.SaveChangesAsync();

            logger.LogInformation($"✅ Permisos asignados correctamente");

            // ========================================
            // CREAR USUARIOS DE PRUEBA
            // ========================================

            logger.LogInformation("👤 Creando usuarios de prueba...");

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    Username = "admin",
                    Email = "admin@g2rism.com",
                    PasswordHash = BCryptHasher.HashPassword("Admin123!", workFactor: 11),
                    TipoUsuario = "empleado",
                    Estado = true,
                    Bloqueado = false,
                    FechaCreacion = DateTime.Now
                },
                new Usuario
                {
                    Username = "empleado1",
                    Email = "empleado1@g2rism.com",
                    PasswordHash = BCryptHasher.HashPassword("Empleado123!", workFactor: 11),
                    TipoUsuario = "empleado",
                    Estado = true,
                    Bloqueado = false,
                    FechaCreacion = DateTime.Now
                },
                new Usuario
                {
                    Username = "cliente1",
                    Email = "cliente1@gmail.com",
                    PasswordHash = BCryptHasher.HashPassword("Cliente123!", workFactor: 11),
                    TipoUsuario = "cliente",
                    Estado = true,
                    Bloqueado = false,
                    FechaCreacion = DateTime.Now
                }
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ {usuarios.Count} usuarios creados correctamente");

            // ========================================
            // ASIGNAR ROLES A USUARIOS
            // ========================================

            logger.LogInformation("🔗 Asignando roles a usuarios...");

            // Admin → Super Administrador
            var adminUsuario = await context.Usuarios.FirstAsync(u => u.Username == "admin");
            var rolSuperAdmin = await context.Roles.FirstAsync(r => r.Nombre == "Super Administrador");

            var asignacionAdmin = new UsuarioRol
            {
                IdUsuario = adminUsuario.IdUsuario,
                IdRol = rolSuperAdmin.IdRol,
                FechaAsignacion = DateTime.Now
            };

            await context.UsuariosRoles.AddAsync(asignacionAdmin);

            // Empleado1 → Empleado
            var empleado1 = await context.Usuarios.FirstAsync(u => u.Username == "empleado1");
            var rolEmpleado = await context.Roles.FirstAsync(r => r.Nombre == "Empleado");

            var asignacionEmpleado = new UsuarioRol
            {
                IdUsuario = empleado1.IdUsuario,
                IdRol = rolEmpleado.IdRol,
                FechaAsignacion = DateTime.Now
            };

            await context.UsuariosRoles.AddAsync(asignacionEmpleado);

            // Cliente1 → Cliente
            var cliente1 = await context.Usuarios.FirstAsync(u => u.Username == "cliente1");
            var rolCliente = await context.Roles.FirstAsync(r => r.Nombre == "Cliente");

            var asignacionCliente = new UsuarioRol
            {
                IdUsuario = cliente1.IdUsuario,
                IdRol = rolCliente.IdRol,
                FechaAsignacion = DateTime.Now
            };

            await context.UsuariosRoles.AddAsync(asignacionCliente);

            await context.SaveChangesAsync();

            logger.LogInformation("✅ Roles asignados a usuarios correctamente");

            // ========================================
            // RESUMEN FINAL
            // ========================================

            logger.LogInformation("════════════════════════════════════════════");
            logger.LogInformation("🎉 SEEDING COMPLETADO EXITOSAMENTE");
            logger.LogInformation("════════════════════════════════════════════");
            logger.LogInformation($"📊 Roles creados: {roles.Count}");
            logger.LogInformation($"📊 Permisos creados: {permisos.Count}");
            logger.LogInformation($"📊 Usuarios creados: {usuarios.Count}");
            logger.LogInformation($"📊 Asignaciones Roles-Permisos: {asignacionesSuperAdmin.Count + asignacionesAdmin.Count + asignacionesEmpleado.Count}");
            logger.LogInformation($"📊 Asignaciones Usuarios-Roles: 3");
            logger.LogInformation("════════════════════════════════════════════");
            logger.LogInformation("👤 Usuarios de prueba:");
            logger.LogInformation("   • admin / Admin123! (Super Administrador)");
            logger.LogInformation("   • empleado1 / Empleado123! (Empleado)");
            logger.LogInformation("   • cliente1 / Cliente123! (Cliente)");
            logger.LogInformation("════════════════════════════════════════════");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error durante el seeding de datos iniciales");
            throw;
        }
    }
}
