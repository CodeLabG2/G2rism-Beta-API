using Microsoft.EntityFrameworkCore;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Data;

/// <summary>
/// Contexto de base de datos principal de la aplicación
/// Aquí se definen todas las tablas (DbSets) del sistema
/// </summary>
public class ApplicationDbContext : DbContext
{
    // Constructor que recibe las opciones de configuración
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ========================================
    // DBSETS - TABLAS DE LA BASE DE DATOS
    // ========================================

    // ===============================================
    // MÓDULO: GESTIÓN DE USUARIOS, ROLES Y PERMISOS
    // ===============================================

    /// <summary>
    /// Tabla de Roles del sistema
    /// </summary>
    public DbSet<Rol> Roles { get; set; }

    /// <summary>
    /// Tabla de Permisos del sistema
    /// </summary>
    public DbSet<Permiso> Permisos { get; set; }

    /// <summary>
    /// Tabla intermedia Roles-Permisos (relación N:M)
    /// </summary>
    public DbSet<RolPermiso> RolesPermisos { get; set; }

    /// <summary>
    /// Tabla de Usuarios del sistema
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; }

    /// <summary>
    /// Tabla intermedia Usuarios-Roles (relación N:M)
    /// </summary>
    public DbSet<UsuarioRol> UsuariosRoles { get; set; }

    /// <summary>
    /// Tabla de Tokens de Recuperación
    /// </summary>
    public DbSet<TokenRecuperacion> TokensRecuperacion { get; set; }

    // ========================================
    // MÓDULO: CLIENTES Y EMPLEADOS (CRM)
    // ========================================

    /// <summary>
    /// Tabla de Categorías de Cliente (CRM - Segmentación)
    /// </summary>
    public DbSet<CategoriaCliente> CategoriasCliente { get; set; }

    /// <summary>
    /// Tabla de Clientes
    /// </summary>
    public DbSet<Cliente> Clientes { get; set; }

    /// <summary>
    /// Tabla de Preferencias de Cliente (CRM - Seguimiento)
    /// </summary>
    public DbSet<PreferenciaCliente> PreferenciasCliente { get; set; }

    /// <summary>
    /// Tabla de Empleados
    /// </summary>
    public DbSet<Empleado> Empleados { get; set; }

    // ========================================
    // MÓDULO: PROVEEDORES
    // ========================================

    /// <summary>
    /// Tabla de Proveedores (Hoteles, Aerolíneas, Transporte, Servicios)
    /// </summary>
    public DbSet<Proveedor> Proveedores { get; set; }

    /// <summary>
    /// Tabla de Contratos con Proveedores
    /// </summary>
    public DbSet<ContratoProveedor> ContratosProveedor { get; set; }

    // ========================================
    // MÓDULO: Servicios
    // ========================================

    /// <summary>
    /// Tabla de Aerolíneas
    /// </summary>
    public DbSet<Aerolinea> Aerolineas { get; set; }

    /// <summary>
    /// Tabla de Vuelos
    /// </summary>
    public DbSet<Vuelo> Vuelos { get; set; }

    // ========================================
    // CONFIGURACIÓN AVANZADA DE ENTIDADES
    // ========================================

    /// <summary>
    /// Configuración de las relaciones entre tablas y restricciones
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ====================================
        // CONFIGURACIÓN DE LA TABLA ROLES
        // ====================================
        modelBuilder.Entity<Rol>(entity =>
        {
            // Índice único en el nombre del rol
            entity.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("idx_rol_nombre_unique");

            // Índice en nivel de acceso para consultas rápidas
            entity.HasIndex(e => e.NivelAcceso)
                .HasDatabaseName("idx_rol_nivel_acceso");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA PERMISOS
        // ====================================
        modelBuilder.Entity<Permiso>(entity =>
        {
            // Índice único en el nombre del permiso
            entity.HasIndex(e => e.NombrePermiso)
                .IsUnique()
                .HasDatabaseName("idx_permiso_nombre_unique");

            // Índice compuesto en módulo y acción
            entity.HasIndex(e => new { e.Modulo, e.Accion })
                .HasDatabaseName("idx_permiso_modulo_accion");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA ROLES_PERMISOS
        // ====================================
        modelBuilder.Entity<RolPermiso>(entity =>
        {
            // Clave primaria compuesta
            entity.HasKey(rp => new { rp.IdRol, rp.IdPermiso })
                .HasName("pk_roles_permisos");

            // Relación con Rol (muchos a uno)
            entity.HasOne(rp => rp.Rol)
                .WithMany(r => r.RolesPermisos)
                .HasForeignKey(rp => rp.IdRol)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina un rol, se eliminan sus permisos
                .HasConstraintName("fk_rol_permiso_rol");

            // Relación con Permiso (muchos a uno)
            entity.HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolesPermisos)
                .HasForeignKey(rp => rp.IdPermiso)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina un permiso, se eliminan sus asignaciones
                .HasConstraintName("fk_rol_permiso_permiso");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA USUARIOS
        // ====================================
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Índice único en username
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("idx_usuario_username_unique");

            // Índice único en email
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("idx_usuario_email_unique");

            // Índice en tipo_usuario
            entity.HasIndex(e => e.TipoUsuario)
                .HasDatabaseName("idx_usuario_tipo");

            // Índice en estado
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_usuario_estado");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA USUARIOS_ROLES
        // ====================================
        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            // Clave primaria compuesta
            entity.HasKey(ur => new { ur.IdUsuario, ur.IdRol })
                .HasName("pk_usuarios_roles");

            // Relación con Usuario
            entity.HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuariosRoles)
                .HasForeignKey(ur => ur.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_usuario_rol_usuario");

            // Relación con Rol
            entity.HasOne(ur => ur.Rol)
                .WithMany() // No necesitamos navegación inversa en Rol por ahora
                .HasForeignKey(ur => ur.IdRol)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_usuario_rol_rol");

            // Índices
            entity.HasIndex(e => e.IdUsuario)
                .HasDatabaseName("idx_usuario_rol_usuario");

            entity.HasIndex(e => e.IdRol)
                .HasDatabaseName("idx_usuario_rol_rol");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA TOKENS_RECUPERACION
        // ====================================
        modelBuilder.Entity<TokenRecuperacion>(entity =>
        {
            // Índice único en token
            entity.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("idx_token_unique");

            // Índice en id_usuario
            entity.HasIndex(e => e.IdUsuario)
                .HasDatabaseName("idx_token_usuario");

            // Índice en fecha_expiracion (para limpiar tokens expirados)
            entity.HasIndex(e => e.FechaExpiracion)
                .HasDatabaseName("idx_token_expiracion");

            // Relación con Usuario
            entity.HasOne(t => t.Usuario)
                .WithMany(u => u.TokensRecuperacion)
                .HasForeignKey(t => t.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_token_usuario");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA CATEGORIAS_CLIENTE
        // ====================================
        modelBuilder.Entity<CategoriaCliente>(entity =>
        {
            // Índice único en el nombre de la categoría
            entity.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("idx_categoria_cliente_nombre_unique");

            // Índice en estado para consultas rápidas
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_categoria_cliente_estado");

            // Índice en descuento para ordenamientos
            entity.HasIndex(e => e.DescuentoPorcentaje)
                .HasDatabaseName("idx_categoria_cliente_descuento");

            // Relación con Clientes (uno a muchos)
            entity.HasMany(c => c.Clientes)
                .WithOne(cl => cl.Categoria)
                .HasForeignKey(cl => cl.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar categoría si tiene clientes
                .HasConstraintName("fk_cliente_categoria");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA CLIENTES
        // ====================================
        modelBuilder.Entity<Cliente>(entity =>
        {
            // Índice único en documento_identidad
            entity.HasIndex(e => e.DocumentoIdentidad)
                .IsUnique()
                .HasDatabaseName("idx_cliente_documento_unique");

            // Índice en correo_electronico para búsquedas
            entity.HasIndex(e => e.CorreoElectronico)
                .HasDatabaseName("idx_cliente_email");

            // Índice en estado para filtrados
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_cliente_estado");

            // Índice compuesto en apellido y nombre para ordenamientos
            entity.HasIndex(e => new { e.Apellido, e.Nombre })
                .HasDatabaseName("idx_cliente_apellido_nombre");

            // Índice en id_categoria para filtros CRM
            entity.HasIndex(e => e.IdCategoria)
                .HasDatabaseName("idx_cliente_categoria");

            // Índice en ciudad para filtros geográficos
            entity.HasIndex(e => e.Ciudad)
                .HasDatabaseName("idx_cliente_ciudad");

            // Relación con Usuario (1:1)
            entity.HasOne(c => c.Usuario)
                .WithMany() // Un usuario puede no tener navegación inversa aún
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_cliente_usuario");

            // Relación con CategoriaCliente (N:1) - ya configurada desde CategoriaCliente

            // Relación con PreferenciaCliente (1:1)
            entity.HasOne(c => c.Preferencia)
                .WithOne(p => p.Cliente)
                .HasForeignKey<PreferenciaCliente>(p => p.IdCliente) // ⭐ ESTO ES CLAVE
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina cliente, eliminar preferencias
                .HasConstraintName("fk_preferencia_cliente");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA PREFERENCIAS_CLIENTE
        // ====================================
        modelBuilder.Entity<PreferenciaCliente>(entity =>
        {
            // Índice en id_cliente para búsquedas rápidas
            entity.HasIndex(e => e.IdCliente)
                .HasDatabaseName("idx_preferencia_cliente");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA EMPLEADOS
        // ====================================
        modelBuilder.Entity<Empleado>(entity =>
        {
            // ========================================
            // ÍNDICES
            // ========================================

            // Índice único en documento_identidad
            entity.HasIndex(e => e.DocumentoIdentidad)
                .IsUnique()
                .HasDatabaseName("idx_empleado_documento_unique");

            // Índice en correo_electronico para búsquedas
            entity.HasIndex(e => e.CorreoElectronico)
                .HasDatabaseName("idx_empleado_email");

            // Índice en estado para filtrados
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_empleado_estado");

            // Índice compuesto en apellido y nombre para ordenamientos
            entity.HasIndex(e => new { e.Apellido, e.Nombre })
                .HasDatabaseName("idx_empleado_apellido_nombre");

            // Índice en cargo para filtros y estadísticas
            entity.HasIndex(e => e.Cargo)
                .HasDatabaseName("idx_empleado_cargo");

            // Índice en id_jefe para consultas de jerarquía
            entity.HasIndex(e => e.IdJefe)
                .HasDatabaseName("idx_empleado_jefe");

            // ========================================
            // RELACIONES
            // ========================================

            // Relación con Usuario (N:1)
            entity.HasOne(e => e.Usuario)
                .WithMany() // Un usuario puede no tener navegación inversa aún
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_empleado_usuario");

            // ========================================
            // AUTO-REFERENCIA (Jerarquía Organizacional)
            // ========================================

            // Relación con Jefe (N:1) - Un empleado puede tener un jefe
            entity.HasOne(e => e.Jefe)
                .WithMany(e => e.Subordinados) // Un jefe tiene muchos subordinados
                .HasForeignKey(e => e.IdJefe)
                .OnDelete(DeleteBehavior.Restrict) // NO eliminar en cascada para evitar eliminaciones accidentales
                .HasConstraintName("fk_empleado_jefe");

            // Nota importante sobre la auto-referencia:
            // - IdJefe nullable permite empleados sin jefe (CEO, Gerente General)
            // - DeleteBehavior.Restrict evita que al eliminar un jefe se eliminen sus subordinados
            // - En lugar de eso, se debe reasignar a los subordinados antes de eliminar al jefe
            // - La navegación inversa "Subordinados" se configura automáticamente
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA PROVEEDORES
        // ====================================
        modelBuilder.Entity<Proveedor>(entity =>
        {
            // Índice único en NIT/RUT
            entity.HasIndex(e => e.NitRut)
                .IsUnique()
                .HasDatabaseName("idx_proveedor_nit_unique");

            // Índice en nombre de empresa
            entity.HasIndex(e => e.NombreEmpresa)
                .HasDatabaseName("idx_proveedor_nombre");

            // Índice en tipo de proveedor
            entity.HasIndex(e => e.TipoProveedor)
                .HasDatabaseName("idx_proveedor_tipo");

            // Índice en estado
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_proveedor_estado");

            // Índice en calificación
            entity.HasIndex(e => e.Calificacion)
                .HasDatabaseName("idx_proveedor_calificacion");

            // Índice en ciudad
            entity.HasIndex(e => e.Ciudad)
                .HasDatabaseName("idx_proveedor_ciudad");

            // Relación con Contratos (1:N)
            entity.HasMany(p => p.Contratos)
                .WithOne(c => c.Proveedor)
                .HasForeignKey(c => c.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_contrato_proveedor");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA CONTRATOS_PROVEEDOR
        // ====================================
        modelBuilder.Entity<ContratoProveedor>(entity =>
        {
            // Índice único en numero_contrato
            entity.HasIndex(e => e.NumeroContrato)
                .IsUnique()
                .HasDatabaseName("idx_contrato_numero_unique");

            // Índice en id_proveedor para búsquedas por proveedor
            entity.HasIndex(e => e.IdProveedor)
                .HasDatabaseName("idx_contrato_proveedor");

            // Índice en estado para filtrados
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_contrato_estado");

            // Índice en fecha_fin para alertas de vencimiento
            entity.HasIndex(e => e.FechaFin)
                .HasDatabaseName("idx_contrato_fecha_fin");

            // Índice compuesto en estado y fecha_fin para contratos vigentes
            entity.HasIndex(e => new { e.Estado, e.FechaFin })
                .HasDatabaseName("idx_contrato_estado_fecha_fin");

            // Relación con Proveedor (N:1) - ya configurada desde Proveedor
        });

        // ============================================
        // CONFIGURACIÓN DE LA TABLA: AEROLÍNEAS
        // ============================================

        modelBuilder.Entity<Aerolinea>(entity =>
        {
            // Índice único en código IATA
            entity.HasIndex(e => e.CodigoIata)
                .IsUnique()
                .HasDatabaseName("IX_Aerolineas_CodigoIata");

            // Índice único en código ICAO
            entity.HasIndex(e => e.CodigoIcao)
                .IsUnique()
                .HasDatabaseName("IX_Aerolineas_CodigoIcao");

            // Índice en nombre para búsquedas
            entity.HasIndex(e => e.Nombre)
                .HasDatabaseName("IX_Aerolineas_Nombre");

            // Índice en país para filtrado
            entity.HasIndex(e => e.Pais)
                .HasDatabaseName("IX_Aerolineas_Pais");

            // Índice en estado
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("IX_Aerolineas_Estado");

            // Relación con Vuelos (1 aerolínea → muchos vuelos)
            entity.HasMany(a => a.Vuelos)
                .WithOne(v => v.Aerolinea)
                .HasForeignKey(v => v.IdAerolinea)
                .OnDelete(DeleteBehavior.Restrict);
        }); 
    }
}
