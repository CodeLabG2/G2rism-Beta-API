# CLAUDE.md

Este archivo proporciona orientación a Claude Code (claude.ai/code) cuando trabaja con código en este repositorio.

## Descripción General del Proyecto

**G2rism Beta API** es una API Web de .NET 9.0 para un sistema integral de gestión turística (Sistema de Turismo) desarrollado por CodeLabG2. El sistema gestiona el ciclo de vida completo de las operaciones turísticas incluyendo autenticación de usuarios, CRM (clientes y empleados), proveedores y contratos, catálogo de servicios de viaje (aerolíneas, vuelos, hoteles, paquetes, servicios adicionales), gestión compleja de reservas y operaciones financieras (facturación y pagos).

**Estado Actual**: API MVP lista para producción con **19 controladores**, **29 modelos**, **145+ endpoints**, autenticación y autorización JWT completa, validación integral de lógica de negocio y características robustas de seguridad.

## Stack Tecnológico

### Framework Principal y Base de Datos
- **Framework**: .NET 9.0 (net9.0)
- **Base de Datos**: MySQL 9.0 vía Pomelo.EntityFrameworkCore.MySql 9.0.0
- **ORM**: Entity Framework Core 9.0.9

### Lógica de Negocio y Mapeo
- **Validación**: FluentValidation.AspNetCore 11.3.0
- **Mapeo**: AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1

### Seguridad y Autenticación
- **Hash de Contraseñas**: BCrypt.Net-Next 4.0.3 (workFactor 11)
- **Autenticación JWT**: Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0
- **Tokens JWT**: System.IdentityModel.Tokens.Jwt 8.0.1
- **Limitación de Velocidad**: Limitación de velocidad integrada en ASP.NET Core

### Comunicación y Documentación
- **Servicio de Email**: SendGrid 9.29.3 (listo para producción)
- **Documentación API**: Swashbuckle.AspNetCore 9.0.6 (Swagger/OpenAPI)

## Comandos Comunes

### Compilar y Ejecutar
```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación (modo desarrollo con Swagger en http://localhost:5026/)
dotnet run

# Ejecutar con watch (recarga automática en cambios)
dotnet watch run
```

### Migraciones de Base de Datos
```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones a la base de datos
dotnet ef database update

# Retroceder a una migración específica
dotnet ef database update NombreMigracion

# Eliminar última migración (si no está aplicada)
dotnet ef migrations remove

# Listar todas las migraciones
dotnet ef migrations list
```

### Pruebas
```bash
# Ejecutar todas las pruebas (si existe proyecto de pruebas)
dotnet test

# Ejecutar pruebas con salida detallada
dotnet test --logger "console;verbosity=detailed"
```

## Arquitectura

### Patrón de Arquitectura en Capas

El proyecto sigue una arquitectura en capas limpia con clara separación de responsabilidades:

1. **Modelos** (`Models/`) - Entidades de dominio con configuración de EF Core **(29 entidades)** ⬆️
2. **DTOs** (`DTOs/`) - Objetos de Transferencia de Datos organizados por subdirectorios de módulos **(82 DTOs en 26 módulos)** ⬆️
3. **Interfaces** (`Interfaces/`) - Contratos de abstracción para repositorios y servicios **(49 interfaces)** ⬆️
4. **Repositorios** (`Repositories/`) - Capa de acceso a datos implementando patrón repositorio **(28 repositorios)** ⬆️
5. **Servicios** (`Services/`) - Capa de lógica de negocio con validación **(24 servicios)** ⬆️
6. **Controladores** (`Controllers/`) - Endpoints de API siguiendo convenciones REST **(19 controladores)** ⬆️
7. **Validadores** (`Validators/`) - Reglas de FluentValidation para DTOs **(49 validadores)** ⬆️
8. **Middleware** (`Middleware/`) - Manejador global de excepciones con stack traces formateados
9. **Helpers** (`Helpers/`) - Utilidades (JwtTokenGenerator, PasswordHasher, EmailHelper, TokenGenerator)
10. **Mappings** (`Mappings/`) - Perfil de AutoMapper para conversiones Model ↔ DTO (MappingProfile.cs)
11. **Data** (`Data/`) - DbContext, DbInitializer (seeding), DbContextFactory
12. **Constants** (`Constants/`) - RoleConstants con roles predefinidos y métodos auxiliares
13. **Authorization** (`Authorization/`) - Manejadores de autorización basada en permisos ⭐ NUEVO
14. **Configuration** (`Configuration/`) - Configuración de seguridad y aplicación ⭐ NUEVO

### Flujo de Inyección de Dependencias

**Program.cs** registra servicios en este orden:

1. **Contexto de Base de Datos** - Conexión MySQL con detección automática de versión del servidor
2. **Configuración de Seguridad** - Configuración JWT, SecuritySettings desde appsettings
3. **AutoMapper** - Escanea todos los ensamblados en busca de perfiles de mapeo
4. **Repositorio Genérico** - `IGenericRepository<T>` → `GenericRepository<T>`
5. **Repositorios de Entidades** - 27 repositorios específicos de módulos
6. **Servicios** - 24 servicios de lógica de negocio
7. **Servicio de Email** - SendGridEmailService (listo para producción)
8. **FluentValidation** - Auto-registro vía `AddValidatorsFromAssembly()`
9. **Autenticación JWT** - Configuración de token Bearer con clock skew cero
10. **Políticas de Autorización** - 40+ políticas basadas en permisos y roles
11. **Limitación de Velocidad** - 5 políticas (auth, password-recovery, refresh, api, global)
12. **Controladores** - Con comentarios XML habilitados
13. **Swagger/OpenAPI** - Soporte de autenticación JWT
14. **Política CORS** - "AllowAll" para desarrollo (restringir en producción)

### Módulos y Características

El sistema está organizado en **8 módulos distintos** con funcionalidad integral:

#### 1. Módulo de Configuración (Roles y Permisos)
- **Modelos**: `Rol`, `Permiso`, `RolPermiso` (muchos-a-muchos)
- **Controladores**: `RolesController`, `PermisosController`
- **Endpoints**: 14 endpoints en total
  - Roles: CRUD + obtener roles con permisos + asignar/remover permisos
  - Permisos: CRUD + obtener permisos por módulo
- **Características**:
  - Niveles de acceso jerárquicos (NivelAcceso: 1=SuperAdmin, 2=Admin, 10=Empleado, 50=Cliente)
  - Asignación de permisos con **estrategia acumulativa** (agrega permisos en lugar de reemplazar)
  - Gestión de roles con control de estado
  - Sistema de autorización basado en permisos
  - Propiedad calculada: `CantidadPermisos` en RolResponseDto

#### 2. Módulo de Autenticación de Usuarios ⭐ MEJORADO
- **Modelos**: `Usuario`, `UsuarioRol` (muchos-a-muchos), `TokenRecuperacion` (LEGACY), `CodigoRecuperacion` ⭐ NUEVO, `RefreshToken` ⭐ NUEVO
- **Controladores**: `AuthController`, `UsuariosController`
- **Endpoints**: 18 endpoints en total
  - Auth: Registrar, Login, Logout, Refresh token ⭐, Recuperación de contraseña (código de 6 dígitos) ⭐, Reset, Cambiar contraseña
  - Usuarios: CRUD + obtener con roles + bloquear/desbloquear + activar/desactivar + asignar/remover roles
- **Características**:
  - **Autenticación JWT** ⭐ NUEVO:
    - Access tokens (60 min de expiración)
    - Refresh tokens (7 días de expiración) con rotación
    - Soporte de revocación de tokens
    - Seguimiento de IP y UserAgent para auditoría
  - **Recuperación de Contraseña** ⭐ ACTUALIZADO:
    - Códigos de recuperación de 6 dígitos (reemplaza tokens largos)
    - Expiración de 1 hora
    - Protección contra fuerza bruta (5 intentos máx.)
    - Integración de email SendGrid
  - Hash de contraseñas BCrypt (workFactor: 11)
  - Validación de fortaleza de contraseña (mayúscula, minúscula, número, carácter especial)
  - Seguimiento de intentos de login y bloqueo automático de cuenta
  - Segregación de tipos de usuario (cliente vs empleado)
  - **REGLA DE NEGOCIO CRÍTICA**: Solo UN Super Administrador permitido en el sistema
  - **REGLA DE NEGOCIO CRÍTICA**: Asignación de roles validada contra tipo de usuario
  - Eliminación suave con campo Estado

#### 3. Módulo CRM - Clientes
- **Modelos**: `CategoriaCliente`, `Cliente`, `PreferenciaCliente`
- **Controladores**: `CategoriasClienteController`, `ClientesController`, `PreferenciasClienteController`
- **Endpoints**: 15 endpoints en total
  - Categorías: CRUD + obtener con conteo de clientes
  - Clientes: CRUD + obtener con detalles de categoría + filtrar por categoría/ciudad
  - Preferencias: CRUD (sin DELETE) + obtener por cliente
- **Características**:
  - Segmentación de clientes con categorías de descuento (basadas en porcentaje)
  - Relación 1:1 entre Cliente y PreferenciaCliente (eliminación en cascada)
  - Relación N:1 entre Cliente y CategoriaCliente (restringir eliminación)
  - Cliente vinculado a Usuario (1:1, restringir eliminación)
  - Propiedades calculadas en Cliente: `Edad` (calculada desde FechaNacimiento), `NombreCompleto`
  - Restricción única en DocumentoIdentidad
  - Seguimiento de preferencias: tipo de alojamiento, tipo de destino, actividades, presupuesto, requisitos especiales

#### 4. Módulo CRM - Empleados
- **Modelos**: `Empleado`
- **Controladores**: `EmpleadosController`
- **Endpoints**: 8 endpoints en total
  - CRUD + obtener con info de jefe + obtener subordinados + obtener por departamento
- **Características**:
  - Jerarquía de empleados (auto-referencia con `IdJefe`)
  - Navegación: `Empleado.Jefe` (jefe) y `Empleado.Subordinados` (lista de subordinados)
  - DeleteBehavior.Restrict en auto-referencia (previene eliminaciones en cascada)
  - Empleado vinculado a Usuario (N:1, restringir eliminación)
  - Propiedades calculadas: `NombreCompleto`, `Edad`, `AntiguedadAnios`, `AntiguedadMeses`, `EsJefe`, `CantidadSubordinados`
  - EmpleadoResponseDto incluye `JefeBasicInfoDto` anidado con detalles del jefe
  - Restricción única en DocumentoIdentidad
  - Campo de salario (decimal 10,2) - visibilidad controlada por autorización

#### 5. Módulo de Proveedores
- **Modelos**: `Proveedor`, `ContratoProveedor`
- **Controladores**: `ProveedoresController`, `ContratosProveedorController`
- **Endpoints**: 16 endpoints en total
  - Proveedores: CRUD + obtener por tipo + obtener activos + obtener por calificación
  - Contratos: CRUD + obtener por proveedor + obtener próximos a vencer + obtener activos
- **Características**:
  - Tipos de proveedor: 'hotel', 'aerolinea', 'transporte', 'servicio'
  - Relaciones 1:N (Proveedor → ContratoProveedor, Hotel, Vuelo, ServicioAdicional, todos restringen eliminación)
  - Gestión de contratos con seguimiento de expiración
  - Sistema de calificación de proveedores (escala 1-5, nullable)
  - Propiedades calculadas en ContratoProveedor: `EstaVigente`, `DiasRestantes`, `ProximoAVencer`, `DuracionDias`
  - Restricciones únicas: NitRut (proveedor), NumeroContrato (contrato)
  - Seguimiento de estado para proveedores y contratos

#### 6. Módulo de Servicios de Viaje ⭐ EXPANSIÓN MASIVA
- **Modelos**: `Aerolinea`, `Vuelo` ⭐ COMPLETADO, `Hotel` ⭐ NUEVO, `ServicioAdicional` ⭐ NUEVO, `PaqueteTuristico` ⭐ NUEVO
- **Controladores**: `AerolineasController`, `VuelosController` ⭐, `HotelesController` ⭐, `ServiciosAdicionalesController` ⭐, `PaquetesTuristicosController` ⭐
- **Endpoints**: 35 endpoints en total (28 NUEVOS!)

  **Aerolíneas** (7 endpoints):
  - CRUD + obtener por país + obtener activas + buscar por código
  - Validación de código IATA (2 caracteres mayúsculas)
  - Validación de código ICAO (3 caracteres mayúsculas)
  - Restricciones únicas en ambos códigos
  - Gestión de políticas de equipaje
  - Relación 1:N (Aerolinea → Vuelo, restringir eliminación)
  - Propiedades calculadas: `EstaActiva`, `NombreCompleto`, `TienePoliticasEquipaje`

  **Vuelos** ⭐ NUEVO (7 endpoints):
  - CRUD + obtener por aerolínea + obtener disponibles + buscar por ruta
  - Seguimiento de disponibilidad de vuelos (cupos_disponibles)
  - Precios con múltiples clases (economica, ejecutiva, primera_clase)
  - Cálculo de duración (duracion_horas)
  - Seguimiento de vuelos directos/con conexión (escala_info, ciudad_escala)
  - Gestión de estado (programado, cancelado, retrasado, completado)
  - Propiedades calculadas: `TieneDisponibilidad`, `EsVueloDirecto`, `EstaActivo`

  **Hoteles** ⭐ NUEVO (7 endpoints):
  - CRUD + obtener por ciudad/país + obtener por calificación
  - Sistema de calificación por estrellas (1-5 estrellas)
  - Gestión de habitaciones (habitaciones_disponibles, tipos_habitacion)
  - Campos JSON:
    - `Fotos` (array de URLs de imágenes)
    - `ServiciosIncluidos` (array de amenidades: wifi, desayuno, piscina, etc.)
  - Precios (precio_por_noche_desde)
  - Datos de ubicación (ciudad, pais, direccion, latitud, longitud)
  - Políticas (politica_cancelacion, horario_checkin, horario_checkout)
  - Propiedades calculadas: `NombreCompleto`, `TieneServiciosPremium`, `ClasificacionTexto`

  **Servicios Adicionales** ⭐ NUEVO (7 endpoints):
  - CRUD + obtener por proveedor + obtener por tipo + obtener disponibles
  - Tipos de servicio: 'tour', 'guia', 'actividad', 'transporte_interno'
  - Seguimiento de duración (duracion_horas)
  - Gestión de capacidad (capacidad_maxima)
  - Campo JSON: `IdiomasDisponibles` (array: español, inglés, francés, etc.)
  - Estado de disponibilidad
  - Propiedades calculadas: `EstaDisponible`, `TieneCapacidad`

  **Paquetes Turísticos** ⭐ NUEVO (7 endpoints):
  - CRUD + obtener por tipo + obtener por destino + obtener disponibles
  - Tipos de paquete: 'vacacional', 'aventura', 'cultural', 'negocios', 'romantico'
  - Seguimiento de itinerario (duracion_dias, duracion_noches)
  - Gestión de capacidad (cupos_disponibles)
  - Campos JSON:
    - `DestinosAdicionales` (array de destinos)
    - `Incluye` (array: alojamiento, transporte, comidas, tours, seguros)
    - `Imagenes` (array de URLs de imágenes)
  - Seguimiento de requisitos (requisitos, edad_minima, nivel_dificultad)
  - Gestión de temporadas (temporada_alta, temporada_baja)
  - Propiedades calculadas: `TieneDisponibilidad`, `EsPaqueteCompleto`

#### 7. Módulo de Reservas ⭐ NUEVO Y COMPLEJO
- **Modelos**: `Reserva`, `ReservaHotel`, `ReservaVuelo`, `ReservaPaquete`, `ReservaServicio`
- **Controladores**: `ReservasController`
- **Endpoints**: 15+ endpoints en total

  **Características Clave**:
  - **Reservas multi-servicio**: Una sola reserva puede incluir hoteles, vuelos, paquetes y servicios adicionales
  - **Cálculos financieros automáticos**:
    - `MontoTotal` = Suma de todos los subtotales de servicios
    - `SaldoPendiente` = MontoTotal - MontoPagado
    - Propiedades calculadas: `EstaPagada`, `PorcentajePagado`, `TieneSaldoPendiente`, `DiasHastaViaje`
  - **Endpoint de creación compleja**: POST `/api/reservas/completa` crea reserva con todos los servicios en una transacción
  - **Gestión de servicios**:
    - Agregar/remover hoteles, vuelos, paquetes, servicios después de la creación
    - Cada servicio mantiene su propio subtotal
    - Recálculo automático de totales de reserva
  - **Validación de disponibilidad**:
    - Vuelos: Verificar y reducir cupos_disponibles
    - Hoteles: Verificar disponibilidad de habitaciones
    - Paquetes: Verificar y reducir cupos_disponibles
    - Servicios: Verificar bandera de disponibilidad
  - **Validación de fechas**:
    - FechaInicioViaje <= FechaFinViaje
    - Todas las fechas de servicio dentro de las fechas de reserva
  - **Gestión de estado**: pendiente, confirmada, cancelada, completada

  **Tablas de Unión** (Muchos-a-Muchos con datos adicionales):
  - `ReservaHotel`: habitacion_tipo, habitaciones_cantidad, subtotal_hotel
  - `ReservaVuelo`: clase_vuelo, pasajeros_cantidad, subtotal_vuelo
  - `ReservaPaquete`: personas_cantidad, subtotal_paquete, personalizaciones (JSON)
  - `ReservaServicio`: participantes_cantidad, fecha_servicio, subtotal_servicio

#### 8. Módulo Financiero ⭐ NUEVO Y LISTO PARA PRODUCCIÓN
- **Modelos**: `FormaDePago`, `Factura`, `Pago`
- **Controladores**: `FormasDePagoController`, `FacturasController`, `PagosController`
- **Endpoints**: 19 endpoints en total

  **Formas de Pago** (5 endpoints):
  - CRUD + obtener activas
  - Métodos predefinidos: Efectivo, Tarjeta Crédito, Tarjeta Débito, Transferencia, PSE, Nequi, Daviplata
  - Inicializados en la base de datos

  **Facturas** ⭐ (7 endpoints):
  - CRUD + obtener por reserva + obtener por estado + obtener vencidas
  - **Numeración automática de facturas**: FAC-{año}-{consecutivo} (ej., FAC-2025-00001)
  - **Cálculos de impuestos**:
    - BaseGravable = Subtotal - Descuentos
    - Impuestos = BaseGravable * (PorcentajeIva / 100)
    - Total = BaseGravable + Impuestos
  - **Campos de cumplimiento DIAN** (placeholders para MVP):
    - ResolucionDian
    - CufeCude
  - **Gestión de estado**: pendiente, pagada, cancelada, vencida
  - **Propiedades calculadas**:
    - `EstaVencida` (verifica FechaVencimiento)
    - `MontoPagado` (suma de pagos aprobados)
    - `SaldoPendiente`
    - `PorcentajePagado`
  - Relación 1:1 con Reserva (restringir eliminación)

  **Pagos** ⭐ (7 endpoints):
  - CRUD + obtener por factura + obtener por estado + obtener por forma de pago
  - **Pagos parciales** soportados (múltiples pagos por factura)
  - **Procesamiento de pagos**:
    - Estado: pendiente, aprobado, rechazado
    - Actualización automática del estado de factura cuando está completamente pagada
    - Validación: monto de pago no puede exceder saldo de factura
  - **Rastro de auditoría**:
    - ReferenciaTransaccion (autorización bancaria, número de recibo)
    - ComprobantePago (URL o base64)
    - Timestamp FechaPago
  - Relaciones N:1 (Factura → Pago cascada, FormaDePago → Pago restringir)

### Patrones de Diseño de Base de Datos

#### 1. Relaciones Muchos-a-Muchos
Tablas de unión explícitas con claves compuestas y datos de negocio adicionales:

- **Configuración**:
  - `RolPermiso` (IdRol + IdPermiso) - Incluye FechaAsignacion, AsignadoPor
  - `UsuarioRol` (IdUsuario + IdRol) - Incluye FechaAsignacion, AsignadoPor

- **Reservas** ⭐ NUEVO:
  - `ReservaHotel` (IdReserva + IdHotel) - Incluye detalles de habitación, fechas check-in/out, subtotal
  - `ReservaVuelo` (IdReserva + IdVuelo) - Incluye clase de vuelo, cantidad de pasajeros, subtotal
  - `ReservaPaquete` (IdReserva + IdPaquete) - Incluye cantidad de personas, subtotal, personalizaciones (JSON)
  - `ReservaServicio` (IdReserva + IdServicio) - Incluye cantidad de participantes, fecha de servicio, subtotal

#### 2. Relaciones Uno-a-Uno
- `Cliente` ↔ `PreferenciaCliente` (eliminación en cascada)
- `Reserva` ↔ `Factura` (restringir eliminación)

#### 3. Relaciones Uno-a-Muchos
- `CategoriaCliente` → `Cliente` (restringir eliminación)
- `Proveedor` → `ContratoProveedor`, `Hotel`, `Vuelo`, `ServicioAdicional` (todos restringen eliminación)
- `Aerolinea` → `Vuelo` (restringir eliminación)
- `Usuario` → `Cliente`, `Empleado` (ambos restringen eliminación)
- `Usuario` → `RefreshToken`, `CodigoRecuperacion` (ambos eliminación en cascada)
- `Cliente` → `Reserva` (restringir eliminación)
- `Empleado` → `Reserva` (restringir eliminación) - quien creó la reserva
- `Factura` → `Pago` (eliminación en cascada)
- `FormaDePago` → `Pago` (restringir eliminación)

#### 4. Relaciones Auto-Referenciadas
- `Empleado.IdJefe` → `Empleado` (estructura jerárquica, restringir eliminación)
- IdJefe nullable permite empleados de nivel superior (CEO, directores)

#### 5. Eliminación Suave
La mayoría de entidades usan campo `Estado` booleano en lugar de eliminaciones físicas:
- Usuario, Rol, Permiso, CategoriaCliente, Cliente, Empleado, Proveedor, ContratoProveedor, Aerolinea, Vuelo, Hotel, ServicioAdicional, PaqueteTuristico, Reserva, FormaDePago

#### 6. Campos de Auditoría
- **Estándar**: `FechaCreacion`, `FechaModificacion`
- **Especial**: `FechaAsignacion` (tablas de unión), `FechaRegistro` (Cliente, Proveedor, Reserva), `FechaActualizacion` (PreferenciaCliente)
- **Financiero**: `FechaEmision`, `FechaVencimiento` (Factura), `FechaPago` (Pago)

#### 7. Restricciones Únicas
Aplicadas vía índices únicos:
- Usuario: Username, Email
- Cliente: DocumentoIdentidad
- Empleado: DocumentoIdentidad
- Proveedor: NitRut
- ContratoProveedor: NumeroContrato
- Aerolinea: CodigoIata, CodigoIcao
- Rol: Nombre
- Permiso: NombrePermiso
- Factura: NumeroFactura
- RefreshToken: Token

#### 8. Comportamiento en Cascada
- **Restrict**: Usado para relaciones críticas (previene eliminaciones accidentales)
  - CategoriaCliente → Cliente
  - Proveedor → Hotel/Vuelo/Servicio/Contrato
  - Usuario → Cliente/Empleado
  - Reserva → Factura
  - FormaDePago → Pago

- **Cascade**: Usado para datos dependientes (eliminados automáticamente con el padre)
  - Rol → RolPermiso
  - Usuario → UsuarioRol, RefreshToken, CodigoRecuperacion
  - Cliente → PreferenciaCliente
  - Reserva → ReservaHotel/ReservaVuelo/ReservaPaquete/ReservaServicio
  - Factura → Pago

#### 9. Índices
Todos tienen nombres personalizados para claridad:
- **Índices únicos** en todas las restricciones únicas
- **Índices de rendimiento** en claves foráneas
- **Índices compuestos** en patrones de consulta comunes:
  - Permiso: Modulo+Accion
  - Cliente: Apellido+Nombre
  - Vuelo: Origen+Destino+FechaSalida
  - Hotel: Ciudad+Estrellas
  - Reserva: IdCliente+EstadoReserva
- **Índices de estado** para filtrado (Estado, Bloqueado, EstadoReserva)

#### 10. Columnas JSON
Usadas para estructuras de datos flexibles para evitar tablas de unión excesivas:
- `Hotel.Fotos`, `Hotel.ServiciosIncluidos`
- `PaqueteTuristico.DestinosAdicionales`, `Incluye`, `Imagenes`
- `ServicioAdicional.IdiomasDisponibles`
- `ReservaPaquete.Personalizaciones`

### Patrones y Convenciones Clave

#### 1. Patrón Repositorio con Base Genérica
- **Repositorio Genérico** (`IGenericRepository<T>`, `GenericRepository<T>`):
  - Proporciona CRUD estándar: GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync, SaveChangesAsync
  - Usa EF Core DbContext
  - Patrón async/await en todo

- **Repositorios específicos de entidad** extienden con consultas personalizadas:
  - Ejemplo: `IRolRepository : IGenericRepository<Rol>`
  - Métodos personalizados: GetRolConPermisosAsync, ExistsByNombreAsync, etc.
  - Usan Include() para carga ansiosa de entidades relacionadas

#### 2. Patrón de Capa de Servicio
- **Los servicios contienen lógica de negocio** y orquestan repositorios
- **Los servicios usan AutoMapper** para convertir Models ↔ DTOs
- **Los servicios validan reglas de negocio** antes de llamadas a repositorio
- **Los servicios lanzan excepciones** para fallas de validación:
  - `ArgumentException` para entrada inválida (400)
  - `KeyNotFoundException` para no encontrado (404)
  - `InvalidOperationException` para violaciones de reglas de negocio (400)
  - `UnauthorizedAccessException` para problemas de permisos (401)

- **Servicios Complejos**:
  - **ReservaService**: Cálculos financieros automáticos, validación de disponibilidad, transacciones atómicas multi-servicio
  - **FacturaService**: Numeración automática de facturas, cálculos de impuestos, cumplimiento DIAN
  - **PagoService**: Soporte de pagos parciales, actualizaciones de estado de factura, validación de saldo
  - **AuthService**: Generación de tokens JWT, rotación de refresh token, códigos de recuperación de 6 dígitos

#### 3. Patrón DTO
Organizados en subdirectorios por módulo (26 módulos, 82 DTOs):

- **CreateDto**: Para crear nuevas entidades
  - Excluye: ID, FechaCreacion, FechaModificacion, propiedades de navegación
  - Incluye: Todos los campos requeridos + Password (para Usuario)
  - Ejemplo: `ReservaCompletaCreateDto` incluye arrays de hoteles, vuelos, paquetes, servicios

- **UpdateDto**: Para actualizaciones (actualizaciones parciales soportadas)
  - **Todos los campos son nullable** para actualizaciones opcionales
  - AutoMapper configurado con `.Condition()` para ignorar valores nulos
  - Excluye: ID, campos de auditoría, propiedades de navegación
  - Ejemplo: `HotelUpdateDto` tiene todas las propiedades nullable

- **ResponseDto**: Para respuestas de API
  - Excluye: Datos sensibles (PasswordHash), colecciones de navegación
  - Incluye: Campos calculados de modelos
  - Ejemplo: `ReservaResponseDto` incluye campos financieros calculados

- **DTOs Especiales**:
  - `XxxConYyyDto`: Para respuestas con datos relacionados anidados
    - Ejemplo: `RolConPermisosDto`, `UsuarioConRolesDto`
  - `AsignarXxxDto`: Para operaciones de asignación
    - Ejemplo: `AsignarPermisoDto`, `AsignarRolesMultiplesDto`
  - `XxxCompletaDto`: Para operaciones de creación complejas
    - Ejemplo: `ReservaCompletaCreateDto` (crea reserva con todos los servicios en una llamada)

#### 4. Estrategia de Validación

**Validación de dos capas**:

1. **FluentValidation** (validación estructural/de formato):
   - Validadores en directorio `Validators/` (49 validadores)
   - Nombrados `{DtoName}Validator.cs`
   - Auto-registrados vía `AddValidatorsFromAssembly()`
   - Se ejecutan antes de la ejecución del método del controlador
   - Validaciones de ejemplo:
     - Longitud de cadena, formato (regex)
     - Formato de email
     - Campos requeridos
     - Fortaleza de contraseña (usando helper PasswordHasher)
     - Restricciones de lista (sin duplicados, conteo min/max)
     - Validación entre campos (Password == ConfirmPassword, FechaInicio <= FechaFin)
     - Formato de código IATA/ICAO (mayúsculas, longitudes específicas)
     - Rangos de calificación (1-5), rangos de porcentaje (0-100)

2. **Capa de Servicio** (validación de lógica de negocio):
   - Verificaciones dependientes de base de datos (unicidad, existencia)
   - Reglas de negocio complejas (unicidad de Super Admin, compatibilidad de roles)
   - Validación de estado (no se puede eliminar categoría con clientes activos)
   - Integridad de relaciones
   - **Validación de disponibilidad** (vuelos, hoteles, paquetes)
   - **Validación financiera** (montos de pago, saldos de factura)

**Ejemplo**: ReservaCompletaCreateDtoValidator
- FluentValidation: Rangos de fechas, restricciones de arrays, validación de ID
- Servicio: Existencia de servicio, disponibilidad, conflictos de fechas, cálculos financieros

#### 5. Estructura de Respuesta de API

**Formato de respuesta consistente**:

- **Respuesta Exitosa** (`ApiResponse<T>`):
  ```csharp
  {
    "success": true,
    "message": "Operación completada exitosamente",
    "data": { /* T */ },
    "timestamp": "2025-12-12T10:30:00"
  }
  ```

- **Respuesta de Error** (`ApiErrorResponse`):
  ```csharp
  {
    "success": false,
    "message": "Descripción del error",
    "statusCode": 400,
    "errorCode": "InvalidOperationException",
    "errors": null,  // Errores de validación opcionales
    "stackTrace": "...",  // Solo en Desarrollo
    "timestamp": "2025-12-12T10:30:00"
  }
  ```

- **Manejo Global de Excepciones** (`GlobalExceptionHandlerMiddleware`):
  - Captura todas las excepciones no manejadas
  - Mapea tipos de excepción a códigos de estado HTTP
  - Formatea stack traces (resalta código de usuario vs framework)
  - Registra errores con logging estructurado
  - Retorna ApiErrorResponse consistente

#### 6. Propiedades Calculadas en Modelos

Usando `[NotMapped]` para campos calculados (previene mapeo de base de datos):

- **Cliente**: `Edad`, `NombreCompleto`
- **Empleado**: `NombreCompleto`, `Edad`, `AntiguedadAnios`, `AntiguedadMeses`, `EsJefe`, `CantidadSubordinados`
- **ContratoProveedor**: `EstaVigente`, `DiasRestantes`, `ProximoAVencer`, `DuracionDias`
- **Aerolinea**: `EstaActiva`, `NombreCompleto`, `TienePoliticasEquipaje`
- **Vuelo** ⭐: `TieneDisponibilidad`, `EsVueloDirecto`, `EstaActivo`
- **Hotel** ⭐: `NombreCompleto`, `TieneServiciosPremium`, `ClasificacionTexto`
- **ServicioAdicional** ⭐: `EstaDisponible`, `TieneCapacidad`
- **PaqueteTuristico** ⭐: `TieneDisponibilidad`, `EsPaqueteCompleto`
- **Reserva** ⭐: `DuracionDias`, `EstaPagada`, `PorcentajePagado`, `TieneSaldoPendiente`, `DiasHastaViaje`
- **Factura** ⭐: `SaldoPendiente`, `EstaVencida`, `PorcentajePagado`

**Beneficios**:
- Modelos de dominio ricos con lógica de negocio
- Calculadas una vez cuando la entidad se carga
- Incluidas automáticamente en ResponseDtos vía AutoMapper
- Sin sobrecarga de almacenamiento en base de datos

### Configuración de AutoMapper

**MappingProfile.cs** contiene todos los mapeos (29 entidades):

- **CreateDto → Model**:
  - Ignora: ID, FechaCreacion, FechaModificacion, propiedades de navegación
  - Mapea: Todos los campos requeridos
  - Manejo especial: Password excluido (hasheado en servicio)

- **UpdateDto → Model**:
  - Ignora: ID, campos de auditoría, propiedades de navegación
  - **Mapeo condicional**: `.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null))`
  - Habilita actualizaciones parciales (CRÍTICO para endpoints PUT)

- **Model → ResponseDto**:
  - Incluye: Propiedades calculadas del modelo
  - Mapea: Nombres de entidades relacionadas (no objetos completos para evitar referencias circulares)
  - Ejemplo: `ReservaResponseDto.NombreCliente` desde `Reserva.Cliente.NombreCompleto`

- **Mapeos especiales**:
  - `Usuario → UsuarioLoginDto`: Aplana roles y permisos
  - `Rol → RolConPermisosDto`: Incluye lista completa de Permiso vía RolesPermisos
  - `Empleado → EmpleadoResponseDto`: Incluye JefeBasicInfoDto anidado

### Constantes y Helpers

#### Constants/RoleConstants.cs
Gestión centralizada de roles:

```csharp
// IDs de Roles (coinciden con seeding)
SUPER_ADMINISTRADOR_ID = 1
ADMINISTRADOR_ID = 2
EMPLEADO_ID = 3
CLIENTE_ID = 4

// Nombres de Roles
SUPER_ADMINISTRADOR = "Super Administrador"
ADMINISTRADOR = "Administrador"
EMPLEADO = "Empleado"
CLIENTE = "Cliente"

// Tipos de Usuario
TIPO_EMPLEADO = "empleado"
TIPO_CLIENTE = "cliente"

// Niveles de Acceso
NIVEL_SUPER_ADMIN = 1
NIVEL_ADMIN = 2
NIVEL_EMPLEADO = 10
NIVEL_CLIENTE = 50

// Métodos Auxiliares
EsRolAdministrativo(int idRol)
EsSuperAdministrador(int idRol)
GetRolesPermitidos(string tipoUsuario)
EsRolValidoParaTipoUsuario(int idRol, string tipoUsuario)
```

#### Helpers/PasswordHasher.cs
Hash y validación de contraseñas:
- `HashPassword(string password)`: BCrypt con workFactor 11
- `VerifyPassword(string password, string hash)`: Verificación BCrypt
- `ValidatePasswordStrength(string password)`: Retorna (bool, string)
  - Mín 8 caracteres
  - Al menos 1 mayúscula, 1 minúscula, 1 número, 1 carácter especial

#### Helpers/JwtTokenGenerator.cs ⭐ NUEVO
Generación y validación de tokens JWT:
- `GenerateAccessToken(Usuario user, IEnumerable<Rol> roles)`: Crea JWT con claims (ID de usuario, username, email, roles, permisos)
- `GenerateRefreshToken()`: Crea refresh token seguro
- Configuración de token desde appsettings.json:
  - SecretKey: `dfa154978480f0d80bbf048c3eb8e3a8`
  - Issuer: G2rismBetaAPI
  - Audience: G2rismBetaClient
  - Access token: 60 minutos
  - Refresh token: 7 días

#### Helpers/TokenGenerator.cs
Generación de token legacy (usado para recuperación de contraseña en sistema antiguo):
- `GenerateToken()`: Token aleatorio seguro (URL-safe)
- **NOTA**: Siendo eliminado progresivamente en favor de códigos de 6 dígitos

#### Helpers/SendGridEmailService.cs ⭐ NUEVO
Servicio de email listo para producción:
- `SendPasswordRecoveryEmailAsync(string toEmail, string toName, string recoveryCode)`: Envía código de recuperación de 6 dígitos
- Configuración desde appsettings.json:
  - ApiKey: YOUR_SENDGRID_API_KEY
  - FromEmail: noreply@g2rism.com
  - FromName: G2rism Beta - Sistema de Turismo

### Sistema de Autorización ⭐ NUEVO

#### Autorización Basada en Permisos

**Authorization/PermissionRequirement.cs**: Requisito de autorización personalizado
**Authorization/PermissionAuthorizationHandler.cs**: Manejador personalizado que verifica permisos de usuario desde claims JWT

**Políticas Definidas** (en Program.cs):

1. **Políticas Basadas en Roles**:
   - `RequireSuperAdminRole`
   - `RequireAdminRole`
   - `RequireEmployeeRole`

2. **Políticas Basadas en Permisos** (40+ políticas):
   - Configuración: `RequirePermission:roles.{crear|leer|actualizar|eliminar}`
   - Configuración: `RequirePermission:permisos.{crear|leer|actualizar|eliminar}`
   - Servicios: `RequirePermission:hoteles.{crear|leer|actualizar|eliminar}`
   - Servicios: `RequirePermission:servicios.{crear|leer|actualizar|eliminar}`
   - Servicios: `RequirePermission:paquetes.{crear|leer|actualizar|eliminar}`
   - Reservas: `RequirePermission:reservas.{crear|leer|actualizar|eliminar}`
   - Financiero: `RequirePermission:facturas.{crear|leer|actualizar|eliminar}`
   - Financiero: `RequirePermission:formasdepago.{crear|leer|actualizar|eliminar}`
   - Financiero: `RequirePermission:pagos.{crear|leer|actualizar|eliminar}`

**Uso en Controladores**:
```csharp
[Authorize(Policy = "RequirePermission:hoteles.crear")]
public async Task<ActionResult> CreateHotel(...)

[Authorize(Policy = "RequireSuperAdminRole")]
public async Task<ActionResult> DeleteUser(...)
```

### Limitación de Velocidad ⭐ NUEVO

**Políticas Configuradas**:

1. **auth**: Login/Registro - 5 solicitudes por minuto
2. **password-recovery**: Recuperación de contraseña - 3 solicitudes por hora
3. **refresh**: Actualización de token - 10 por minuto
4. **api**: API General - 100 solicitudes por minuto (ventana deslizante)
5. **Limitador Global**: Basado en IP - 200 solicitudes por minuto

**Aplicación**:
- Middleware: `app.UseRateLimiter()` en Program.cs
- Aplicado a endpoints específicos con atributo `[EnableRateLimiting("nombre-politica")]`

### Conexión de Base de Datos

**Ubicación de Cadena de Conexión**: `appsettings.json` → `ConnectionStrings:DefaultConnection`

**Configuración Actual**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=g2rism_beta_db;User=root;Password=mysqlPOPESVD6505.;"
  }
}
```

**IMPORTANTE**:
- Nunca hacer commit de credenciales reales al control de versiones
- Usar variables de entorno o user secrets en producción
- La cadena de conexión usa `ServerVersion.AutoDetect()` para compatibilidad con MySQL

### Estrategia de Seeding

**DbInitializer.Initialize()** es llamado al inicio de la aplicación (Program.cs):

1. **Aplica migraciones pendientes** (si las hay)
2. **Verifica datos existentes** (idempotente)
3. **Inicializa datos iniciales** si la base de datos está vacía:

   **Roles** (4):
   - Super Administrador (nivel 1)
   - Administrador (nivel 2)
   - Empleado (nivel 10)
   - Cliente (nivel 50)

   **Permisos** (8+):
   - roles.crear, roles.leer, roles.actualizar, roles.eliminar
   - permisos.crear, permisos.leer, permisos.actualizar, permisos.eliminar
   - (Más permisos agregados para nuevos módulos)

   **Asignaciones Rol-Permiso**:
   - Super Admin: TODOS los permisos
   - Admin: TODOS excepto eliminaciones sensibles
   - Empleado: Permisos de lectura + algunos de creación
   - Cliente: Limitado a operaciones de cliente

   **Métodos de Pago** (7) ⭐ NUEVO:
   - Efectivo
   - Tarjeta de Crédito
   - Tarjeta de Débito
   - Transferencia Bancaria
   - PSE
   - Nequi
   - Daviplata

   **Usuarios de Prueba** (3):
   - admin / Admin123! (Super Administrador)
   - empleado1 / Empleado123! (Empleado)
   - cliente1 / Cliente123! (Cliente)

**Salida de Seeding**: Logs detallados de consola con emojis y estadísticas

### Migraciones

**Total de Migraciones**: 23 (incrementado desde 9)

**Migraciones Originales** (1-9):
1. `20251028133800_InitialCreate_RolesPermisos`: Tablas Roles, Permisos, RolPermiso
2. `20251031133411_SecondCreateUsuarios`: Tablas Usuarios, UsuarioRol, TokenRecuperacion
3. `20251107002209_ModuloCategoriasCliente`: Tabla CategoriaCliente
4. `20251107123658_ModuloCliente`: Tabla Cliente
5. `20251109175531_ModuloPreferenciasCliente`: Tabla PreferenciaCliente
6. `20251110042441_ModuloEmpleados`: Tabla Empleados
7. `20251110205734_ModuloProveedores`: Tablas Proveedores, ContratosProveedor
8. `20251114173235_ModuloServiciosAerolineas`: Tablas Aerolineas, Vuelos (inicial)
9. `20251120201304_EliminarIdReferenciaDeUsuarios`: Removido campo IdReferencia de Usuario

**NUEVAS Migraciones** ⭐ (10-23):
10. `20251127023313_AgregarTablaRefreshTokens`: Tabla de refresh tokens JWT
11. `20251129034113_AgregarTablaCodigosRecuperacion`: Tabla de códigos de recuperación de 6 dígitos
12. `20251203180059_AgregarCamposAuditoriaAPermisos`: Campos de auditoría para Permiso
13. `20251205002642_CompletarModuloVuelos`: Modelo Vuelo completo con todos los campos
14. `20251205011234_ModuloHoteles`: Tabla Hoteles con campos JSON
15. `20251205023642_ModuloServiciosAdicionales`: Tabla de servicios adicionales
16. `20251207144613_ModuloPaquetesTuristicos`: Tabla de paquetes turísticos con campos JSON
17. `20251209020136_ModuloReservas`: Tabla principal de reservas
18. `20251209022216_AgregarReservasHoteles`: Tabla de unión Reserva-Hotel
19. `20251209025151_AgregarReservasVuelos`: Tabla de unión Reserva-Vuelo
20. `20251209110921_AgregarReservaPaquete`: Tabla de unión Reserva-Paquete
21. `20251209172430_AgregarRelacionReservasServicios`: Tabla de unión Reserva-Servicio
22. `20251209181512_ModuloFinanciero`: Módulo financiero (Facturas, Pagos, FormasDePago)

**ApplicationDbContextModelSnapshot.cs**: Snapshot del esquema de base de datos actual

## Flujo de Trabajo de Desarrollo

### Agregar un Nuevo Módulo

Al agregar una nueva entidad/módulo, seguir este orden:

1. **Modelo** (`Models/TuEntidad.cs`):
   - Definir entidad con anotaciones apropiadas
   - Incluir propiedades calculadas con `[NotMapped]`
   - Agregar comentarios de documentación XML
   - Considerar columnas JSON para arrays/objetos flexibles

2. **DbContext** (`Data/ApplicationDbContext.cs`):
   - Agregar propiedad `DbSet<TuEntidad>`
   - Configurar relaciones en `OnModelCreating`:
     - Definir índices (únicos, rendimiento, compuestos)
     - Configurar claves foráneas
     - Establecer comportamientos en cascada (Restrict vs Cascade)
     - Agregar restricciones
     - Configurar columnas JSON si es necesario

3. **Migración**:
   ```bash
   dotnet ef migrations add ModuloTuEntidad
   ```
   - Revisar código de migración generado
   - Verificar índices y restricciones
   - Verificar configuración de columnas JSON

4. **DTOs** (`DTOs/TuEntidad/`):
   - Crear `TuEntidadCreateDto.cs`
   - Crear `TuEntidadUpdateDto.cs` (TODOS los campos nullable)
   - Crear `TuEntidadResponseDto.cs`
   - Crear DTOs especiales si es necesario (ConXxx, AsignarXxx, CompletaXxx)

5. **AutoMapper** (`Mappings/MappingProfile.cs`):
   - Agregar mapeo CreateDto → Model
   - Agregar mapeo UpdateDto → Model con `.ForAllMembers(opt => opt.Condition(...))` para actualizaciones parciales
   - Agregar mapeo Model → ResponseDto
   - Manejar propiedades calculadas y objetos anidados

6. **Interfaz de Repositorio** (`Interfaces/ITuEntidadRepository.cs`):
   ```csharp
   public interface ITuEntidadRepository : IGenericRepository<TuEntidad>
   {
       Task<TuEntidad?> GetByXxxAsync(int id);
       Task<bool> ExistsByXxxAsync(string xxx);
       // Consultas personalizadas específicas de esta entidad
   }
   ```

7. **Implementación de Repositorio** (`Repositories/TuEntidadRepository.cs`):
   - Heredar de `GenericRepository<TuEntidad>`
   - Implementar consultas personalizadas
   - Usar Include() para carga ansiosa

8. **Interfaz de Servicio** (`Interfaces/ITuEntidadService.cs`):
   ```csharp
   public interface ITuEntidadService
   {
       Task<TuEntidadResponseDto> CreateAsync(TuEntidadCreateDto dto);
       Task<IEnumerable<TuEntidadResponseDto>> GetAllAsync();
       // Métodos de negocio personalizados
   }
   ```

9. **Implementación de Servicio** (`Services/TuEntidadService.cs`):
   - Implementar lógica de negocio
   - Agregar validación (lanzar excepciones apropiadas)
   - Usar repositorio para acceso a datos
   - Usar AutoMapper para conversiones DTO
   - Manejar cálculos complejos si es necesario

10. **Validadores** (`Validators/`):
    - Crear `TuEntidadCreateDtoValidator.cs`
    - Crear `TuEntidadUpdateDtoValidator.cs`
    - Heredar de `AbstractValidator<T>`
    - Agregar validaciones estructurales (FluentValidation)

11. **Controlador** (`Controllers/TuEntidadesController.cs`):
    - Usar `[ApiController]` y `[Route("api/[controller]")]`
    - Inyectar servicio, mapper, logger
    - Retornar `ApiResponse<T>` o `ApiErrorResponse`
    - Agregar comentarios XML para Swagger
    - Usar códigos de estado HTTP apropiados
    - Agregar atributos de autorización: `[Authorize(Policy = "...")]`

12. **Registrar en Program.cs**:
    ```csharp
    builder.Services.AddScoped<ITuEntidadRepository, TuEntidadRepository>();
    builder.Services.AddScoped<ITuEntidadService, TuEntidadService>();
    ```

13. **Agregar Políticas de Autorización** (si es necesario):
    ```csharp
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("RequirePermission:tumodulo.crear", policy =>
            policy.Requirements.Add(new PermissionRequirement("tumodulo.crear")));
    ```

14. **Aplicar Migración**:
    ```bash
    dotnet ef database update
    ```

15. **Probar**:
    - Usar Swagger UI para probar endpoints
    - Verificar reglas de validación (FluentValidation + Capa de servicio)
    - Verificar lógica de negocio
    - Probar políticas de autorización
    - Probar actualizaciones parciales (PUT con algunos campos nulos)

### Hacer Cambios a Entidades Existentes

1. **Modificar la clase Model**
2. **Actualizar AutoMapper** mapeos si los DTOs cambiaron
3. **Actualizar Validadores** si las reglas de validación cambiaron
4. **Crear Migración**:
   ```bash
   dotnet ef migrations add NombreCambioDescriptivo
   ```
5. **Revisar Migración** código cuidadosamente
6. **Aplicar Migración**:
   ```bash
   dotnet ef database update
   ```

### Convenciones de Controlador

**Estructura Estándar**:
```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]  // Requerir autenticación para todos los endpoints
public class TuController : ControllerBase
{
    private readonly ITuServicio _service;
    private readonly IMapper _mapper;
    private readonly ILogger<TuController> _logger;

    // Inyección de constructor

    [HttpGet]
    [Authorize(Policy = "RequirePermission:tumodulo.leer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TuDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<TuDto>>>> GetAll()
    {
        _logger.LogInformation("📋 Obteniendo todos...");
        // Implementación
    }

    [HttpPost]
    [Authorize(Policy = "RequirePermission:tumodulo.crear")]
    [ProducesResponseType(typeof(ApiResponse<TuDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<TuDto>>> Create([FromBody] TuCreateDto dto)
    {
        _logger.LogInformation("📝 Creando...");
        // Implementación
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }
}
```

**Puntos Clave**:
- Usar `[ApiController]` para validación automática de modelo
- Agregar `[Authorize]` a nivel de clase, políticas específicas a nivel de método
- Retornar `ApiResponse<T>` para éxito
- Retornar `ApiErrorResponse` para errores (manejado por middleware)
- Usar códigos de estado HTTP estándar
- Agregar comentarios XML para Swagger
- Registrar operaciones importantes con emojis para legibilidad

**Códigos de Estado HTTP**:
- 200 OK: GET/PUT exitoso
- 201 Created: POST exitoso (con encabezado Location)
- 204 No Content: DELETE exitoso
- 400 Bad Request: Errores de validación, violaciones de reglas de negocio
- 401 Unauthorized: Token JWT faltante o inválido
- 403 Forbidden: Token válido pero permisos insuficientes
- 404 Not Found: Recurso no encontrado
- 500 Internal Server Error: Excepciones no manejadas

### Convenciones de Nomenclatura

**Controladores**:
- Nombres plurales: `RolesController`, `ClientesController`, `HotelesController`, `ReservasController`

**Servicios/Repositorios**:
- Nombre de entidad singular + Service/Repository
- Ejemplos: `RolService`, `HotelRepository`, `ReservaService`

**Interfaces**:
- Prefijadas con `I`
- Ejemplos: `IRolService`, `IHotelRepository`, `IGenericRepository<T>`

**DTOs**:
- Nombre de entidad + propósito
- Ejemplos: `RolCreateDto`, `HotelUpdateDto`, `ReservaResponseDto`
- Especiales: `RolConPermisosDto`, `UsuarioConRolesDto`, `ReservaCompletaCreateDto`

**Validadores**:
- Nombre DTO + Validator
- Ejemplos: `RolCreateDtoValidator`, `HotelUpdateDtoValidator`

**Tablas de Base de Datos**:
- Nombres plurales en español
- Ejemplos: `roles`, `clientes`, `hoteles`, `reservas`, `facturas`, `pagos`

**Columnas**:
- Snake case: `id_usuario`, `fecha_creacion`, `numero_factura`

**Claves Foráneas**:
- `id_` + nombre de entidad
- Ejemplos: `id_rol`, `id_hotel`, `id_reserva`, `id_factura`

**Propiedades de Navegación**:
- Singular para 1:1 y N:1: `Usuario`, `Categoria`, `Jefe`, `Hotel`, `Factura`
- Plural para 1:N: `Clientes`, `Subordinados`, `Contratos`, `Vuelos`, `Pagos`
- Tablas de unión para N:M: `RolesPermisos`, `UsuariosRoles`, `ReservasHoteles`, `ReservasVuelos`

## Notas Importantes

### 1. Política CORS
Actualmente establecida en `AllowAll` para desarrollo:
```csharp
policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
```
**IMPORTANTE**: Restringir a orígenes frontend específicos en producción.

### 2. Seguridad de Contraseñas
- **NUNCA** almacenar contraseñas en texto plano
- Usar `PasswordHasher.HashPassword()` antes de guardar
- Verificar con `PasswordHasher.VerifyPassword(plainText, hash)`
- BCrypt workFactor: 11 (balance entre seguridad y rendimiento)
- Requisitos de contraseña: 8+ caracteres, mayúscula, minúscula, número, carácter especial

### 3. Autenticación JWT ⭐
- **Access tokens**: 60 minutos de expiración
- **Refresh tokens**: 7 días de expiración con rotación
- **Revocación de tokens**: Soportada vía tabla RefreshToken
- **Claims incluidos**: ID de Usuario, Username, Email, Roles (array), Permisos (array)
- **Clock skew cero**: ClockSkew = TimeSpan.Zero para expiración precisa
- **IMPORTANTE**: Cambiar clave secreta JWT en producción (variable de entorno)

### 4. Swagger/OpenAPI
- **URL**: `http://localhost:5026/` (Swagger UI en raíz en Desarrollo)
- **Solo en modo Desarrollo**
- **Soporte JWT**: Click en botón "Authorize", ingresar token (no se necesita prefijo "Bearer")
- **Título**: "G2rism Beta API - Módulo de Configuración"
- **Versión**: v1.0

### 5. Orden de Middleware ⭐ CRÍTICO
```csharp
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();  // ¡PRIMERO!
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRateLimiter();  // NUEVO
app.UseCors("AllowAll");
app.UseAuthentication();  // NUEVO - DEBE estar antes de Authorization
app.UseAuthorization();
app.MapControllers();
```

### 6. Relaciones de Entidades
- **Carga Ansiosa**: Usar `Include()` y `ThenInclude()` en repositorios
- **Referencias Circulares**: AutoMapper maneja esto automáticamente
- **Comportamiento de Eliminación**: Ver sección 8 en Patrones de Diseño de Base de Datos
- **Columnas JSON**: Usar para arrays/objetos flexibles (evitar tablas de unión excesivas)

### 7. Seguridad de Migraciones
- **SIEMPRE** revisar migraciones antes de aplicar
- **NUNCA** eliminar migraciones aplicadas a producción
- Usar nombres descriptivos: `ModuloNombre`, `AgregarCampoX`, etc.
- Verificar índices y restricciones en código generado
- Probar estrategia de rollback antes de despliegue a producción
- **IMPORTANTE**: Revisar configuraciones de columnas JSON

### 8. Repositorio Genérico
- Usar para operaciones CRUD estándar
- Extender con métodos personalizados para consultas complejas
- Todos los métodos son async
- SaveChangesAsync() debe ser llamado explícitamente

### 9. Capa de Servicio
- **Las reglas de negocio pertenecen aquí**, no en controladores o repositorios
- Validar antes de llamar a repositorios
- Lanzar excepciones específicas para diferentes tipos de error
- Usar constantes (como RoleConstants) para lógica de negocio
- Mantener controladores delgados (solo orquestación)
- **Cálculos complejos** (financieros, disponibilidad) pertenecen en servicios

### 10. Validación de DTO
- **FluentValidation** para validación estructural/de formato (49 validadores)
- **Capa de Servicio** para validación de lógica de negocio
- Validadores auto-registrados (AddValidatorsFromAssembly)
- Validadores personalizados para reglas dependientes de base de datos
- Enfoque de dos capas previene que datos inválidos lleguen al servicio

### 11. Actualizaciones Parciales ⭐ CRÍTICO
- Las clases UpdateDto tienen **TODAS las propiedades nullable**
- AutoMapper configurado con `.Condition()` para ignorar nulos:
  ```csharp
  .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null))
  ```
- Solo los campos proporcionados se actualizan
- Campos de auditoría (FechaModificacion) actualizados automáticamente
- **Aplicado a TODOS los mapeos UpdateDto** (correcciones de errores recientes)

### 12. Logging
- Logging estructurado con ILogger
- Emojis para categorización visual:
  - 📝 Creando
  - ✅ Éxito
  - ⚠️ Advertencia
  - ❌ Error
  - 🔍 Buscando
  - 🔗 Asignando
  - 🗑️ Eliminando
  - 💰 Operaciones financieras
  - 📧 Envío de email
- Registrar operaciones importantes con datos contextuales

### 13. Aplicación de Reglas de Negocio

**Reglas de Negocio Críticas**:

1. **Unicidad de Super Administrador**:
   - Solo UN usuario puede tener el rol Super Administrador
   - Validado en UsuarioService.CrearUsuarioAsync y AsignarRolesAsync
   - Usa UsuarioRolRepository.ExisteSuperAdministradorAsync()

2. **Compatibilidad Rol-Tipo de Usuario**:
   - Usuarios empleados: Solo roles Super Admin, Admin o Empleado
   - Usuarios clientes: Solo rol Cliente
   - Validado usando RoleConstants.EsRolValidoParaTipoUsuario()
   - Previene mezcla de roles de empleado y cliente

3. **Prevenir Eliminación con Dependencias**:
   - No se puede eliminar CategoriaCliente si tiene Clientes activos
   - No se puede eliminar Proveedor si tiene Contratos/Hoteles/Vuelos/Servicios activos
   - No se puede eliminar Reserva si tiene Factura
   - Aplicado por DeleteBehavior.Restrict en base de datos

4. **Fortaleza de Contraseña**:
   - Validada en PasswordHasher.ValidatePasswordStrength()
   - FluentValidation en validadores CreateDto
   - Validación de capa de servicio antes de hashear

5. **Restricciones Únicas**:
   - Aplicadas a nivel de base de datos (índices únicos)
   - Verificaciones de capa de servicio antes de insertar/actualizar
   - Métodos de repositorio personalizados (ExistsByXxxAsync)

6. **Validación de Disponibilidad** ⭐ NUEVO:
   - Vuelos: Verificar cupos_disponibles, reducir atómicamente en reserva
   - Hoteles: Verificar habitaciones_disponibles
   - Paquetes: Verificar cupos_disponibles, reducir atómicamente en reserva
   - Servicios: Verificar bandera disponibilidad

7. **Validación Financiera** ⭐ NUEVO:
   - Monto de pago no puede exceder SaldoPendiente de factura
   - Solo facturas pendiente/parcialmente pagadas pueden recibir pagos
   - Estado de factura se actualiza automáticamente cuando está completamente pagada
   - Totales financieros de reserva recalculados en cambios de servicio

8. **Asignación de Permisos** ⭐ CAMBIADO:
   - **Estrategia acumulativa**: Asignar permisos agrega a existentes (no reemplaza)
   - Cambiado en commit 33450ca

### 14. Consideraciones de Seguridad

**Implementación Actual** ✅:
- Hash de contraseñas con BCrypt (workFactor 11)
- Autenticación JWT con refresh tokens
- Autorización basada en permisos (40+ políticas)
- Limitación de velocidad (5 políticas)
- Seguimiento de intentos de login (IntentosFallidos)
- Bloqueo de cuenta (campo Bloqueado)
- Códigos de recuperación de 6 dígitos (expiración 1 hora, 5 intentos máx.)
- Eliminación suave (campo Estado)
- Seguimiento de IP y UserAgent para refresh tokens

**TODO para Producción** ⚠️:
- [ ] Cambiar clave secreta JWT (variable de entorno)
- [ ] Aplicación HTTPS (RequireHttpsMetadata = true)
- [ ] Restricción CORS a orígenes frontend específicos
- [ ] Mover todos los secretos a variables de entorno o Azure Key Vault
- [ ] Implementar clave API SendGrid real (actualmente placeholder)
- [ ] Agregar logging de auditoría para operaciones sensibles (pagos, generación de facturas)
- [ ] Implementar facturación electrónica DIAN (generación CUFE/CUDE)
- [ ] Agregar validación de carga de archivos (imágenes para hoteles, paquetes)

### 15. Desarrollo vs Producción

**Configuración Actual** (Desarrollo):
- Swagger habilitado
- Política CORS AllowAll
- Mensajes de error detallados con stack traces
- Cadena de conexión de base de datos en appsettings.json
- RequireHttpsMetadata = false para JWT
- Clave API SendGrid placeholder

**Checklist de Producción**:
- [x] Autenticación JWT ✅
- [x] Autorización basada en permisos ✅
- [x] Limitación de velocidad ✅
- [x] Integración de email (SendGrid) ✅
- [ ] Deshabilitar Swagger
- [ ] Restringir CORS a orígenes específicos
- [ ] Ocultar stack traces en respuestas de error
- [ ] Mover cadena de conexión a variables de entorno o Azure Key Vault
- [ ] Configurar clave API SendGrid real
- [ ] Habilitar redirección HTTPS y requisito HTTPS JWT
- [ ] Agregar endpoints de health check
- [ ] Configurar logging de producción (Application Insights, Serilog)
- [ ] Revisar y optimizar índices
- [ ] Agregar estrategia de caché (Redis para datos accedidos frecuentemente)
- [ ] Implementar paginación para todos los endpoints de lista

### 16. Resumen de Estructura del Proyecto

```
G2rismBeta.API/
├── Authorization/ (2 archivos) ⭐ NUEVO
│   ├── PermissionRequirement.cs
│   └── PermissionAuthorizationHandler.cs
├── Configuration/ (1 archivo) ⭐ NUEVO
│   └── SecuritySettings.cs
├── Constants/
│   └── RoleConstants.cs (IDs de roles, nombres, métodos auxiliares)
├── Controllers/ (19 controladores) ⬆️ DE 11
│   ├── AerolineasController.cs
│   ├── AuthController.cs (mejorado con JWT)
│   ├── CategoriasClienteController.cs
│   ├── ClientesController.cs
│   ├── ContratosProveedorController.cs
│   ├── EmpleadosController.cs
│   ├── FacturasController.cs ⭐ NUEVO
│   ├── FormasDePagoController.cs ⭐ NUEVO
│   ├── HotelesController.cs ⭐ NUEVO
│   ├── PagosController.cs ⭐ NUEVO
│   ├── PaquetesTuristicosController.cs ⭐ NUEVO
│   ├── PermisosController.cs
│   ├── PreferenciasClienteController.cs
│   ├── ProveedoresController.cs
│   ├── ReservasController.cs ⭐ NUEVO (complejo)
│   ├── RolesController.cs
│   ├── ServiciosAdicionalesController.cs ⭐ NUEVO
│   ├── UsuariosController.cs
│   └── VuelosController.cs ⭐ NUEVO
├── Data/
│   ├── ApplicationDbContext.cs (1,015 líneas con todas las configuraciones)
│   ├── ApplicationDbContextFactory.cs (Para migraciones)
│   └── DbInitializer.cs (Lógica de seeding)
├── DTOs/ (82 DTOs en 26 subdirectorios) ⬆️ DE 56
│   ├── Aerolinea/ (3)
│   ├── Auth/ (10) - mejorado con DTOs JWT
│   ├── CategoriaCliente/ (3)
│   ├── Cliente/ (4)
│   ├── ContratoProveedor/ (3)
│   ├── Empleado/ (4)
│   ├── Factura/ (3) ⭐ NUEVO
│   ├── FormaDePago/ (3) ⭐ NUEVO
│   ├── Hotel/ (3) ⭐ NUEVO
│   ├── Pago/ (3) ⭐ NUEVO
│   ├── PaqueteTuristico/ (3) ⭐ NUEVO
│   ├── Permiso/ (3)
│   ├── PreferenciaCliente/ (3)
│   ├── Proveedor/ (3)
│   ├── Reserva/ (4) ⭐ NUEVO (incluye ReservaCompletaCreateDto)
│   ├── ReservaHotel/ (2) ⭐ NUEVO
│   ├── ReservaPaquete/ (2) ⭐ NUEVO
│   ├── ReservaServicio/ (2) ⭐ NUEVO
│   ├── ReservaVuelo/ (2) ⭐ NUEVO
│   ├── Rol/ (4)
│   ├── RolPermiso/ (3)
│   ├── ServicioAdicional/ (3) ⭐ NUEVO
│   ├── Usuario/ (5)
│   ├── UsuarioRol/ (2)
│   └── Vuelo/ (3) ⭐ NUEVO
├── Helpers/
│   ├── JwtTokenGenerator.cs ⭐ NUEVO
│   ├── PasswordHasher.cs (Hash BCrypt + validación)
│   ├── SendGridEmailService.cs ⭐ NUEVO (listo para producción)
│   └── TokenGenerator.cs (legacy)
├── Interfaces/ (49 interfaces) ⬆️ DE 27
│   ├── IGenericRepository.cs
│   ├── 27 repositorios de entidades
│   ├── 24 servicios (incluyendo IAuthService)
├── Mappings/
│   └── MappingProfile.cs (Todos los mapeos AutoMapper para 29 entidades)
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs (Manejo de excepciones + stack traces formateados)
├── Migrations/ (23 migraciones + snapshot) ⬆️ DE 9
├── Models/ (29 entidades) ⬆️ DE 14
│   ├── Aerolinea.cs
│   ├── ApiErrorResponse.cs
│   ├── ApiResponse.cs
│   ├── CategoriaCliente.cs
│   ├── Cliente.cs
│   ├── CodigoRecuperacion.cs ⭐ NUEVO
│   ├── ContratoProveedor.cs
│   ├── Empleado.cs
│   ├── Factura.cs ⭐ NUEVO
│   ├── FormaDePago.cs ⭐ NUEVO
│   ├── Hotel.cs ⭐ NUEVO
│   ├── Pago.cs ⭐ NUEVO
│   ├── PaqueteTuristico.cs ⭐ NUEVO
│   ├── Permiso.cs
│   ├── PreferenciaCliente.cs
│   ├── Proveedor.cs
│   ├── RefreshToken.cs ⭐ NUEVO
│   ├── Reserva.cs ⭐ NUEVO
│   ├── ReservaHotel.cs ⭐ NUEVO
│   ├── ReservaPaquete.cs ⭐ NUEVO
│   ├── ReservaServicio.cs ⭐ NUEVO
│   ├── ReservaVuelo.cs ⭐ NUEVO
│   ├── Rol.cs
│   ├── RolPermiso.cs
│   ├── ServicioAdicional.cs ⭐ NUEVO
│   ├── TokenRecuperacion.cs (LEGACY - siendo eliminado)
│   ├── Usuario.cs
│   ├── UsuarioRol.cs
│   └── Vuelo.cs ⭐ COMPLETADO
├── Repositories/ (28 repositorios) ⬆️ DE 14
│   ├── GenericRepository.cs
│   └── 27 repositorios de entidades
├── Services/ (24 servicios) ⬆️ DE 11
│   ├── AerolineaService.cs
│   ├── AuthService.cs (mejorado con JWT)
│   ├── CategoriaClienteService.cs
│   ├── ClienteService.cs
│   ├── ContratoProveedorService.cs
│   ├── EmpleadoService.cs
│   ├── FacturaService.cs ⭐ NUEVO
│   ├── FormaDePagoService.cs ⭐ NUEVO
│   ├── HotelService.cs ⭐ NUEVO
│   ├── PagoService.cs ⭐ NUEVO
│   ├── PaqueteTuristicoService.cs ⭐ NUEVO
│   ├── PermisoService.cs
│   ├── PreferenciaClienteService.cs
│   ├── ProveedorService.cs
│   ├── ReservaHotelService.cs ⭐ NUEVO
│   ├── ReservaPaqueteService.cs ⭐ NUEVO
│   ├── ReservaService.cs ⭐ NUEVO (lógica de negocio compleja)
│   ├── ReservaServicioService.cs ⭐ NUEVO
│   ├── ReservaVueloService.cs ⭐ NUEVO
│   ├── RolService.cs
│   ├── SendGridEmailService.cs ⭐ NUEVO
│   ├── ServicioAdicionalService.cs ⭐ NUEVO
│   ├── UsuarioService.cs
│   └── VueloService.cs ⭐ NUEVO
├── Validators/ (49 validadores) ⬆️ DE 24
│   └── [Todos los validadores DTO]
├── appsettings.json (Conexión + JWT + SendGrid + config de Seguridad)
├── CLAUDE.md (Este archivo - ACTUALIZADO)
├── G2rismBeta.API.csproj (Archivo de proyecto con dependencias)
└── Program.cs (586 líneas - configuración DI + pipeline middleware)
```

### 17. Resumen de Endpoints de API

**Total: 145+ endpoints** a través de 19 controladores (incrementado desde 95 a través de 11 controladores)

#### Módulo de Configuración (14 endpoints)
**RolesController** (8 endpoints):
- GET /api/roles
- GET /api/roles/{id}
- GET /api/roles/{id}/con-permisos
- POST /api/roles
- PUT /api/roles/{id}
- DELETE /api/roles/{id}
- POST /api/roles/{id}/asignar-permiso
- DELETE /api/roles/{id}/remover-permiso/{idPermiso}

**PermisosController** (6 endpoints):
- GET /api/permisos
- GET /api/permisos/{id}
- GET /api/permisos/modulo/{modulo}
- POST /api/permisos
- PUT /api/permisos/{id}
- DELETE /api/permisos/{id}

#### Módulo de Autenticación y Usuarios (18 endpoints)
**AuthController** (6 endpoints):
- POST /api/auth/register
- POST /api/auth/login (retorna access + refresh tokens)
- POST /api/auth/logout
- POST /api/auth/refresh (rotación de refresh token)
- POST /api/auth/recuperar-password (código de 6 dígitos)
- POST /api/auth/reset-password
- POST /api/auth/cambiar-password

**UsuariosController** (12 endpoints):
- GET /api/usuarios
- GET /api/usuarios/{id}
- GET /api/usuarios/{id}/roles
- POST /api/usuarios
- PUT /api/usuarios/{id}
- DELETE /api/usuarios/{id}
- POST /api/usuarios/{id}/bloquear
- POST /api/usuarios/{id}/desbloquear
- POST /api/usuarios/{id}/activar
- POST /api/usuarios/{id}/desactivar
- POST /api/usuarios/{id}/asignar-roles
- DELETE /api/usuarios/{id}/remover-rol/{idRol}

#### Módulo CRM - Clientes (15 endpoints)
**CategoriasClienteController** (5 endpoints):
- GET /api/categoriascliente
- GET /api/categoriascliente/{id}
- POST /api/categoriascliente
- PUT /api/categoriascliente/{id}
- DELETE /api/categoriascliente/{id}

**ClientesController** (7 endpoints):
- GET /api/clientes
- GET /api/clientes/{id}
- GET /api/clientes/{id}/con-categoria
- GET /api/clientes/categoria/{idCategoria}
- POST /api/clientes
- PUT /api/clientes/{id}
- DELETE /api/clientes/{id}

**PreferenciasClienteController** (3 endpoints):
- GET /api/preferenciascliente
- GET /api/preferenciascliente/{id}
- GET /api/preferenciascliente/cliente/{idCliente}
- POST /api/preferenciascliente
- PUT /api/preferenciascliente/{id}
- (Sin DELETE - eliminación en cascada con Cliente)

#### Módulo CRM - Empleados (8 endpoints)
**EmpleadosController** (8 endpoints):
- GET /api/empleados
- GET /api/empleados/{id}
- GET /api/empleados/{id}/con-jefe
- GET /api/empleados/{id}/subordinados
- GET /api/empleados/departamento/{departamento}
- POST /api/empleados
- PUT /api/empleados/{id}
- DELETE /api/empleados/{id}

#### Módulo de Proveedores (16 endpoints)
**ProveedoresController** (8 endpoints):
- GET /api/proveedores
- GET /api/proveedores/{id}
- GET /api/proveedores/tipo/{tipo}
- GET /api/proveedores/activos
- GET /api/proveedores/calificacion/{min}
- POST /api/proveedores
- PUT /api/proveedores/{id}
- DELETE /api/proveedores/{id}

**ContratosProveedorController** (8 endpoints):
- GET /api/contratosproveedor
- GET /api/contratosproveedor/{id}
- GET /api/contratosproveedor/proveedor/{idProveedor}
- GET /api/contratosproveedor/vigentes
- GET /api/contratosproveedor/proximos-vencer
- POST /api/contratosproveedor
- PUT /api/contratosproveedor/{id}
- DELETE /api/contratosproveedor/{id}

#### Módulo de Servicios de Viaje (35 endpoints) ⭐
**AerolineasController** (7 endpoints):
- GET /api/aerolineas
- GET /api/aerolineas/{id}
- GET /api/aerolineas/pais/{pais}
- GET /api/aerolineas/activas
- GET /api/aerolineas/buscar/{codigo}
- POST /api/aerolineas
- PUT /api/aerolineas/{id}

**VuelosController** ⭐ (7 endpoints):
- GET /api/vuelos
- GET /api/vuelos/{id}
- GET /api/vuelos/aerolinea/{idAerolinea}
- GET /api/vuelos/disponibles
- GET /api/vuelos/buscar?origen=XXX&destino=YYY
- POST /api/vuelos
- PUT /api/vuelos/{id}

**HotelesController** ⭐ (7 endpoints):
- GET /api/hoteles
- GET /api/hoteles/{id}
- GET /api/hoteles/ciudad/{ciudad}
- GET /api/hoteles/pais/{pais}
- GET /api/hoteles/estrellas/{estrellas}
- POST /api/hoteles
- PUT /api/hoteles/{id}

**ServiciosAdicionalesController** ⭐ (7 endpoints):
- GET /api/serviciosadicionales
- GET /api/serviciosadicionales/{id}
- GET /api/serviciosadicionales/proveedor/{idProveedor}
- GET /api/serviciosadicionales/tipo/{tipo}
- GET /api/serviciosadicionales/disponibles
- POST /api/serviciosadicionales
- PUT /api/serviciosadicionales/{id}

**PaquetesTuristicosController** ⭐ (7 endpoints):
- GET /api/paquetesturisticos
- GET /api/paquetesturisticos/{id}
- GET /api/paquetesturisticos/tipo/{tipo}
- GET /api/paquetesturisticos/destino/{destino}
- GET /api/paquetesturisticos/disponibles
- POST /api/paquetesturisticos
- PUT /api/paquetesturisticos/{id}

#### Módulo de Reservas (15+ endpoints) ⭐
**ReservasController** ⭐ (15+ endpoints):
- GET /api/reservas
- GET /api/reservas/{id}
- POST /api/reservas (reserva simple)
- POST /api/reservas/completa (crear con todos los servicios atómicamente)
- PUT /api/reservas/{id}
- DELETE /api/reservas/{id}
- PUT /api/reservas/{id}/estado
- POST /api/reservas/{id}/hoteles (agregar hotel a reserva existente)
- DELETE /api/reservas/{id}/hoteles/{idHotel}
- POST /api/reservas/{id}/vuelos (agregar vuelo a reserva existente)
- DELETE /api/reservas/{id}/vuelos/{idVuelo}
- POST /api/reservas/{id}/paquetes (agregar paquete a reserva existente)
- DELETE /api/reservas/{id}/paquetes/{idPaquete}
- POST /api/reservas/{id}/servicios (agregar servicio a reserva existente)
- DELETE /api/reservas/{id}/servicios/{idServicio}

#### Módulo Financiero (19 endpoints) ⭐
**FormasDePagoController** ⭐ (5 endpoints):
- GET /api/formasdepago
- GET /api/formasdepago/{id}
- GET /api/formasdepago/activas
- POST /api/formasdepago
- PUT /api/formasdepago/{id}

**FacturasController** ⭐ (7 endpoints):
- GET /api/facturas
- GET /api/facturas/{id}
- GET /api/facturas/reserva/{idReserva}
- GET /api/facturas/estado/{estado}
- GET /api/facturas/vencidas
- POST /api/facturas
- PUT /api/facturas/{id}

**PagosController** ⭐ (7 endpoints):
- GET /api/pagos
- GET /api/pagos/{id}
- GET /api/pagos/factura/{idFactura}
- GET /api/pagos/estado/{estado}
- GET /api/pagos/forma/{idFormaDePago}
- POST /api/pagos (procesa pago, actualiza factura)
- PUT /api/pagos/{id}

### 18. Estadísticas de Código

**Resumen**:

| Categoría | Cantidad | Cambio desde Nov 24 |
|----------|----------|---------------------|
| **Modelos** | 29 | +15 (107% de incremento) |
| **Controladores** | 19 | +8 (73% de incremento) |
| **Servicios** | 24 | +13 (118% de incremento) |
| **Repositorios** | 28 | +14 (100% de incremento) |
| **DTOs** | 82 | +26 (46% de incremento) |
| **Validadores** | 49 | +25 (104% de incremento) |
| **Migraciones** | 23 | +14 (155% de incremento) |
| **Interfaces** | 49 | +22 (81% de incremento) |
| **Total de Endpoints** | 145+ | +50 (53% de incremento) |

**Archivos Clave**:
- **ApplicationDbContext.cs**: 1,015 líneas (todas las 29 configuraciones de entidad)
- **Program.cs**: 586 líneas (DI + middleware + políticas)
- **Total de archivos C#**: ~250+ archivos
- **LOC total estimado**: ~35,000+ líneas

### 19. Actividad de Desarrollo Reciente

**Cambios Principales (Dic 5-12, 2024)**:

1. **Autenticación y Autorización JWT** (Dic 5-8):
   - Implementación JWT completa con refresh tokens
   - 40+ políticas de autorización basadas en permisos
   - Políticas de limitación de velocidad

2. **Completación de Servicios de Viaje** (Dic 5-7):
   - Módulo Vuelos completado
   - Módulo Hoteles agregado (campos JSON para fotos, servicios)
   - Módulo ServiciosAdicionales agregado
   - Módulo PaquetesTuristicos agregado (campos JSON para itinerarios)

3. **Sistema de Reservas** (Dic 9):
   - Entidad Reserva compleja con cálculos automáticos
   - 4 tablas de unión (Hoteles, Vuelos, Paquetes, Servicios)
   - Endpoint de creación de reserva completa (transacción atómica)
   - Endpoints de gestión de servicios (agregar/remover después de creación)

4. **Módulo Financiero** (Dic 9):
   - Sistema de facturación con numeración automática
   - Procesamiento de pagos con soporte de pago parcial
   - Campos de cumplimiento DIAN (placeholders)
   - 7 métodos de pago inicializados

5. **Correcciones de Errores y Mejoras** (Dic 10-12):
   - Corregido soporte de actualización parcial en todos los módulos
   - Corregidas políticas de autorización
   - Cambiada asignación de permisos a acumulativa
   - Corregidas condiciones de AutoMapper para campos nullable
   - Mejoras de seeding

**Commits de Git** (Últimos 5):
```
29f9cea fix(financiero): seedings completos para datos quemados
8c1937f fix(financiero): corregir autorización y actualizaciones parciales
f45b1a0 fix(servicios): corregir actualizaciones parciales
dae6d70 fix(reservas): corregir autorización y actualizaciones parciales
ac6bca9 fix(paquetes): agregar autorización y corregir actualizaciones parciales
```

### 20. Estrategia de Pruebas

**Actual**: Pruebas manuales vía Swagger UI

**Recomendado para Producción**:

1. **Pruebas Unitarias**:
   - Lógica de negocio de capa de servicio (especialmente ReservaService, FacturaService, PagoService)
   - Validadores (reglas FluentValidation)
   - Métodos auxiliares (PasswordHasher, RoleConstants, JwtTokenGenerator)
   - Propiedades calculadas en modelos

2. **Pruebas de Integración**:
   - Capa de repositorio con base de datos en memoria
   - Endpoints de controlador (todos los 145+)
   - Mapeos de AutoMapper (29 entidades)
   - Políticas de autorización (40+ políticas)

3. **Pruebas End-to-End**:
   - Flujos de usuario completos:
     - Registrar → Login → Token JWT → Operaciones autorizadas
     - Crear reserva → Agregar servicios → Generar factura → Procesar pago
     - Flujo de recuperación de contraseña (código de 6 dígitos)
     - Rotación de refresh token
   - Aplicación de reglas de negocio
   - Manejo de errores

**Herramientas de Prueba**:
- xUnit o NUnit
- FluentAssertions
- Moq (para mocking)
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.AspNetCore.Mvc.Testing (para pruebas de integración)

## Referencia Rápida

### Tareas Comunes

**Crear una nueva entidad**:
1. Modelo → DbContext → Migración → DTOs → Mapeos → Repositorio → Servicio → Validadores → Controlador → Políticas de autorización → Registrar en Program.cs → Aplicar migración

**Agregar un nuevo endpoint**:
1. Agregar método a interfaz de servicio → Implementar en servicio → Agregar acción de controlador → Agregar política de autorización → Probar en Swagger

**Modificar esquema de base de datos**:
1. Actualizar modelo → Crear migración → Revisar → Aplicar migración

**Depurar error de validación**:
1. Verificar validador FluentValidation → Verificar validación de capa de servicio → Verificar anotaciones de modelo

**Corregir error de relación**:
1. Verificar configuración OnModelCreating → Verificar propiedades de navegación → Verificar comportamiento en cascada → Recrear migración

**Agregar nuevo permiso**:
1. Inicializar permiso en DbInitializer → Crear política en Program.cs → Agregar a roles en seeding → Aplicar a endpoints de controlador

### Consultas Útiles

**Encontrar todos los usos de un servicio**:
```bash
grep -r "ITuServicio" --include="*.cs"
```

**Listar todos los endpoints**:
Verificar métodos HTTP de cada controlador o usar Swagger UI

**Ver esquema de base de datos actual**:
Verificar Migrations/ApplicationDbContextModelSnapshot.cs

**Ver datos inicializados**:
Verificar Data/DbInitializer.cs

**Encontrar todas las políticas de autorización**:
Verificar sección AddAuthorizationBuilder de Program.cs

### Consejos de Rendimiento

1. Usar `.AsNoTracking()` para consultas de solo lectura
2. **Implementar paginación** para conjuntos de resultados grandes (CRÍTICO para producción)
3. Usar índices estratégicamente (ya configurados para consultas comunes)
4. Considerar caché para datos accedidos frecuentemente:
   - Métodos de pago (raramente cambian)
   - Categorías (raramente cambian)
   - Roles y permisos activos
5. Usar Select() para proyectar solo campos necesarios
6. Implementar carga perezosa cuidadosamente (preferir Include explícito)
7. Monitorear problemas de consulta N+1 (usar Include ansiosamente)
8. Considerar Redis para datos de sesión y búsquedas accedidas frecuentemente

## Mejoras Futuras

**Fase 2** ✅ COMPLETADO:
- [x] Implementar autenticación JWT ✅
- [x] Agregar atributos de autorización basados en permisos ✅
- [x] Completar funcionalidad de envío de email (SendGrid) ✅
- [x] Implementar campos de rastro de auditoría ✅
- [x] Limitación de velocidad ✅

**Fase 3** ✅ COMPLETADO:
- [x] Completar modelo y controlador Vuelo ✅
- [x] Agregar módulo de reserva/reservación ✅
- [x] Implementar procesamiento de pagos ✅
- [x] Agregar módulo financiero (facturación) ✅

**Fase 4** (En Progreso):
- [ ] Agregar soporte de carga de archivos (imágenes de hotel, imágenes de paquete, recibos de pago)
- [ ] Implementar facturación electrónica DIAN (generación CUFE/CUDE)
- [ ] Agregar dashboard de reportes y analíticas
- [ ] Implementar paginación para todos los endpoints de lista
- [ ] Agregar capa de caché (Redis)

**Fase 5** (Planeado):
- [ ] Soporte multi-idioma (i18n)
- [ ] Mejoras de API para aplicación móvil
- [ ] Notificaciones en tiempo real (SignalR)
- [ ] Búsqueda y filtrado avanzado (Elasticsearch)
- [ ] Exportación de datos (facturas PDF, reportes Excel)
- [ ] Logging de rastro de auditoría para operaciones sensibles
- [ ] Suite de pruebas automatizadas (unitarias + integración)

---

**Última Actualización**: 12 de Diciembre de 2025
**Analizado Por**: Claude Sonnet 4.5
**Versión del Proyecto**: Beta 1.0 MVP
**Versión de Base de Datos**: Migración #23 (ModuloFinanciero)
**Total de Endpoints**: 145+
**Estado de Producción**: ~85% (se necesita refuerzo de seguridad menor)
