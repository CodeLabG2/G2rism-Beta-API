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
    /// Tabla de Tokens de Recuperación (LEGACY - Migrar a CodigosRecuperacion)
    /// </summary>
    public DbSet<TokenRecuperacion> TokensRecuperacion { get; set; }

    /// <summary>
    /// Tabla de Códigos de Recuperación (6 dígitos numéricos)
    /// Reemplaza a TokensRecuperacion con códigos cortos
    /// </summary>
    public DbSet<CodigoRecuperacion> CodigosRecuperacion { get; set; }

    /// <summary>
    /// Tabla de Refresh Tokens (JWT)
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

    /// <summary>
    /// Tabla de Hoteles
    /// </summary>
    public DbSet<Hotel> Hoteles { get; set; }

    /// <summary>
    /// Tabla de Servicios Adicionales
    /// </summary>
    public DbSet<ServicioAdicional> ServiciosAdicionales { get; set; }

    /// <summary>
    /// Tabla de Paquetes Turísticos
    /// </summary>
    public DbSet<PaqueteTuristico> PaquetesTuristicos { get; set; }

    // ========================================
    // MÓDULO: RESERVAS
    // ========================================

    /// <summary>
    /// Tabla principal de Reservas
    /// </summary>
    public DbSet<Reserva> Reservas { get; set; }

    /// <summary>
    /// Tabla intermedia Reservas-Hoteles (relación N:M)
    /// </summary>
    public DbSet<ReservaHotel> ReservasHoteles { get; set; }

    /// <summary>
    /// Tabla intermedia Reservas-Vuelos (relación N:M)
    /// </summary>
    public DbSet<ReservaVuelo> ReservasVuelos { get; set; }

    /// <summary>
    /// Tabla intermedia Reservas-Paquetes (relación N:M)
    /// </summary>
    public DbSet<ReservaPaquete> ReservasPaquetes { get; set; }

    // TODO: Agregar en futuras fases
    // public DbSet<ReservaServicio> ReservasServicios { get; set; }

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
        // CONFIGURACIÓN DE LA TABLA TOKENS_RECUPERACION (LEGACY)
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
        // CONFIGURACIÓN DE LA TABLA CODIGOS_RECUPERACION
        // ====================================
        modelBuilder.Entity<CodigoRecuperacion>(entity =>
        {
            // Índice único en código (no puede haber códigos duplicados activos)
            entity.HasIndex(e => e.Codigo)
                .IsUnique()
                .HasDatabaseName("idx_codigo_unique");

            // Índice en id_usuario para consultas rápidas
            entity.HasIndex(e => e.IdUsuario)
                .HasDatabaseName("idx_codigo_usuario");

            // Índice en fecha_expiracion para limpiar códigos expirados
            entity.HasIndex(e => e.FechaExpiracion)
                .HasDatabaseName("idx_codigo_expiracion");

            // Índice compuesto para códigos activos de un usuario
            entity.HasIndex(e => new { e.IdUsuario, e.Usado, e.Bloqueado, e.FechaExpiracion })
                .HasDatabaseName("idx_codigo_usuario_activo");

            // Relación con Usuario
            entity.HasOne(c => c.Usuario)
                .WithMany(u => u.CodigosRecuperacion)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_codigo_usuario");
        });

        // ====================================
        // CONFIGURACIÓN DE LA TABLA REFRESH_TOKENS
        // ====================================
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Índice único en token (debe ser único en el sistema)
            entity.HasIndex(e => e.Token)
                .IsUnique()
                .HasDatabaseName("idx_refresh_token_unique");

            // Índice en id_usuario para consultas rápidas de tokens por usuario
            entity.HasIndex(e => e.IdUsuario)
                .HasDatabaseName("idx_refresh_token_usuario");

            // Índice en fecha_expiracion para limpiar tokens expirados
            entity.HasIndex(e => e.FechaExpiracion)
                .HasDatabaseName("idx_refresh_token_expiracion");

            // Índice compuesto para buscar tokens activos de un usuario
            entity.HasIndex(e => new { e.IdUsuario, e.Revocado, e.FechaExpiracion })
                .HasDatabaseName("idx_refresh_token_usuario_activo");

            // Relación con Usuario
            entity.HasOne(rt => rt.Usuario)
                .WithMany() // No necesitamos navegación inversa en Usuario por ahora
                .HasForeignKey(rt => rt.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina usuario, eliminar sus tokens
                .HasConstraintName("fk_refresh_token_usuario");
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

        // ====================================
        // CONFIGURACIÓN DE LA TABLA VUELOS
        // ====================================
        modelBuilder.Entity<Vuelo>(entity =>
        {
            // Índice único en número de vuelo
            entity.HasIndex(e => e.NumeroVuelo)
                .IsUnique()
                .HasDatabaseName("idx_vuelo_numero_unique");

            // Índice compuesto para búsqueda origen-destino
            entity.HasIndex(e => new { e.Origen, e.Destino })
                .HasDatabaseName("idx_vuelo_origen_destino");

            // Índice en fecha_salida para búsquedas temporales
            entity.HasIndex(e => e.FechaSalida)
                .HasDatabaseName("idx_vuelo_fecha_salida");

            // Índice en estado
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_vuelo_estado");

            // Índice en id_aerolinea
            entity.HasIndex(e => e.IdAerolinea)
                .HasDatabaseName("idx_vuelo_aerolinea");

            // Índice en id_proveedor
            entity.HasIndex(e => e.IdProveedor)
                .HasDatabaseName("idx_vuelo_proveedor");

            // Relación con Aerolínea (N:1) - ya configurada desde Aerolinea

            // Relación con Proveedor (N:1)
            entity.HasOne(v => v.Proveedor)
                .WithMany() // No necesitamos navegación inversa en Proveedor por ahora
                .HasForeignKey(v => v.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar proveedor si tiene vuelos
                .HasConstraintName("fk_vuelo_proveedor");
        });

        // ========================================
        // CONFIGURACIÓN: HOTEL
        // ========================================
        modelBuilder.Entity<Hotel>(entity =>
        {
            // Índice compuesto único: nombre + ciudad (evita hoteles duplicados en misma ciudad)
            entity.HasIndex(e => new { e.Nombre, e.Ciudad })
                .IsUnique()
                .HasDatabaseName("idx_hotel_nombre_ciudad_unique");

            // Índice en ciudad para búsquedas por ubicación
            entity.HasIndex(e => e.Ciudad)
                .HasDatabaseName("idx_hotel_ciudad");

            // Índice en país para búsquedas por país
            entity.HasIndex(e => e.Pais)
                .HasDatabaseName("idx_hotel_pais");

            // Índice en estrellas para filtrado por clasificación
            entity.HasIndex(e => e.Estrellas)
                .HasDatabaseName("idx_hotel_estrellas");

            // Índice en categoría para filtrado
            entity.HasIndex(e => e.Categoria)
                .HasDatabaseName("idx_hotel_categoria");

            // Índice en estado
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_hotel_estado");

            // Índice en precio_por_noche para búsquedas por rango de precio
            entity.HasIndex(e => e.PrecioPorNoche)
                .HasDatabaseName("idx_hotel_precio");

            // Índice en id_proveedor
            entity.HasIndex(e => e.IdProveedor)
                .HasDatabaseName("idx_hotel_proveedor");

            // Índice compuesto para búsquedas con servicios (hoteles premium)
            entity.HasIndex(e => new { e.TienePiscina, e.TieneGimnasio, e.TieneRestaurante })
                .HasDatabaseName("idx_hotel_servicios_premium");

            // Relación con Proveedor (N:1)
            entity.HasOne(h => h.Proveedor)
                .WithMany() // No necesitamos navegación inversa en Proveedor
                .HasForeignKey(h => h.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar proveedor si tiene hoteles
                .HasConstraintName("fk_hotel_proveedor");
        });

        // ========================================
        // CONFIGURACIÓN: RESERVA
        // ========================================
        modelBuilder.Entity<Reserva>(entity =>
        {
            // Índice en id_cliente para consultas por cliente
            entity.HasIndex(e => e.IdCliente)
                .HasDatabaseName("idx_reserva_cliente");

            // Índice en id_empleado para consultas por empleado
            entity.HasIndex(e => e.IdEmpleado)
                .HasDatabaseName("idx_reserva_empleado");

            // Índice en estado para filtrado de reservas
            entity.HasIndex(e => e.Estado)
                .HasDatabaseName("idx_reserva_estado");

            // Índice en fecha_inicio_viaje para consultas temporales
            entity.HasIndex(e => e.FechaInicioViaje)
                .HasDatabaseName("idx_reserva_fecha_inicio");

            // Índice en fecha_fin_viaje
            entity.HasIndex(e => e.FechaFinViaje)
                .HasDatabaseName("idx_reserva_fecha_fin");

            // Índice compuesto para búsqueda de reservas activas por cliente
            entity.HasIndex(e => new { e.IdCliente, e.Estado, e.FechaInicioViaje })
                .HasDatabaseName("idx_reserva_cliente_estado_fecha");

            // Índice compuesto para búsqueda de reservas por rango de fechas
            entity.HasIndex(e => new { e.FechaInicioViaje, e.FechaFinViaje, e.Estado })
                .HasDatabaseName("idx_reserva_fechas_estado");

            // Índice en fecha_hora (creación) para auditoría
            entity.HasIndex(e => e.FechaHora)
                .HasDatabaseName("idx_reserva_fecha_creacion");

            // Relación con Cliente (N:1)
            entity.HasOne(r => r.Cliente)
                .WithMany() // No necesitamos navegación inversa en Cliente por ahora
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar cliente si tiene reservas
                .HasConstraintName("fk_reserva_cliente");

            // Relación con Empleado (N:1)
            entity.HasOne(r => r.Empleado)
                .WithMany() // No necesitamos navegación inversa en Empleado por ahora
                .HasForeignKey(r => r.IdEmpleado)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar empleado si tiene reservas asignadas
                .HasConstraintName("fk_reserva_empleado");
        });

        // ========================================
        // CONFIGURACIÓN: RESERVA_HOTEL
        // ========================================
        modelBuilder.Entity<ReservaHotel>(entity =>
        {
            // Índice en id_reserva para consultas rápidas
            entity.HasIndex(e => e.IdReserva)
                .HasDatabaseName("idx_reserva_hotel_reserva");

            // Índice en id_hotel para consultas rápidas
            entity.HasIndex(e => e.IdHotel)
                .HasDatabaseName("idx_reserva_hotel_hotel");

            // Índice en fecha_checkin para filtrado temporal
            entity.HasIndex(e => e.FechaCheckin)
                .HasDatabaseName("idx_reserva_hotel_checkin");

            // Índice compuesto para búsqueda de reservas de hotel por fechas
            entity.HasIndex(e => new { e.IdHotel, e.FechaCheckin, e.FechaCheckout })
                .HasDatabaseName("idx_reserva_hotel_fechas");

            // Relación con Reserva (N:1)
            entity.HasOne(rh => rh.Reserva)
                .WithMany(r => r.ReservasHoteles)
                .HasForeignKey(rh => rh.IdReserva)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina la reserva, eliminar sus hoteles
                .HasConstraintName("fk_reserva_hotel_reserva");

            // Relación con Hotel (N:1)
            entity.HasOne(rh => rh.Hotel)
                .WithMany() // No necesitamos navegación inversa en Hotel
                .HasForeignKey(rh => rh.IdHotel)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar hotel si tiene reservas
                .HasConstraintName("fk_reserva_hotel_hotel");
        });

        // ========================================
        // CONFIGURACIÓN: RESERVA_VUELO
        // ========================================
        modelBuilder.Entity<ReservaVuelo>(entity =>
        {
            // Índice en id_reserva para consultas rápidas
            entity.HasIndex(e => e.IdReserva)
                .HasDatabaseName("idx_reserva_vuelo_reserva");

            // Índice en id_vuelo para consultas rápidas
            entity.HasIndex(e => e.IdVuelo)
                .HasDatabaseName("idx_reserva_vuelo_vuelo");

            // Índice en clase para filtrado
            entity.HasIndex(e => e.Clase)
                .HasDatabaseName("idx_reserva_vuelo_clase");

            // Índice compuesto para búsqueda de reservas de vuelo
            entity.HasIndex(e => new { e.IdVuelo, e.IdReserva })
                .HasDatabaseName("idx_reserva_vuelo_vuelo_reserva");

            // Relación con Reserva (N:1)
            entity.HasOne(rv => rv.Reserva)
                .WithMany(r => r.ReservasVuelos)
                .HasForeignKey(rv => rv.IdReserva)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina la reserva, eliminar sus vuelos
                .HasConstraintName("fk_reserva_vuelo_reserva");

            // Relación con Vuelo (N:1)
            entity.HasOne(rv => rv.Vuelo)
                .WithMany() // No necesitamos navegación inversa en Vuelo
                .HasForeignKey(rv => rv.IdVuelo)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar vuelo si tiene reservas
                .HasConstraintName("fk_reserva_vuelo_vuelo");
        });

        // ========================================
        // CONFIGURACIÓN: RESERVA_PAQUETE
        // ========================================
        modelBuilder.Entity<ReservaPaquete>(entity =>
        {
            // Índice en id_reserva para consultas rápidas
            entity.HasIndex(e => e.IdReserva)
                .HasDatabaseName("idx_reserva_paquete_reserva");

            // Índice en id_paquete para consultas rápidas
            entity.HasIndex(e => e.IdPaquete)
                .HasDatabaseName("idx_reserva_paquete_paquete");

            // Índice en fecha_inicio_paquete para filtrado
            entity.HasIndex(e => e.FechaInicioPaquete)
                .HasDatabaseName("idx_reserva_paquete_fecha_inicio");

            // Índice compuesto para búsqueda de reservas de paquete
            entity.HasIndex(e => new { e.IdPaquete, e.IdReserva })
                .HasDatabaseName("idx_reserva_paquete_paquete_reserva");

            // Relación con Reserva (N:1)
            entity.HasOne(rp => rp.Reserva)
                .WithMany(r => r.ReservasPaquetes)
                .HasForeignKey(rp => rp.IdReserva)
                .OnDelete(DeleteBehavior.Cascade) // Si se elimina la reserva, eliminar sus paquetes
                .HasConstraintName("fk_reserva_paquete_reserva");

            // Relación con PaqueteTuristico (N:1)
            entity.HasOne(rp => rp.Paquete)
                .WithMany() // No necesitamos navegación inversa en PaqueteTuristico
                .HasForeignKey(rp => rp.IdPaquete)
                .OnDelete(DeleteBehavior.Restrict) // No eliminar paquete si tiene reservas
                .HasConstraintName("fk_reserva_paquete_paquete");
        });
    }
}
