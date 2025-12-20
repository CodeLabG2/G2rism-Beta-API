# G2rism Beta API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-9.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Sistema de Gestión Turística - API RESTful**

Desarrollado por [CodeLabG2](https://github.com/CodeLabG2)

[Documentación](#-documentación) • [Instalación](#-instalación) • [API Reference](#-endpoints-principales) • [Arquitectura](#-arquitectura)

</div>

---

## Tabla de Contenidos

- [Descripción](#-descripción)
- [Características](#-características)
- [Stack Tecnológico](#-stack-tecnológico)
- [Arquitectura](#-arquitectura)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Base de Datos](#-base-de-datos)
- [Autenticación y Autorización](#-autenticación-y-autorización)
- [Módulos Funcionales](#-módulos-funcionales)
- [Endpoints Principales](#-endpoints-principales)
- [Validación](#-validación)
- [Seguridad](#-seguridad)
- [Desarrollo](#-desarrollo)
- [Deployment](#-deployment)
- [Documentación](#-documentación)
- [Licencia](#-licencia)

---

## 📋 Descripción

**G2rism Beta API** es una API REST completa y robusta para la gestión integral de operaciones turísticas. El sistema maneja todo el ciclo de vida de una agencia de viajes, desde la gestión de clientes y proveedores hasta la creación de reservas complejas y facturación automatizada.

### Estadísticas del Proyecto

```
📁 19 Controladores          (~9,500 líneas de código)
🗂️  29 Modelos de Entidades   (~4,200 líneas de código)
⚙️  24 Servicios de Negocio   (~8,800 líneas de código)
📦 82 DTOs                    (26 módulos organizados)
✅ 49 Validadores             (FluentValidation)
🔌 145+ Endpoints REST
🗄️  23 Migraciones de BD
🔐 40+ Políticas de Autorización
```

### Estado del Proyecto

✅ **MVP Listo para Producción**

- Sistema de autenticación completo (JWT)
- Módulo financiero funcional (facturación y pagos)
- Sistema de reservas multi-servicio
- Gestión completa de CRM (clientes y empleados)
- Catálogo de servicios turísticos
- Documentación Swagger completa

---

## ✨ Características

### Funcionalidades Core

#### 🔐 Sistema de Autenticación Completo
- Registro y login de usuarios
- Autenticación JWT con Access Tokens (60 min) y Refresh Tokens (7 días)
- Recuperación de contraseña con códigos de 6 dígitos vía email (SendGrid)
- Hash de contraseñas con BCrypt (workFactor: 11)
- Bloqueo automático de cuentas tras intentos fallidos
- Validación de fortaleza de contraseña

#### 🛡️ Autorización Avanzada
- Sistema de roles jerárquico (Super Admin, Admin, Empleado, Cliente)
- Autorización basada en permisos granulares (módulo.acción)
- 40+ políticas de autorización predefinidas
- Handler personalizado de permisos con JWT claims
- Restricción: Solo un Super Administrador permitido

#### 👥 Gestión de CRM
- **Clientes**: Categorización, preferencias personalizadas, descuentos
- **Empleados**: Jerarquía organizacional, gestión de departamentos
- Propiedades calculadas (edad, antigüedad, nombre completo)
- Segregación entre clientes y empleados

#### 🤝 Gestión de Proveedores
- Clasificación por tipo (hotel, aerolínea, transporte, servicio)
- Sistema de contratos con seguimiento de vencimiento
- Sistema de calificación (1-5 estrellas)
- Alertas de contratos próximos a vencer

#### ✈️ Catálogo de Servicios
- **Aerolíneas**: Códigos IATA/ICAO, políticas de equipaje
- **Vuelos**: Gestión de disponibilidad, precios por clase, escalas
- **Hoteles**: Clasificación por estrellas, servicios incluidos, geolocalización
- **Servicios Adicionales**: Tours, guías, actividades, transporte interno
- **Paquetes Turísticos**: Tipos variados, temporadas, requisitos

#### 📅 Sistema de Reservas Complejo
- Reservas multi-servicio (hoteles + vuelos + paquetes + servicios)
- Cálculos financieros automáticos (subtotales, descuentos, totales)
- Validación de disponibilidad en tiempo real
- Endpoint de creación compleja (transaccional)
- Gestión de estados (pendiente, confirmada, cancelada, completada)
- Seguimiento de pagos y saldos

#### 💰 Módulo Financiero
- **Facturas**: Numeración automática (FAC-{año}-{consecutivo})
- Cálculo automático de impuestos (IVA)
- Soporte de pagos parciales
- Múltiples formas de pago (Efectivo, Tarjetas, PSE, Nequi, Daviplata)
- Seguimiento de vencimientos y saldos pendientes
- Rastro de auditoría completo

### Características Técnicas

- **Validación de Dos Capas**: FluentValidation + Lógica de Negocio
- **Manejo Global de Excepciones**: Middleware personalizado
- **Rate Limiting**: 5 políticas (auth, password-recovery, refresh, api, global)
- **CORS Configurado**: Soporte multi-origen para frontends
- **Propiedades Calculadas**: Campos derivados con `[NotMapped]`
- **Actualizaciones Parciales**: DTOs con mapeo condicional
- **Logging Estructurado**: ILogger en todos los componentes
- **Documentación Swagger**: Con autenticación JWT integrada

---

## 🛠️ Stack Tecnológico

### Framework y Lenguaje
- **.NET 9.0** (C# 12.0)
- **ASP.NET Core Web API**

### Base de Datos
- **MySQL 9.0**
- **Entity Framework Core 9.0.9** (ORM)
- **Pomelo.EntityFrameworkCore.MySql 9.0.0** (Provider MySQL)

### Seguridad
- **BCrypt.Net-Next 4.0.3** (Hash de contraseñas)
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0**
- **System.IdentityModel.Tokens.Jwt 8.0.1**

### Lógica de Negocio
- **AutoMapper 12.0.1** (Mapeo DTO ↔ Entidad)
- **FluentValidation 11.3.0** (Validación de datos)

### Servicios Externos
- **SendGrid 9.29.3** (Envío de correos electrónicos)

### Documentación
- **Swashbuckle.AspNetCore 9.0.6** (Swagger/OpenAPI)

---

## 🏗️ Arquitectura

### Estructura del Proyecto

```
G2rismBeta.API/
│
├── Controllers/          # 19 controladores REST (~9,500 líneas)
│   ├── AuthController.cs
│   ├── UsuariosController.cs
│   ├── RolesController.cs
│   ├── ClientesController.cs
│   ├── EmpleadosController.cs
│   ├── ReservasController.cs
│   └── ... (13 más)
│
├── Models/              # 29 entidades de dominio (~4,200 líneas)
│   ├── Usuario.cs
│   ├── Rol.cs
│   ├── Cliente.cs
│   ├── Empleado.cs
│   ├── Reserva.cs
│   ├── Factura.cs
│   └── ... (23 más)
│
├── DTOs/                # 82 DTOs organizados en 26 submódulos
│   ├── Auth/
│   ├── Usuarios/
│   ├── Clientes/
│   ├── Reservas/
│   ├── Facturas/
│   └── ...
│
├── Services/            # 24 servicios de lógica de negocio (~8,800 líneas)
│   ├── AuthService.cs
│   ├── UsuarioService.cs
│   ├── ClienteService.cs
│   ├── ReservaService.cs
│   └── ...
│
├── Repositories/        # 28 repositorios de acceso a datos
│   ├── IRepository.cs (genérico)
│   ├── Repository.cs (implementación base)
│   └── ... (específicos por entidad)
│
├── Interfaces/          # 49 contratos de abstracción
│   ├── Services/
│   └── Repositories/
│
├── Validators/          # 49 validadores FluentValidation
│   ├── Auth/
│   ├── Clientes/
│   ├── Reservas/
│   └── ...
│
├── Mappings/            # Perfiles de AutoMapper
│   └── MappingProfile.cs
│
├── Data/                # Contexto y configuración de BD
│   ├── ApplicationDbContext.cs
│   ├── DbInitializer.cs
│   └── ApplicationDbContextFactory.cs
│
├── Middleware/          # Middleware personalizado
│   └── GlobalExceptionHandlerMiddleware.cs
│
├── Authorization/       # Sistema de autorización
│   └── PermissionAuthorizationHandler.cs
│
├── Helpers/             # Utilidades
│   ├── JwtTokenGenerator.cs
│   ├── PasswordHasher.cs
│   ├── EmailService.cs
│   └── TokenGenerator.cs
│
├── Configuration/       # Configuración de seguridad
│   └── JwtConfiguration.cs
│
├── Constants/           # Constantes del sistema
│   └── RoleConstants.cs
│
└── Migrations/          # 23 migraciones de base de datos
    ├── 20250101000000_InitialCreate.cs
    └── ... (22 más)
```

### Patrones de Diseño

- **Repository Pattern**: Abstracción de acceso a datos
- **Service Layer Pattern**: Lógica de negocio separada
- **DTO Pattern**: Transferencia de datos entre capas
- **Dependency Injection**: Inyección de dependencias nativa de .NET
- **Unit of Work**: Transacciones manejadas por DbContext
- **AutoMapper**: Mapeo automático entre DTOs y entidades

---

## 🚀 Instalación

### Requisitos Previos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL 9.0+](https://dev.mysql.com/downloads/mysql/)
- [Git](https://git-scm.com/)
- Editor de código (Visual Studio 2022, VS Code, Rider)

### Pasos de Instalación

1. **Clonar el repositorio**

```bash
git clone https://github.com/CodeLabG2/g2rism-beta-api.git
cd g2rism-beta-api
```

2. **Restaurar dependencias**

```bash
dotnet restore
```

3. **Configurar la base de datos**

Editar `appsettings.json` con tus credenciales de MySQL:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=g2rism_beta_db;User=tu_usuario;Password=tu_password"
}
```

4. **Ejecutar migraciones**

```bash
dotnet ef database update
```

Esto creará la base de datos con:
- 29 tablas
- Datos de seeding (roles, permisos, usuarios de prueba, formas de pago)

5. **Configurar SendGrid (opcional para producción)**

Editar `appsettings.json`:

```json
"SendGrid": {
  "ApiKey": "TU_SENDGRID_API_KEY",
  "FromEmail": "noreply@tudominio.com",
  "FromName": "G2rism Beta"
}
```

6. **Ejecutar la aplicación**

```bash
dotnet run
```

La API estará disponible en:
- **HTTP**: http://localhost:5026
- **HTTPS**: https://localhost:7026
- **Swagger UI**: http://localhost:5026/

---

## ⚙️ Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Port=3306;Database=g2rism_beta_db;User=root;Password=***"
  },
  "Jwt": {
    "SecretKey": "dfa154978480f0d80bbf048c3eb8e3a8",
    "Issuer": "G2rismBetaAPI",
    "Audience": "G2rismBetaClient",
    "AccessTokenExpirationMinutes": "60",
    "RefreshTokenExpirationDays": "7"
  },
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "FromEmail": "noreply@g2rism.com",
    "FromName": "G2rism Beta - Sistema de Turismo"
  },
  "Security": {
    "AllowedFrontendUrls": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Variables de Entorno (Producción)

Para producción, se recomienda usar variables de entorno en lugar de appsettings.json:

```bash
export ConnectionStrings__DefaultConnection="Server=..."
export Jwt__SecretKey="tu-clave-secreta-super-segura"
export SendGrid__ApiKey="tu-api-key"
```

---

## 🗄️ Base de Datos

### Esquema de Tablas (29 tablas)

#### Configuración
- `roles` - Roles del sistema
- `permisos` - Permisos granulares
- `roles_permisos` - Relación muchos-a-muchos

#### Usuarios y Autenticación
- `usuarios` - Información de usuarios
- `usuarios_roles` - Asignación de roles a usuarios
- `refresh_tokens` - Tokens de actualización JWT
- `codigos_recuperacion` - Códigos de recuperación de contraseña

#### CRM - Clientes
- `categorias_cliente` - Categorías con descuentos
- `clientes` - Información de clientes
- `preferencias_cliente` - Preferencias personalizadas

#### CRM - Empleados
- `empleados` - Información de empleados (con jerarquía)

#### Proveedores
- `proveedores` - Información de proveedores
- `contratos_proveedor` - Contratos con seguimiento

#### Servicios
- `aerolineas` - Aerolíneas con códigos IATA/ICAO
- `vuelos` - Vuelos con disponibilidad
- `hoteles` - Hoteles con clasificación
- `servicios_adicionales` - Tours, guías, actividades
- `paquetes_turisticos` - Paquetes completos

#### Reservas
- `reservas` - Reservas principales
- `reservas_hoteles` - Hoteles en reserva (muchos-a-muchos)
- `reservas_vuelos` - Vuelos en reserva (muchos-a-muchos)
- `reservas_paquetes` - Paquetes en reserva (muchos-a-muchos)
- `reservas_servicios` - Servicios en reserva (muchos-a-muchos)

#### Financiero
- `formas_de_pago` - Métodos de pago
- `facturas` - Facturas con numeración automática
- `pagos` - Pagos parciales/completos

### Migraciones

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update MigracionAnterior

# Eliminar última migración (sin aplicar)
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script
```

### Seeding de Datos

Al iniciar la aplicación por primera vez, se crean automáticamente:

**4 Roles**:
- Super Administrador (nivel 1)
- Administrador (nivel 2)
- Empleado (nivel 10)
- Cliente (nivel 50)

**8+ Permisos Base**:
- roles.crear, roles.leer, roles.actualizar, roles.eliminar
- permisos.crear, permisos.leer, permisos.actualizar, permisos.eliminar

**7 Formas de Pago**:
- Efectivo, Tarjeta de Crédito, Tarjeta de Débito, Transferencia Bancaria, PSE, Nequi, Daviplata

**3 Usuarios de Prueba**:
```
Usuario: admin | Contraseña: Admin123! | Rol: Super Administrador
Usuario: empleado1 | Contraseña: Empleado123! | Rol: Empleado
Usuario: cliente1 | Contraseña: Cliente123! | Rol: Cliente
```

---

## 🔐 Autenticación y Autorización

### Flujo de Autenticación JWT

```
┌─────────────┐
│   Cliente   │
└──────┬──────┘
       │
       │ 1. POST /api/auth/login
       │    { username, password }
       ▼
┌─────────────────────────────┐
│   AuthController/Service    │
│  - Valida credenciales      │
│  - Genera Access Token      │
│  - Genera Refresh Token     │
│  - Almacena Refresh Token   │
└──────┬──────────────────────┘
       │
       │ 2. Responde con tokens
       │    { accessToken, refreshToken, expiresIn }
       ▼
┌─────────────┐
│   Cliente   │──────────────────────────────────┐
│  (Almacena  │                                  │
│   tokens)   │                                  │
└──────┬──────┘                                  │
       │                                         │
       │ 3. Requests subsecuentes                │
       │    Authorization: Bearer {accessToken}  │
       ▼                                         │
┌─────────────────────────────┐                 │
│   Middleware JWT            │                 │
│  - Valida Access Token      │                 │
│  - Extrae Claims            │                 │
│  - Autoriza según política  │                 │
└──────┬──────────────────────┘                 │
       │                                         │
       │ 4. Acceso concedido                     │
       ▼                                         │
┌─────────────┐                                 │
│  Endpoint   │                                 │
└─────────────┘                                 │
                                                │
    (Access Token expira después de 60 min)    │
                                                │
┌─────────────┐                                 │
│   Cliente   │                                 │
│ (Detecta    │◄────────────────────────────────┘
│  expiración)│
└──────┬──────┘
       │
       │ 5. POST /api/auth/refresh
       │    { refreshToken }
       ▼
┌─────────────────────────────┐
│   AuthController/Service    │
│  - Valida Refresh Token     │
│  - Genera nuevo Access Token│
└──────┬──────────────────────┘
       │
       │ 6. Responde con nuevo Access Token
       ▼
┌─────────────┐
│   Cliente   │
└─────────────┘
```

### Configuración JWT

**Access Token** (60 minutos):
- Claims incluidos: userId, username, email, roles, permisos
- Algoritmo: HMAC-SHA256
- Issuer: G2rismBetaAPI
- Audience: G2rismBetaClient

**Refresh Token** (7 días):
- Token seguro generado aleatoriamente
- Almacenado en base de datos con fecha de expiración
- Vinculado a usuario y dispositivo (opcional)
- Se revoca al hacer logout

### Sistema de Autorización

#### Autorización Basada en Roles

```csharp
[Authorize(Policy = "RequireSuperAdminRole")]
public async Task<ActionResult> DeleteUser(int id) { }

[Authorize(Policy = "RequireAdminRole")]
public async Task<ActionResult> ManageEmployees() { }

[Authorize(Policy = "RequireEmployeeRole")]
public async Task<ActionResult> ViewReports() { }
```

#### Autorización Basada en Permisos

```csharp
[Authorize(Policy = "RequirePermission:hoteles.crear")]
public async Task<ActionResult> CreateHotel(CreateHotelDto dto) { }

[Authorize(Policy = "RequirePermission:reservas.leer")]
public async Task<ActionResult> GetReservations() { }

[Authorize(Policy = "RequirePermission:facturas.actualizar")]
public async Task<ActionResult> UpdateInvoice(int id, UpdateFacturaDto dto) { }
```

#### Políticas de Autorización (40+ definidas)

**Por Roles**:
- `RequireSuperAdminRole`
- `RequireAdminRole`
- `RequireEmployeeRole`

**Por Permisos** (formato: `RequirePermission:{modulo}.{accion}`):
```
roles.crear, roles.leer, roles.actualizar, roles.eliminar
usuarios.crear, usuarios.leer, usuarios.actualizar, usuarios.eliminar
clientes.crear, clientes.leer, clientes.actualizar, clientes.eliminar
empleados.crear, empleados.leer, empleados.actualizar, empleados.eliminar
proveedores.crear, proveedores.leer, proveedores.actualizar, proveedores.eliminar
hoteles.crear, hoteles.leer, hoteles.actualizar, hoteles.eliminar
vuelos.crear, vuelos.leer, vuelos.actualizar, vuelos.eliminar
servicios.crear, servicios.leer, servicios.actualizar, servicios.eliminar
paquetes.crear, paquetes.leer, paquetes.actualizar, paquetes.eliminar
reservas.crear, reservas.leer, reservas.actualizar, reservas.eliminar
facturas.crear, facturas.leer, facturas.actualizar, facturas.eliminar
pagos.crear, pagos.leer, pagos.actualizar, pagos.eliminar
```

### Rate Limiting

**5 Políticas Configuradas**:

| Política | Límite | Ventana | Aplicación |
|----------|--------|---------|------------|
| `auth` | 5 requests | 1 minuto | Login, Registro |
| `password-recovery` | 3 requests | 1 hora | Recuperación de contraseña |
| `refresh` | 10 requests | 1 minuto | Renovación de tokens |
| `api` | 100 requests | 1 minuto | Endpoints generales |
| `global` | 200 requests | 1 minuto | Por IP (global) |

Uso:
```csharp
[EnableRateLimiting("auth")]
public async Task<ActionResult> Login(LoginDto dto) { }
```

---

## 📦 Módulos Funcionales

### 1. Módulo de Configuración (Roles y Permisos)

**Controladores**: `RolesController`, `PermisosController`
**Entidades**: `Rol`, `Permiso`, `RolPermiso`

**Características**:
- Sistema de roles jerárquico (4 niveles)
- Permisos granulares (módulo + acción)
- Relación muchos-a-muchos con estrategia acumulativa
- Control de estado activo/inactivo
- Restricción: Solo un Super Administrador

**Endpoints principales**:
```
GET    /api/roles
POST   /api/roles
GET    /api/roles/{id}
PUT    /api/roles/{id}
DELETE /api/roles/{id}
POST   /api/roles/{id}/permisos/asignar
GET    /api/permisos
POST   /api/permisos
```

---

### 2. Módulo de Autenticación y Usuarios

**Controladores**: `AuthController`, `UsuariosController`
**Entidades**: `Usuario`, `UsuarioRol`, `RefreshToken`, `CodigoRecuperacion`

**Características**:
- Autenticación JWT (Access + Refresh tokens)
- Recuperación de contraseña con códigos de 6 dígitos vía SendGrid
- Hash de contraseñas con BCrypt (workFactor: 11)
- Validación de fortaleza de contraseña
- Bloqueo automático de cuentas
- Segregación: clientes vs empleados

**Endpoints principales**:
```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
POST /api/auth/recuperar-password
POST /api/auth/reset-password
GET  /api/usuarios
POST /api/usuarios
POST /api/usuarios/{id}/roles/asignar
```

---

### 3. Módulo CRM - Clientes

**Controladores**: `CategoriasClienteController`, `ClientesController`, `PreferenciasClienteController`
**Entidades**: `CategoriaCliente`, `Cliente`, `PreferenciaCliente`

**Características**:
- Segmentación con categorías y descuentos
- Relación 1:1 Cliente-PreferenciaCliente (cascada)
- Propiedades calculadas: `Edad`, `NombreCompleto`
- Seguimiento de preferencias: alojamiento, destinos, actividades, presupuesto
- Restricción única en `DocumentoIdentidad`

**Endpoints principales**:
```
GET  /api/categoriascliente
POST /api/clientes
GET  /api/clientes/{id}
PUT  /api/clientes/{id}
GET  /api/preferenciascliente/cliente/{idCliente}
PUT  /api/preferenciascliente/{id}
```

---

### 4. Módulo CRM - Empleados

**Controlador**: `EmpleadosController`
**Entidad**: `Empleado`

**Características**:
- Jerarquía de empleados (auto-referencia con `IdJefe`)
- Navegación: `Empleado.Jefe` y `Empleado.Subordinados`
- Propiedades calculadas: `NombreCompleto`, `Edad`, `AntiguedadAnios`, `EsJefe`, `CantidadSubordinados`
- DeleteBehavior.Restrict en jerarquía

**Endpoints principales**:
```
GET  /api/empleados
POST /api/empleados
GET  /api/empleados/{id}
PUT  /api/empleados/{id}
GET  /api/empleados/{id}/subordinados
GET  /api/empleados/departamento/{departamento}
```

---

### 5. Módulo de Proveedores

**Controladores**: `ProveedoresController`, `ContratosProveedorController`
**Entidades**: `Proveedor`, `ContratoProveedor`

**Características**:
- Tipos: hotel, aerolínea, transporte, servicio
- Sistema de calificación (1-5)
- Gestión de contratos con seguimiento de expiración
- Propiedades calculadas: `EstaVigente`, `DiasRestantes`, `ProximoAVencer`
- Restricciones únicas: `NitRut`, `NumeroContrato`

**Endpoints principales**:
```
GET  /api/proveedores/tipo/{tipo}
GET  /api/proveedores/activos
POST /api/proveedores
GET  /api/contratosProveedor/proveedor/{id}
GET  /api/contratosProveedor/proximos-vencer
POST /api/contratosProveedor
```

---

### 6. Módulo de Servicios de Viaje

**Controladores**: `AerolineasController`, `VuelosController`, `HotelesController`, `ServiciosAdicionalesController`, `PaquetesTuristicosController`

#### Aerolíneas
**Características**:
- Códigos IATA (2 chars) e ICAO (3 chars) únicos
- Políticas de equipaje
- Relación 1:N con Vuelos

**Endpoints**:
```
GET  /api/aerolineas
POST /api/aerolineas
GET  /api/aerolineas/codigo/{codigo}
```

#### Vuelos
**Características**:
- Gestión de disponibilidad (`cupos_disponibles`)
- Precios por clase (económica, ejecutiva, primera)
- Escalas y ciudades intermedias
- Estados: programado, cancelado, retrasado, completado

**Endpoints**:
```
GET  /api/vuelos
GET  /api/vuelos/disponibles
POST /api/vuelos
PUT  /api/vuelos/{id}/disponibilidad
```

#### Hoteles
**Características**:
- Sistema de estrellas (1-5)
- Campos JSON: `Fotos`, `ServiciosIncluidos`
- Geolocalización (latitud, longitud)
- Políticas de cancelación

**Endpoints**:
```
GET  /api/hoteles
GET  /api/hoteles/ciudad/{ciudad}
POST /api/hoteles
GET  /api/hoteles/estrellas/{estrellas}
```

#### Servicios Adicionales
**Características**:
- Tipos: tour, guía, actividad, transporte_interno
- Campo JSON: `IdiomasDisponibles`
- Gestión de capacidad máxima

**Endpoints**:
```
GET  /api/serviciosAdicionales
GET  /api/serviciosAdicionales/tipo/{tipo}
POST /api/serviciosAdicionales
```

#### Paquetes Turísticos
**Características**:
- Tipos: vacacional, aventura, cultural, negocios, romántico
- Campos JSON: `DestinosAdicionales`, `Incluye`, `Imagenes`
- Gestión de temporadas (alta/baja)
- Requisitos: edad mínima, nivel de dificultad

**Endpoints**:
```
GET  /api/paquetesTuristicos
GET  /api/paquetesTuristicos/disponibles
POST /api/paquetesTuristicos
GET  /api/paquetesTuristicos/tipo/{tipo}
```

---

### 7. Módulo de Reservas (Complejo)

**Controlador**: `ReservasController`
**Entidades**: `Reserva`, `ReservaHotel`, `ReservaVuelo`, `ReservaPaquete`, `ReservaServicio`

**Características**:
- **Reservas multi-servicio**: hoteles + vuelos + paquetes + servicios
- **Cálculos automáticos**:
  - `MontoTotal` = Suma de subtotales
  - `SaldoPendiente` = MontoTotal - MontoPagado
  - Propiedades calculadas: `EstaPagada`, `PorcentajePagado`, `DiasHastaViaje`
- **Endpoint de creación compleja**: `POST /api/reservas/completa` (transaccional)
- **Validación de disponibilidad** en tiempo real
- Estados: pendiente, confirmada, cancelada, completada

**Endpoints principales**:
```
POST /api/reservas/completa              # Crear reserva completa (recomendado)
GET  /api/reservas
GET  /api/reservas/{id}
GET  /api/reservas/cliente/{id}
POST /api/reservas/{id}/hoteles/agregar
POST /api/reservas/{id}/vuelos/agregar
POST /api/reservas/{id}/paquetes/agregar
POST /api/reservas/{id}/servicios/agregar
PUT  /api/reservas/{id}/estado
DELETE /api/reservas/{id}
```

**Ejemplo de creación completa**:
```json
POST /api/reservas/completa
{
  "idCliente": 1,
  "fechaReserva": "2025-01-01",
  "fechaInicio": "2025-02-01",
  "fechaFin": "2025-02-10",
  "observaciones": "Luna de miel",
  "hoteles": [
    {
      "idHotel": 5,
      "tipoHabitacion": "Suite",
      "cantidadHabitaciones": 1,
      "fechaCheckIn": "2025-02-01",
      "fechaCheckOut": "2025-02-10"
    }
  ],
  "vuelos": [
    {
      "idVuelo": 3,
      "clase": "ejecutiva",
      "cantidadPasajeros": 2
    }
  ],
  "paquetes": [
    {
      "idPaquete": 2,
      "cantidadPersonas": 2,
      "personalizaciones": { "incluyeDesayuno": true }
    }
  ],
  "servicios": [
    {
      "idServicio": 1,
      "cantidadParticipantes": 2,
      "fechaServicio": "2025-02-05"
    }
  ]
}
```

---

### 8. Módulo Financiero (Listo para Producción)

**Controladores**: `FormasDePagoController`, `FacturasController`, `PagosController`
**Entidades**: `FormaDePago`, `Factura`, `Pago`

#### Formas de Pago
**Características**:
- Métodos predefinidos: Efectivo, Tarjetas, Transferencia, PSE, Nequi, Daviplata
- Inicializados automáticamente en seeding

**Endpoints**:
```
GET /api/formasDePago
GET /api/formasDePago/activas
```

#### Facturas
**Características**:
- **Numeración automática**: `FAC-{año}-{consecutivo}`
- **Cálculos de impuestos**:
  ```
  BaseGravable = Subtotal - Descuentos
  Impuestos = BaseGravable × (PorcentajeIva / 100)
  Total = BaseGravable + Impuestos
  ```
- Campos DIAN: `ResolucionDian`, `CufeCude` (placeholders MVP)
- Estados: pendiente, pagada, cancelada, vencida
- Propiedades calculadas: `EstaVencida`, `MontoPagado`, `SaldoPendiente`
- Relación 1:1 con Reserva

**Endpoints**:
```
POST /api/facturas
GET  /api/facturas
GET  /api/facturas/{id}
GET  /api/facturas/reserva/{id}
GET  /api/facturas/vencidas
PUT  /api/facturas/{id}
DELETE /api/facturas/{id}
```

#### Pagos
**Características**:
- **Pagos parciales** soportados
- Actualización automática del estado de factura
- Validación: monto no excede saldo pendiente
- Rastro de auditoría: `ReferenciaTransaccion`, `ComprobantePago`
- Estados: pendiente, aprobado, rechazado

**Endpoints**:
```
POST /api/pagos
GET  /api/pagos
GET  /api/pagos/{id}
GET  /api/pagos/factura/{id}
PUT  /api/pagos/{id}
DELETE /api/pagos/{id}
```

**Ejemplo de flujo completo**:
```json
# 1. Crear reserva
POST /api/reservas/completa
{
  "idCliente": 1,
  ...
}
# Response: { id: 10, montoTotal: 5000000 }

# 2. Generar factura
POST /api/facturas
{
  "idReserva": 10,
  "subtotal": 5000000,
  "descuentos": 0,
  "porcentajeIva": 19,
  "fechaEmision": "2025-01-15",
  "fechaVencimiento": "2025-01-30"
}
# Response: { numeroFactura: "FAC-2025-00042", total: 5950000 }

# 3. Registrar pago parcial
POST /api/pagos
{
  "idFactura": 42,
  "idFormaDePago": 2,
  "monto": 3000000,
  "referenciaTransaccion": "TRX-12345",
  "fechaPago": "2025-01-20"
}
# Response: { estado: "aprobado" }

# 4. Registrar pago final
POST /api/pagos
{
  "idFactura": 42,
  "idFormaDePago": 2,
  "monto": 2950000,
  "referenciaTransaccion": "TRX-12346",
  "fechaPago": "2025-01-25"
}
# Response: { estado: "aprobado" }
# Factura se marca automáticamente como "pagada"
```

---

## 🔌 Endpoints Principales

### Autenticación

```http
POST   /api/auth/register             # Registrar nuevo usuario
POST   /api/auth/login                # Iniciar sesión (retorna JWT)
POST   /api/auth/refresh              # Renovar access token
POST   /api/auth/logout               # Cerrar sesión (revoca refresh token)
POST   /api/auth/recuperar-password   # Generar código de 6 dígitos
POST   /api/auth/reset-password       # Restablecer contraseña con código
```

### Usuarios y Roles

```http
GET    /api/usuarios                  # Listar usuarios
POST   /api/usuarios                  # Crear usuario
GET    /api/usuarios/{id}             # Obtener usuario
PUT    /api/usuarios/{id}             # Actualizar usuario
DELETE /api/usuarios/{id}             # Eliminar usuario
POST   /api/usuarios/{id}/roles/asignar  # Asignar roles

GET    /api/roles                     # Listar roles
POST   /api/roles                     # Crear rol
POST   /api/roles/{id}/permisos/asignar  # Asignar permisos
```

### Clientes

```http
GET    /api/clientes                  # Listar clientes
POST   /api/clientes                  # Crear cliente
GET    /api/clientes/{id}             # Obtener cliente
PUT    /api/clientes/{id}             # Actualizar cliente
DELETE /api/clientes/{id}             # Eliminar cliente
```

### Empleados

```http
GET    /api/empleados                 # Listar empleados
POST   /api/empleados                 # Crear empleado
GET    /api/empleados/{id}            # Obtener empleado
GET    /api/empleados/{id}/subordinados  # Obtener subordinados
GET    /api/empleados/departamento/{dept}  # Filtrar por departamento
```

### Proveedores

```http
GET    /api/proveedores               # Listar proveedores
GET    /api/proveedores/tipo/{tipo}   # Filtrar por tipo
GET    /api/proveedores/activos       # Proveedores activos
POST   /api/proveedores               # Crear proveedor
```

### Servicios de Viaje

```http
# Vuelos
GET    /api/vuelos/disponibles        # Vuelos con cupos
POST   /api/vuelos                    # Crear vuelo

# Hoteles
GET    /api/hoteles                   # Listar hoteles
GET    /api/hoteles/ciudad/{ciudad}   # Hoteles por ciudad
POST   /api/hoteles                   # Crear hotel

# Paquetes
GET    /api/paquetesTuristicos/disponibles  # Paquetes con cupos
POST   /api/paquetesTuristicos        # Crear paquete
```

### Reservas

```http
POST   /api/reservas/completa         # Crear reserva completa (recomendado)
GET    /api/reservas                  # Listar reservas
GET    /api/reservas/cliente/{id}     # Reservas de un cliente
PUT    /api/reservas/{id}/estado      # Cambiar estado
```

### Financiero

```http
# Facturas
POST   /api/facturas                  # Generar factura (numeración automática)
GET    /api/facturas/reserva/{id}     # Factura de una reserva
GET    /api/facturas/vencidas         # Facturas vencidas

# Pagos
POST   /api/pagos                     # Registrar pago
GET    /api/pagos/factura/{id}        # Pagos de una factura
```

### Documentación Completa

Swagger UI disponible en: **http://localhost:5026/**

---

## ✅ Validación

### Sistema de Validación de Dos Capas

#### 1. FluentValidation (Estructura y Formato)

Validadores automáticos para todos los DTOs:

```csharp
// Ejemplo: CreateClienteValidator.cs
public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("El nombre es obligatorio")
            .MaximumLength(100).WithMessage("Máximo 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio")
            .EmailAddress().WithMessage("Formato de email inválido");

        RuleFor(x => x.DocumentoIdentidad)
            .NotEmpty().WithMessage("El documento es obligatorio")
            .Matches(@"^\d{8,10}$").WithMessage("Documento inválido (8-10 dígitos)");
    }
}
```

#### 2. Validación de Lógica de Negocio (Servicios)

Validaciones más complejas en la capa de servicios:

```csharp
// Ejemplo: ReservaService.cs
public async Task<ServiceResult<ReservaDto>> CreateReservaAsync(...)
{
    // Validar disponibilidad de vuelo
    if (vuelo.CuposDisponibles < cantidadPasajeros)
    {
        return ServiceResult<ReservaDto>.Failure(
            $"El vuelo solo tiene {vuelo.CuposDisponibles} cupos disponibles"
        );
    }

    // Validar fechas
    if (fechaInicio < DateTime.UtcNow)
    {
        return ServiceResult<ReservaDto>.Failure(
            "La fecha de inicio no puede ser en el pasado"
        );
    }

    // Lógica de negocio...
}
```

### Respuestas de Validación

**Éxito**:
```json
{
  "success": true,
  "data": { ... },
  "message": "Cliente creado exitosamente"
}
```

**Error de Validación**:
```json
{
  "success": false,
  "errors": [
    "El nombre es obligatorio",
    "Formato de email inválido"
  ],
  "message": "Errores de validación"
}
```

**Error de Lógica de Negocio**:
```json
{
  "success": false,
  "message": "El vuelo solo tiene 5 cupos disponibles"
}
```

---

## 🔒 Seguridad

### Medidas de Seguridad Implementadas

#### 1. Autenticación y Autorización
- ✅ JWT con Access Tokens (60 min) y Refresh Tokens (7 días)
- ✅ Autorización basada en roles y permisos
- ✅ Hash de contraseñas con BCrypt (workFactor: 11)
- ✅ Validación de fortaleza de contraseña

#### 2. Protección contra Ataques
- ✅ **Rate Limiting** (5 políticas configuradas)
- ✅ **CORS** configurado (AllowedFrontendUrls)
- ✅ Validación de entrada con FluentValidation
- ✅ Sanitización de datos
- ✅ Prevención de SQL Injection (EF Core parametrizado)

#### 3. Gestión de Secretos
- ⚠️ **IMPORTANTE**: En producción, usar variables de entorno
- ⚠️ Nunca versionar `appsettings.Production.json` con secretos
- ⚠️ Rotar `Jwt.SecretKey` regularmente
- ⚠️ Proteger `SendGrid.ApiKey`

#### 4. HTTPS
- ✅ Configurado por defecto en desarrollo
- ⚠️ **Obligatorio en producción**

#### 5. Bloqueo de Cuentas
- ✅ Bloqueo automático tras intentos fallidos
- ✅ Registro de intentos de login

### Mejores Prácticas de Seguridad

**En Desarrollo**:
```bash
# Usar secretos de usuario de .NET
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "tu-clave-secreta"
dotnet user-secrets set "SendGrid:ApiKey" "tu-api-key"
```

**En Producción**:
```bash
# Variables de entorno
export JWT_SECRET_KEY="clave-super-segura-de-produccion"
export SENDGRID_API_KEY="SG.xxx"
export DB_PASSWORD="password-seguro"
```

---

## 💻 Desarrollo

### Comandos Comunes

```bash
# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con hot reload
dotnet watch run

# Ejecutar tests (si existen)
dotnet test

# Limpiar build
dotnet clean

# Restaurar paquetes
dotnet restore

# Formatear código
dotnet format
```

### Migraciones

```bash
# Crear migración
dotnet ef migrations add NombreDeMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir migración
dotnet ef database update MigracionAnterior

# Eliminar última migración (no aplicada)
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script

# Ver migraciones aplicadas
dotnet ef migrations list
```

### Convenciones de Código

#### Nomenclatura
- **Clases y métodos**: PascalCase (`ClienteService`, `GetClienteAsync`)
- **Parámetros y variables**: camelCase (`idCliente`, `nombre`)
- **Constantes**: UPPER_SNAKE_CASE (`MAX_RETRIES`)
- **Tablas de BD**: snake_case (`clientes`, `reservas_hoteles`)

#### DTOs
- Sufijos según operación:
  - `CreateXxxDto` - Crear
  - `UpdateXxxDto` - Actualizar (todos los campos nullable)
  - `XxxDto` - Lectura/Respuesta
  - `XxxDetailsDto` - Lectura con detalles

#### Servicios
- Métodos asíncronos terminan en `Async`
- Retornan `ServiceResult<T>` o `ServiceResult`
- Incluyen logs con emojis

```csharp
public async Task<ServiceResult<ClienteDto>> CreateClienteAsync(CreateClienteDto dto)
{
    try
    {
        _logger.LogInformation("📝 Creando nuevo cliente: {Email}", dto.Email);

        // Lógica...

        _logger.LogInformation("✅ Cliente creado exitosamente: ID {Id}", cliente.IdCliente);
        return ServiceResult<ClienteDto>.Success(clienteDto, "Cliente creado exitosamente");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Error al crear cliente");
        return ServiceResult<ClienteDto>.Failure("Error al crear cliente");
    }
}
```

#### Repositorios
- Implementan patrón Repository genérico
- Métodos específicos según necesidad
- Operaciones asíncronas

```csharp
public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByDocumentoIdentidadAsync(string documento);
    Task<IEnumerable<Cliente>> GetByCategoriaAsync(int idCategoria);
}
```

### Agregar un Nuevo Módulo

Pasos para agregar un nuevo módulo (ejemplo: `Destinos`):

1. **Crear Entidad** (`Models/Destino.cs`)
```csharp
public class Destino
{
    public int IdDestino { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    // ...
}
```

2. **Agregar DbSet** (`Data/ApplicationDbContext.cs`)
```csharp
public DbSet<Destino> Destinos { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Destino>(entity =>
    {
        entity.ToTable("destinos");
        entity.HasKey(e => e.IdDestino);
        // Configuraciones...
    });
}
```

3. **Crear Migración**
```bash
dotnet ef migrations add ModuloDestinos
dotnet ef database update
```

4. **Crear DTOs** (`DTOs/Destinos/`)
```csharp
public class CreateDestinoDto { ... }
public class UpdateDestinoDto { ... }
public class DestinoDto { ... }
```

5. **Crear Validador** (`Validators/Destinos/CreateDestinoValidator.cs`)

6. **Crear Repositorio** (`Repositories/DestinoRepository.cs`)

7. **Crear Servicio** (`Services/DestinoService.cs`)

8. **Crear Controlador** (`Controllers/DestinosController.cs`)

9. **Registrar en DI** (`Program.cs`)
```csharp
builder.Services.AddScoped<IDestinoRepository, DestinoRepository>();
builder.Services.AddScoped<IDestinoService, DestinoService>();
```

10. **Configurar AutoMapper** (`Mappings/MappingProfile.cs`)
```csharp
CreateMap<Destino, DestinoDto>();
CreateMap<CreateDestinoDto, Destino>();
CreateMap<UpdateDestinoDto, Destino>()
    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
```

11. **Configurar Políticas de Autorización** (`Program.cs`)
```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequirePermission:destinos.crear", ...)
    .AddPolicy("RequirePermission:destinos.leer", ...)
    // ...
```

12. **Crear Permisos en BD** (manualmente o en seeding)
```sql
INSERT INTO permisos (modulo, accion, descripcion)
VALUES ('destinos', 'crear', 'Crear destinos');
```

---

## 🚀 Deployment

### Preparación para Producción

#### 1. Configuración de Entorno

Crear `appsettings.Production.json` (NO versionar):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-db-server;Database=g2rism_prod;..."
  },
  "Jwt": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "G2rismAPI",
    "Audience": "G2rismClient"
  },
  "SendGrid": {
    "ApiKey": "${SENDGRID_API_KEY}"
  },
  "Security": {
    "AllowedFrontendUrls": [
      "https://app.g2rism.com",
      "https://www.g2rism.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 2. Publicar Aplicación

```bash
# Publicar para producción
dotnet publish -c Release -o ./publish

# Publicar con runtime específico (ejemplo: Linux x64)
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

#### 3. Configurar Servidor Web

**Opción 1: Nginx (Reverse Proxy)**

```nginx
server {
    listen 80;
    server_name api.g2rism.com;

    location / {
        proxy_pass http://localhost:5026;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

**Opción 2: IIS (Windows)**

Instalar módulo ASP.NET Core y configurar sitio web apuntando a la carpeta `publish`.

#### 4. Servicio Systemd (Linux)

Crear `/etc/systemd/system/g2rism-api.service`:

```ini
[Unit]
Description=G2rism Beta API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/g2rism-api
ExecStart=/usr/bin/dotnet /var/www/g2rism-api/G2rismBeta.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=g2rism-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=JWT_SECRET_KEY=tu-clave-secreta
Environment=SENDGRID_API_KEY=tu-api-key

[Install]
WantedBy=multi-user.target
```

Iniciar servicio:
```bash
sudo systemctl enable g2rism-api
sudo systemctl start g2rism-api
sudo systemctl status g2rism-api
```

#### 5. SSL/TLS (Certificado HTTPS)

```bash
# Instalar Certbot
sudo apt install certbot python3-certbot-nginx

# Obtener certificado SSL
sudo certbot --nginx -d api.g2rism.com

# Renovación automática
sudo certbot renew --dry-run
```

#### 6. Base de Datos

```bash
# Aplicar migraciones en producción
dotnet ef database update --connection "Server=..."

# O ejecutar script SQL generado previamente
mysql -u usuario -p g2rism_prod < migration-script.sql
```

### Checklist de Deployment

- [ ] Variables de entorno configuradas
- [ ] Cadena de conexión segura (sin credenciales hardcodeadas)
- [ ] `Jwt.SecretKey` rotada y segura (mínimo 32 caracteres)
- [ ] SendGrid API Key configurada
- [ ] CORS configurado con dominios de producción
- [ ] HTTPS habilitado (certificado SSL)
- [ ] Migraciones de BD aplicadas
- [ ] Seeding de datos ejecutado
- [ ] Rate Limiting activo
- [ ] Logs configurados (nivel Warning o Error)
- [ ] Health checks configurados (opcional)
- [ ] Backups de BD programados

### Monitoreo (Recomendado)

**Application Insights** (Azure):
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Serilog** (Logging estructurado):
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

---

## 📖 Documentación

### Documentación Existente

- **README.md** (este archivo): Documentación general del proyecto
- **CLAUDE_ES.md**: Documentación completa en español para Claude Code (~900 líneas)
- **Swagger UI**: http://localhost:5026/ (documentación interactiva de API)

### Documentación de API (Swagger)

Acceder a Swagger UI en desarrollo:
1. Ejecutar la aplicación: `dotnet run`
2. Abrir navegador: http://localhost:5026/
3. Autenticarse con JWT:
   - Click en "Authorize"
   - Ingresar token en formato: `Bearer {tu-access-token}`
   - Click en "Authorize" y luego "Close"

Para obtener un token:
```bash
curl -X POST http://localhost:5026/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "Admin123!"}'
```

### Generar Documentación Adicional

**Comentarios XML para Swagger**:

En el .csproj:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

En controladores:
```csharp
/// <summary>
/// Crea un nuevo cliente
/// </summary>
/// <param name="dto">Datos del cliente a crear</param>
/// <returns>Cliente creado con su ID asignado</returns>
/// <response code="201">Cliente creado exitosamente</response>
/// <response code="400">Errores de validación</response>
/// <response code="401">No autenticado</response>
/// <response code="403">No autorizado</response>
[HttpPost]
[ProducesResponseType(typeof(ApiResponse<ClienteDto>), 201)]
[ProducesResponseType(typeof(ApiErrorResponse), 400)]
public async Task<ActionResult> CreateCliente([FromBody] CreateClienteDto dto)
```

---

## 🤝 Contribución

### Flujo de Trabajo Git

1. **Fork del repositorio**
2. **Crear rama de feature**:
   ```bash
   git checkout -b feature/nueva-funcionalidad
   ```
3. **Hacer commits descriptivos**:
   ```bash
   git commit -m "feat(clientes): agregar endpoint de búsqueda avanzada"
   ```
4. **Push a la rama**:
   ```bash
   git push origin feature/nueva-funcionalidad
   ```
5. **Crear Pull Request**

### Convenciones de Commits

Seguir [Conventional Commits](https://www.conventionalcommits.org/):

```
feat(modulo): descripción breve
fix(modulo): descripción del bug corregido
docs: actualizar README
refactor(modulo): mejorar estructura de código
test(modulo): agregar tests unitarios
chore: tareas de mantenimiento
```

---

## 📄 Licencia

Este proyecto está bajo la licencia **MIT**.

```
MIT License

Copyright (c) 2025 CodeLabG2

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 📞 Contacto

**Desarrollado por**: [CodeLabG2](https://github.com/CodeLabG2)

**Proyecto**: G2rism Beta API
**Repositorio**: https://github.com/CodeLabG2/g2rism-beta-api
**Issues**: https://github.com/CodeLabG2/g2rism-beta-api/issues

---

## 🙏 Agradecimientos

- **Microsoft** - .NET Framework
- **Pomelo Foundation** - MySQL Provider para EF Core
- **SendGrid** - Servicio de correo electrónico
- **JWT.io** - Estándar de autenticación
- **FluentValidation** - Biblioteca de validación
- **AutoMapper** - Mapeo de objetos

---

<div align="center">

**Hecho con ❤️ por CodeLabG2**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-9.0-4479A1?style=flat-square&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)

</div>
