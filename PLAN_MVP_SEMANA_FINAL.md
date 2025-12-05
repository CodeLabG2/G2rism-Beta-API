# 🚀 PLAN DEFINITIVO MVP FUNCIONAL - SEMANA FINAL
## G2rism Beta API - Estrategia de Ejecución

**Fecha de Inicio**: 2025-12-04
**Tiempo Disponible**: 7 días (1 semana)
**Objetivo**: Sistema funcional end-to-end (Backend API + Frontend básico)
**Estado Actual**: 36% completado (14/38 tablas)
**Meta Final**: 85% funcional (MVP demostrable)

---

## 📊 CONTEXTO ACTUAL DEL PROYECTO

### ✅ LO QUE YA ESTÁ IMPLEMENTADO (Excelente Calidad - 8.5/10)

**Módulos Completados al 100%** (5.5/10 módulos):

1. **🔐 SEGURIDAD** - Roles + Permisos
   - ✅ Tablas: `roles`, `permisos`, `roles_permisos`
   - ✅ Controllers: RolesController (8 endpoints), PermisosController (6 endpoints)
   - ✅ Authorization policies basadas en permisos
   - ✅ PermissionAuthorizationHandler personalizado

2. **👤 AUTENTICACIÓN JWT** - Production Ready
   - ✅ Tablas: `usuarios`, `usuarios_roles`, `refresh_tokens`, `codigos_recuperacion`
   - ✅ Controllers: AuthController (8 endpoints), UsuariosController (12 endpoints)
   - ✅ JWT Access Token + Refresh Token
   - ✅ BCrypt password hashing (workFactor 11)
   - ✅ Rate limiting (5 intentos/minuto en login)
   - ✅ Account lockout por intentos fallidos
   - ✅ Recuperación de contraseña con códigos de 6 dígitos

3. **👥 CRM - CLIENTES**
   - ✅ Tablas: `categorias_cliente`, `clientes`, `preferencias_cliente`
   - ✅ Controllers: CategoriasClienteController, ClientesController, PreferenciasClienteController
   - ✅ Sistema de segmentación (categorías con descuentos)
   - ✅ Seguimiento de preferencias (1:1)
   - ✅ Propiedades computadas: Edad, NombreCompleto

4. **👔 CRM - EMPLEADOS**
   - ✅ Tabla: `empleados`
   - ✅ Controller: EmpleadosController (8 endpoints)
   - ✅ Jerarquía organizacional (auto-referencia: Jefe-Subordinados)
   - ✅ Propiedades computadas: Antigüedad, EsJefe, CantidadSubordinados

5. **🏢 PROVEEDORES**
   - ✅ Tablas: `proveedores`, `contratos_proveedor`
   - ✅ Controllers: ProveedoresController, ContratosProveedorController
   - ✅ Tipos: hotel, aerolinea, transporte, servicio
   - ✅ Sistema de calificación (1-5)
   - ✅ Alertas de contratos próximos a vencer
   - ✅ Propiedades computadas en contratos: EstaVigente, DiasRestantes

6. **✈️ AEROLÍNEAS** (Parcial - 50%)
   - ✅ Tabla: `aerolineas` (completamente implementada)
   - ✅ Controller: AerolineasController (7 endpoints)
   - ✅ Validación códigos IATA (2 chars) e ICAO (3 chars)
   - ⚠️ Tabla: `vuelos` (existe pero SIN implementación de código)

### 🎯 FORTALEZAS DEL CÓDIGO ACTUAL

- ✅ **Arquitectura limpia**: Repository + Service + Controller (3 capas)
- ✅ **SOLID principles** aplicados
- ✅ **FluentValidation**: 28 validators implementados
- ✅ **AutoMapper**: Configurado con mappings bidireccionales
- ✅ **Documentación**: Swagger completo + CLAUDE.md extenso
- ✅ **Seguridad robusta**: JWT + BCrypt + Rate Limiting
- ✅ **Base de datos bien diseñada**: Normalización 3NF, índices estratégicos
- ✅ **Migraciones ordenadas**: 10 migraciones secuenciales
- ✅ **Código limpio**: Nombres descriptivos, métodos pequeños
- ✅ **Generic Repository**: Reutilización de código CRUD

### ❌ LO QUE FALTA IMPLEMENTAR (Crítico para MVP)

**Módulos Faltantes** (4.5/10 módulos):

1. ❌ **SERVICIOS COMPLETO** (8 tablas - 1/8 implementada)
   - ❌ Vuelos (tabla existe, falta código)
   - ❌ Hoteles
   - ❌ Paquetes Turísticos
   - ❌ Itinerarios (POSPONER para versión 2.0)
   - ❌ Itinerarios_Actividades (POSPONER)
   - ❌ Servicios Adicionales (tours, guías)
   - ❌ Transportes (POSPONER)

2. ❌ **RESERVAS** (5 tablas - CORE DEL NEGOCIO)
   - ❌ Reservas (tabla principal)
   - ❌ Reservas_Hoteles
   - ❌ Reservas_Vuelos
   - ❌ Reservas_Paquetes
   - ❌ Reservas_Servicios

3. ❌ **FINANCIERO BÁSICO** (3/7 tablas - Priorizar mínimo)
   - ❌ Facturas (CRÍTICO)
   - ❌ Pagos (CRÍTICO)
   - ❌ Formas_de_Pago (CRÍTICO)
   - ⏸️ Cotizaciones (POSPONER)
   - ⏸️ Notas_Credito (POSPONER)
   - ⏸️ Ordenes_Compra (POSPONER)

4. ⏸️ **MÓDULOS POSPUESTOS PARA VERSIÓN 2.0**:
   - ⏸️ Transporte (3 tablas)
   - ⏸️ Comunicación (2 tablas)
   - ⏸️ Auditoría (1 tabla)
   - ⏸️ Configuración Sistema (1 tabla)

---

## 🎯 ESTRATEGIA: MVP FUNCIONAL (Opción 1)

### 🚨 DECISIÓN ESTRATÉGICA TOMADA

**RECHAZADO**: ❌ Migrar a MVC
**Razones**:
- Perderíamos 120+ horas solo migrando lo existente
- MVC con Razor es tecnología del pasado (baja demanda laboral)
- API REST es arquitectura moderna (alta demanda, mejor portafolio)
- No alcanza el tiempo (1 semana)
- Perderíamos todo el progreso actual de calidad

**APROBADO**: ✅ Continuar con API REST + Frontend separado
**Razones**:
- Aprovechar 36% ya implementado (excelente calidad)
- Tecnología moderna (API REST + Frontend desacoplado)
- Trabajo en equipo facilitado (backend y frontend en paralelo)
- Aprendizaje de skills valiosos (consumir APIs)
- Mejor para portafolio profesional
- Factible en 1 semana

### 🎯 OBJETIVO DEL MVP

**Sistema funcional end-to-end que permita**:
```
Cliente → Login → Ve catálogo (Hoteles/Vuelos/Paquetes)
      → Crea Reserva → Sistema calcula total
      → Se genera Factura → Registra Pago
      → Confirmación (log en consola)
```

### 📦 ALCANCE DEL MVP

**Incluye** (Funcionalidad Core - 85%):
- ✅ Autenticación completa (ya está)
- ✅ Gestión de usuarios y roles (ya está)
- ✅ CRM Clientes (ya está)
- ✅ Proveedores (ya está)
- ✅ Catálogo de Servicios (Vuelos, Hoteles, Paquetes básicos)
- ✅ Sistema de Reservas (completo con relaciones N:M)
- ✅ Facturación básica (sin integración DIAN real)
- ✅ Registro de Pagos
- ✅ Cálculo automático de totales
- ✅ Estados de reserva (pendiente/confirmada/cancelada/completada)

**Excluye** (Para versión 2.0 - 15%):
- ❌ Itinerarios detallados día a día
- ❌ Módulo de Transporte completo
- ❌ Cotizaciones
- ❌ Notas de Crédito
- ❌ Órdenes de Compra
- ❌ Comunicación (emails reales)
- ❌ Auditoría avanzada
- ❌ Reportes elaborados
- ❌ Tests exhaustivos (solo smoke tests)

### 🎨 SIMPLIFICACIONES ESTRATÉGICAS

1. **Paquetes Turísticos**: Sin itinerarios detallados
   - Solo descripción general, precio, incluye/no incluye
   - Relación directa Paquete → Reserva (sin itinerarios intermedios)

2. **Facturas**: Campos DIAN preparados pero opcionales
   - `resolucion_dian`, `cufe_cude` → nullable
   - Validación de facturación real para versión 2.0

3. **Confirmaciones**: Log en consola en vez de emails
   - EmailHelper ya existe como placeholder
   - Integración SMTP real para versión 2.0

4. **Transportes**: No implementar en MVP
   - Tabla `transportes` existe (relacionada con proveedores)
   - Asignación de transporte para versión 2.0

---

## 📅 CRONOGRAMA DETALLADO (7 DÍAS)

### 📆 **LUNES** - Día 1 (8 horas)
**Objetivo**: Completar módulo Servicios Parte 1

#### Tarea 1: Completar Vuelos (4 horas)
**Archivos a crear**:
```
✅ Services/VueloService.cs
✅ Interfaces/IVueloService.cs
✅ Repositories/VueloRepository.cs (el genérico puede no ser suficiente)
✅ DTOs/Vuelo/VueloCreateDto.cs
✅ DTOs/Vuelo/VueloUpdateDto.cs
✅ DTOs/Vuelo/VueloResponseDto.cs
✅ Validators/VueloCreateDtoValidator.cs
✅ Validators/VueloUpdateDtoValidator.cs
✅ Controllers/VuelosController.cs
✅ Actualizar Mappings/MappingProfile.cs
```

**Funcionalidades**:
- CRUD completo de vuelos
- Búsqueda por origen/destino
- Filtrado por fecha
- Consulta de disponibilidad (cupos)
- Relación con Aerolíneas (FK)
- Relación con Proveedores (FK)

**Validaciones**:
- Cupos > 0
- Fecha salida > Fecha actual
- Fecha llegada > Fecha salida
- Precios > 0

**Endpoints** (7):
```
GET    /api/vuelos
GET    /api/vuelos/{id}
GET    /api/vuelos/buscar?origen={ciudad}&destino={ciudad}&fecha={date}
GET    /api/vuelos/disponibles
POST   /api/vuelos
PUT    /api/vuelos/{id}
DELETE /api/vuelos/{id}
```

#### Tarea 2: Implementar Hoteles (4 horas)
**Archivos a crear**:
```
✅ Models/Hotel.cs (nuevo modelo)
✅ Migración: dotnet ef migrations add ModuloHoteles
✅ Services/HotelService.cs
✅ Interfaces/IHotelService.cs
✅ Repositories/HotelRepository.cs
✅ DTOs/Hotel/ (Create, Update, Response)
✅ Validators/Hotel (Create, Update)
✅ Controllers/HotelesController.cs
✅ Actualizar MappingProfile.cs
✅ Registrar en Program.cs
```

**Modelo Hotel** (según diagrama ER):
```csharp
public class Hotel
{
    public int IdHotel { get; set; }
    public int IdProveedor { get; set; } // FK → proveedores
    public string Nombre { get; set; }
    public string Ciudad { get; set; }
    public string Direccion { get; set; }
    public string? Contacto { get; set; }
    public string? Descripcion { get; set; }
    public string? Categoria { get; set; }
    public int? Estrellas { get; set; } // 1-5
    public decimal PrecioPorNoche { get; set; }
    public int? CapacidadPorHabitacion { get; set; }
    public bool TieneWifi { get; set; }
    public bool TienePiscina { get; set; }
    public bool TieneRestaurante { get; set; }
    public string? PoliticasCancelacion { get; set; }
    public TimeSpan? CheckInHora { get; set; }
    public TimeSpan? CheckOutHora { get; set; }
    public string? Fotos { get; set; } // JSON
    public bool Estado { get; set; }

    // Navegación
    public virtual Proveedor Proveedor { get; set; }
}
```

**Endpoints** (8):
```
GET    /api/hoteles
GET    /api/hoteles/{id}
GET    /api/hoteles/ciudad/{ciudad}
GET    /api/hoteles/estrellas/{estrellas}
GET    /api/hoteles/activos
POST   /api/hoteles
PUT    /api/hoteles/{id}
DELETE /api/hoteles/{id}
```

---

### 📆 **MARTES** - Día 2 (8 horas)
**Objetivo**: Completar módulo Servicios Parte 2

#### Tarea 1: Implementar Servicios Adicionales (3 horas)
**Archivos a crear**:
```
✅ Models/Servicio.cs (nombre mejor: ServicioAdicional)
✅ Migración: dotnet ef migrations add ModuloServiciosAdicionales
✅ Services/ServicioAdicionalService.cs
✅ Interfaces/IServicioAdicionalService.cs
✅ Repositories/ServicioAdicionalRepository.cs
✅ DTOs/ServicioAdicional/ (Create, Update, Response)
✅ Validators/ServicioAdicional
✅ Controllers/ServiciosAdicionalesController.cs
✅ Actualizar MappingProfile.cs
✅ Registrar en Program.cs
```

**Modelo ServicioAdicional**:
```csharp
public class ServicioAdicional
{
    public int IdServicio { get; set; }
    public int IdProveedor { get; set; } // FK → proveedores
    public string Nombre { get; set; }
    public string Tipo { get; set; } // tour, guia, actividad, transporte_interno
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string Unidad { get; set; } // persona, grupo, hora, dia
    public bool Disponibilidad { get; set; }
    public int? TiempoEstimado { get; set; } // minutos
    public bool Estado { get; set; }

    // Navegación
    public virtual Proveedor Proveedor { get; set; }
}
```

**Endpoints** (7):
```
GET    /api/servicios-adicionales
GET    /api/servicios-adicionales/{id}
GET    /api/servicios-adicionales/tipo/{tipo}
GET    /api/servicios-adicionales/disponibles
POST   /api/servicios-adicionales
PUT    /api/servicios-adicionales/{id}
DELETE /api/servicios-adicionales/{id}
```

#### Tarea 2: Implementar Paquetes Turísticos (5 horas)
**Archivos a crear**:
```
✅ Models/PaqueteTuristico.cs
✅ Migración: dotnet ef migrations add ModuloPaquetesTuristicos
✅ Services/PaqueteTuristicoService.cs
✅ Interfaces/IPaqueteTuristicoService.cs
✅ Repositories/PaqueteTuristicoRepository.cs
✅ DTOs/PaqueteTuristico/ (Create, Update, Response)
✅ Validators/PaqueteTuristico
✅ Controllers/PaquetesTuristicosController.cs
✅ Actualizar MappingProfile.cs
✅ Registrar en Program.cs
```

**Modelo PaqueteTuristico** (SIMPLIFICADO - Sin itinerarios):
```csharp
public class PaqueteTuristico
{
    public int IdPaquete { get; set; }
    public string Nombre { get; set; }
    public string? Detalle { get; set; }
    public string DestinoPrincipal { get; set; }
    public int Duracion { get; set; } // días
    public decimal Precio { get; set; }
    public int CuposDisponibles { get; set; }
    public string? Incluye { get; set; } // JSON array
    public string? NoIncluye { get; set; } // JSON array
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? TipoPaquete { get; set; } // aventura, familiar, empresarial, lujo
    public string? NivelDificultad { get; set; } // bajo, medio, alto
    public int? EdadMinima { get; set; }
    public bool Estado { get; set; }

    // NO incluir navegación a Itinerarios en MVP
}
```

**Endpoints** (9):
```
GET    /api/paquetes-turisticos
GET    /api/paquetes-turisticos/{id}
GET    /api/paquetes-turisticos/destino/{destino}
GET    /api/paquetes-turisticos/tipo/{tipo}
GET    /api/paquetes-turisticos/disponibles
GET    /api/paquetes-turisticos/duracion?min={dias}&max={dias}
POST   /api/paquetes-turisticos
PUT    /api/paquetes-turisticos/{id}
DELETE /api/paquetes-turisticos/{id}
```

---

### 📆 **MIÉRCOLES** - Día 3 (8 horas)
**Objetivo**: Módulo Reservas Parte 1 (Tabla principal + Hoteles)

#### Tarea 1: Tabla Reservas Principal (3 horas)
**Archivos a crear**:
```
✅ Models/Reserva.cs
✅ Migración: dotnet ef migrations add ModuloReservas
✅ Services/ReservaService.cs (básico)
✅ Interfaces/IReservaService.cs
✅ Repositories/ReservaRepository.cs
✅ DTOs/Reserva/ReservaCreateDto.cs
✅ DTOs/Reserva/ReservaUpdateDto.cs
✅ DTOs/Reserva/ReservaResponseDto.cs
✅ Validators/ReservaCreateDtoValidator.cs
✅ Validators/ReservaUpdateDtoValidator.cs
✅ Controllers/ReservasController.cs (CRUD básico)
✅ Actualizar MappingProfile.cs
✅ Registrar en Program.cs
```

**Modelo Reserva**:
```csharp
public class Reserva
{
    public int IdReserva { get; set; }
    public int IdCliente { get; set; } // FK → clientes
    public int IdEmpleado { get; set; } // FK → empleados (quien gestiona)
    public string? Descripcion { get; set; }
    public DateTime FechaInicioViaje { get; set; }
    public DateTime FechaFinViaje { get; set; }
    public int NumeroPasajeros { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoPagado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public string Estado { get; set; } // pendiente, confirmada, cancelada, completada
    public string? Observaciones { get; set; }
    public DateTime FechaHora { get; set; } // fecha de creación de la reserva
    public DateTime? FechaModificacion { get; set; }

    // Navegación
    public virtual Cliente Cliente { get; set; }
    public virtual Empleado Empleado { get; set; }

    // Relaciones con servicios (se agregan después)
    public virtual ICollection<ReservaHotel> ReservasHoteles { get; set; }
    public virtual ICollection<ReservaVuelo> ReservasVuelos { get; set; }
    public virtual ICollection<ReservaPaquete> ReservasPaquetes { get; set; }
    public virtual ICollection<ReservaServicio> ReservasServicios { get; set; }
}
```

**Endpoints básicos** (6):
```
GET    /api/reservas
GET    /api/reservas/{id}
GET    /api/reservas/cliente/{idCliente}
GET    /api/reservas/estado/{estado}
POST   /api/reservas (básico, sin servicios aún)
PUT    /api/reservas/{id}
```

**Validaciones**:
- Fecha inicio < Fecha fin
- Numero pasajeros > 0
- Cliente y Empleado deben existir
- Estado debe ser válido (enum)

#### Tarea 2: Relación Reservas-Hoteles (3 horas)
**Archivos a crear**:
```
✅ Models/ReservaHotel.cs
✅ Actualizar migración anterior o crear nueva
✅ Services/ReservaHotelService.cs
✅ Interfaces/IReservaHotelService.cs
✅ Repositories/ReservaHotelRepository.cs
✅ DTOs/ReservaHotel/ (Create, Response)
✅ Validators/ReservaHotelCreateDtoValidator.cs
✅ Actualizar ReservasController con endpoint para agregar hoteles
```

**Modelo ReservaHotel**:
```csharp
public class ReservaHotel
{
    public int Id { get; set; } // PK independiente
    public int IdReserva { get; set; } // FK → reservas
    public int IdHotel { get; set; } // FK → hoteles
    public DateTime FechaCheckin { get; set; }
    public DateTime FechaCheckout { get; set; }
    public int NumeroHabitaciones { get; set; }
    public string? TipoHabitacion { get; set; } // simple, doble, suite
    public int NumeroHuespedes { get; set; }
    public decimal PrecioPorNoche { get; set; }
    public decimal Subtotal { get; set; }
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Reserva Reserva { get; set; }
    public virtual Hotel Hotel { get; set; }
}
```

**Endpoints adicionales**:
```
POST   /api/reservas/{id}/hoteles (agregar hotel a reserva)
GET    /api/reservas/{id}/hoteles (listar hoteles de reserva)
DELETE /api/reservas/{idReserva}/hoteles/{idReservaHotel}
```

#### Tarea 3: Cálculo Automático de Totales (2 horas)
**Funcionalidad**:
- Cuando se agrega un hotel → sumar subtotal al MontoTotal
- Actualizar SaldoPendiente = MontoTotal - MontoPagado
- Validar que checkout > checkin
- Calcular noches automáticamente
- Calcular subtotal = noches * precioPorNoche * numeroHabitaciones

---

### 📆 **JUEVES** - Día 4 (8 horas)
**Objetivo**: Módulo Reservas Parte 2 (Vuelos + Paquetes + Servicios)

#### Tarea 1: Relación Reservas-Vuelos (2.5 horas)
**Archivos a crear**:
```
✅ Models/ReservaVuelo.cs
✅ Actualizar migración
✅ Services/ReservaVueloService.cs
✅ Interfaces/IReservaVueloService.cs
✅ Repositories/ReservaVueloRepository.cs
✅ DTOs/ReservaVuelo/
✅ Validators/ReservaVueloCreateDtoValidator.cs
✅ Actualizar ReservasController
```

**Modelo ReservaVuelo**:
```csharp
public class ReservaVuelo
{
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public int IdVuelo { get; set; }
    public int NumeroPasajeros { get; set; }
    public string Clase { get; set; } // economica, ejecutiva
    public string? AsientosAsignados { get; set; } // JSON array ["12A", "12B"]
    public decimal PrecioPorPasajero { get; set; }
    public decimal Subtotal { get; set; }
    public bool EquipajeIncluido { get; set; }
    public int? EquipajeExtra { get; set; } // kg adicionales

    // Navegación
    public virtual Reserva Reserva { get; set; }
    public virtual Vuelo Vuelo { get; set; }
}
```

**Endpoints**:
```
POST   /api/reservas/{id}/vuelos
GET    /api/reservas/{id}/vuelos
DELETE /api/reservas/{idReserva}/vuelos/{idReservaVuelo}
```

**Validaciones**:
- Verificar cupos disponibles en vuelo
- Descontar cupos al confirmar
- Subtotal = NumeroPasajeros * PrecioPorPasajero

#### Tarea 2: Relación Reservas-Paquetes (2.5 horas)
**Archivos a crear**:
```
✅ Models/ReservaPaquete.cs
✅ Actualizar migración
✅ Services/ReservaPaqueteService.cs
✅ Interfaces/IReservaPaqueteService.cs
✅ Repositories/ReservaPaqueteRepository.cs
✅ DTOs/ReservaPaquete/
✅ Validators/ReservaPaqueteCreateDtoValidator.cs
✅ Actualizar ReservasController
```

**Modelo ReservaPaquete**:
```csharp
public class ReservaPaquete
{
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public int IdPaquete { get; set; }
    public int NumeroPersonas { get; set; }
    public DateTime FechaInicioPaquete { get; set; }
    public DateTime FechaFinPaquete { get; set; }
    public decimal PrecioPorPersona { get; set; }
    public decimal Subtotal { get; set; }
    public string? Personalizaciones { get; set; } // JSON

    // Navegación
    public virtual Reserva Reserva { get; set; }
    public virtual PaqueteTuristico Paquete { get; set; }
}
```

**Endpoints**:
```
POST   /api/reservas/{id}/paquetes
GET    /api/reservas/{id}/paquetes
DELETE /api/reservas/{idReserva}/paquetes/{idReservaPaquete}
```

#### Tarea 3: Relación Reservas-Servicios (2 horas)
**Archivos a crear**:
```
✅ Models/ReservaServicio.cs
✅ Actualizar migración
✅ Services/ReservaServicioService.cs
✅ Interfaces/IReservaServicioService.cs
✅ Repositories/ReservaServicioRepository.cs
✅ DTOs/ReservaServicio/
✅ Validators/ReservaServicioCreateDtoValidator.cs
✅ Actualizar ReservasController
```

**Modelo ReservaServicio**:
```csharp
public class ReservaServicio
{
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public int IdServicio { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime FechaAgregado { get; set; }
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Reserva Reserva { get; set; }
    public virtual ServicioAdicional Servicio { get; set; }
}
```

**Endpoints**:
```
POST   /api/reservas/{id}/servicios
GET    /api/reservas/{id}/servicios
DELETE /api/reservas/{idReserva}/servicios/{idReservaServicio}
```

#### Tarea 4: Endpoint Completo de Reserva (1 hora)
**Nuevo endpoint**:
```
POST /api/reservas/completa

Body:
{
    "idCliente": 1,
    "idEmpleado": 1,
    "fechaInicioViaje": "2025-12-20",
    "fechaFinViaje": "2025-12-27",
    "numeroPasajeros": 2,
    "hoteles": [
        {
            "idHotel": 5,
            "fechaCheckin": "2025-12-20",
            "fechaCheckout": "2025-12-23",
            "numeroHabitaciones": 1,
            "tipoHabitacion": "doble"
        }
    ],
    "vuelos": [
        {
            "idVuelo": 3,
            "numeroPasajeros": 2,
            "clase": "economica"
        }
    ],
    "paquetes": [],
    "servicios": [
        {
            "idServicio": 2,
            "cantidad": 1
        }
    ]
}
```

**Funcionalidad**:
- Crear reserva + todos los servicios en una transacción
- Calcular totales automáticamente
- Validar disponibilidad de todos los servicios
- Retornar reserva completa con todos los detalles

---

### 📆 **VIERNES** - Día 5 (8 horas)
**Objetivo**: Módulo Financiero Básico

#### Tarea 1: Facturas (4 horas)
**Archivos a crear**:
```
✅ Models/Factura.cs
✅ Migración: dotnet ef migrations add ModuloFinanciero
✅ Services/FacturaService.cs
✅ Interfaces/IFacturaService.cs
✅ Repositories/FacturaRepository.cs
✅ DTOs/Factura/ (Create, Update, Response)
✅ Validators/FacturaCreateDtoValidator.cs
✅ Controllers/FacturasController.cs
✅ Actualizar MappingProfile.cs
✅ Registrar en Program.cs
```

**Modelo Factura**:
```csharp
public class Factura
{
    public int IdFactura { get; set; }
    public int IdReserva { get; set; } // FK → reservas
    public string NumeroFactura { get; set; } // UNIQUE, autogenerado
    public DateTime FechaEmision { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public string? ResolucionDian { get; set; } // OPCIONAL en MVP
    public string? CufeCude { get; set; } // OPCIONAL en MVP
    public string TipoFactura { get; set; } // venta, devolucion
    public string Estado { get; set; } // pendiente, pagada, cancelada, vencida
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal PorcentajeIva { get; set; } // 19% por defecto
    public decimal Descuentos { get; set; }
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Reserva Reserva { get; set; }
    public virtual ICollection<Pago> Pagos { get; set; }
}
```

**Endpoints** (8):
```
GET    /api/facturas
GET    /api/facturas/{id}
GET    /api/facturas/reserva/{idReserva}
GET    /api/facturas/numero/{numeroFactura}
GET    /api/facturas/estado/{estado}
POST   /api/facturas (generar desde reserva)
PUT    /api/facturas/{id}
PATCH  /api/facturas/{id}/estado
```

**Funcionalidad**:
- Generar factura automáticamente desde reserva
- Calcular subtotal desde MontoTotal de reserva
- Aplicar IVA (19%)
- Aplicar descuentos según categoría de cliente
- Calcular total final
- Generar número de factura único (FAC-{año}-{consecutivo})

#### Tarea 2: Formas de Pago (1 hora)
**Archivos a crear**:
```
✅ Models/FormaDePago.cs
✅ Actualizar migración
✅ Services/FormaDePagoService.cs (CRUD simple)
✅ Interfaces/IFormaDePagoService.cs
✅ Repositories/FormaDePagoRepository.cs
✅ DTOs/FormaDePago/
✅ Controllers/FormasDePagoController.cs
✅ Seed en DbInitializer
```

**Modelo FormaDePago**:
```csharp
public class FormaDePago
{
    public int IdFormaPago { get; set; }
    public string Metodo { get; set; } // efectivo, tarjeta_credito, tarjeta_debito, transferencia, pse
    public bool RequiereVerificacion { get; set; }
    public bool Activo { get; set; }
    public string? Descripcion { get; set; }
}
```

**Seed inicial**:
```csharp
new FormaDePago { Metodo = "Efectivo", RequiereVerificacion = false, Activo = true }
new FormaDePago { Metodo = "Tarjeta de Crédito", RequiereVerificacion = true, Activo = true }
new FormaDePago { Metodo = "Tarjeta de Débito", RequiereVerificacion = true, Activo = true }
new FormaDePago { Metodo = "Transferencia Bancaria", RequiereVerificacion = true, Activo = true }
new FormaDePago { Metodo = "PSE", RequiereVerificacion = true, Activo = true }
```

#### Tarea 3: Pagos (3 horas)
**Archivos a crear**:
```
✅ Models/Pago.cs
✅ Actualizar migración
✅ Services/PagoService.cs
✅ Interfaces/IPagoService.cs
✅ Repositories/PagoRepository.cs
✅ DTOs/Pago/ (Create, Response)
✅ Validators/PagoCreateDtoValidator.cs
✅ Controllers/PagosController.cs
```

**Modelo Pago**:
```csharp
public class Pago
{
    public int IdPago { get; set; }
    public int IdFactura { get; set; } // FK → facturas
    public int IdFormaPago { get; set; } // FK → formas_de_pago
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string? ReferenciaTransaccion { get; set; }
    public string? ComprobantePago { get; set; } // URL o base64
    public string Estado { get; set; } // pendiente, aprobado, rechazado
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Factura Factura { get; set; }
    public virtual FormaDePago FormaDePago { get; set; }
}
```

**Endpoints** (7):
```
GET    /api/pagos
GET    /api/pagos/{id}
GET    /api/pagos/factura/{idFactura}
POST   /api/pagos (registrar pago)
PUT    /api/pagos/{id}
PATCH  /api/pagos/{id}/estado
DELETE /api/pagos/{id} (solo si está pendiente)
```

**Funcionalidad**:
- Registrar pago parcial o total
- Actualizar MontoPagado y SaldoPendiente de reserva
- Cambiar estado de factura a "pagada" si total cubierto
- Validar que monto no exceda saldo pendiente
- Múltiples pagos para una misma factura (abonos)

---

### 📆 **SÁBADO** - Día 6 (6 horas)
**Objetivo**: Integración, Testing y Preparación para Frontend

#### Tarea 1: Integración Completa (2 horas)
**Flujo end-to-end**:
```
1. Cliente hace login
2. Ve catálogo (hoteles, vuelos, paquetes)
3. Crea reserva con múltiples servicios
4. Sistema genera factura automáticamente
5. Cliente registra pago
6. Sistema actualiza estados
```

**Testing con Swagger**:
- Probar cada endpoint individualmente
- Probar flujo completo
- Verificar cálculos automáticos
- Verificar relaciones FK
- Probar validaciones

#### Tarea 2: Configurar CORS (30 minutos)
**Archivo**: `Program.cs`

```csharp
// Agregar después de builder.Services.AddControllers()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5500",  // Live Server default
            "http://127.0.0.1:5500",
            "http://localhost:3000",  // Create React App
            "https://tu-frontend.netlify.app" // Producción
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Usar ANTES de app.UseAuthorization()
app.UseCors("AllowFrontend");
```

#### Tarea 3: Crear Documentación para Frontend (2 horas)
**Archivo a crear**: `API_INTEGRATION_GUIDE.md`

Contenido:
1. Introducción a la API
2. Base URL y autenticación
3. Cómo obtener JWT
4. Ejemplos de fetch() para cada endpoint crítico
5. Manejo de errores
6. Estructura de respuestas
7. Códigos de estado HTTP

#### Tarea 4: Crear api.js Helper (1.5 horas)
**Crear en el proyecto**: `Frontend-Examples/api.js`

Contenido:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';

function getToken() { /* ... */ }
async function apiRequest(endpoint, options) { /* ... */ }

const API = {
    auth: {
        login: async (username, password) => { /* ... */ },
        logout: async () => { /* ... */ }
    },
    hoteles: {
        getAll: async () => { /* ... */ },
        getById: async (id) => { /* ... */ }
    },
    vuelos: { /* ... */ },
    paquetes: { /* ... */ },
    reservas: {
        create: async (reservaData) => { /* ... */ },
        getByCliente: async (idCliente) => { /* ... */ }
    },
    facturas: { /* ... */ },
    pagos: { /* ... */ }
};
```

---

### 📆 **DOMINGO** - Día 7 (4 horas)
**Objetivo**: Documentación Final y Deploy

#### Tarea 1: Actualizar Documentación (2 horas)
**Archivos a actualizar**:
```
✅ README.md (principal)
✅ CLAUDE.md (agregar módulos nuevos)
✅ API_INTEGRATION_GUIDE.md (finalizar)
✅ CHANGELOG.md (crear con cambios de la semana)
```

**README.md** debe incluir:
- Descripción del proyecto
- Stack tecnológico
- Prerequisitos
- Instalación (backend)
- Configuración de base de datos
- Ejecutar migraciones
- Ejecutar proyecto
- Endpoints disponibles (resumen)
- Cómo conectar el frontend
- Credenciales de prueba
- Contacto del equipo

#### Tarea 2: Deploy Backend (1.5 horas)
**Opciones gratuitas**:

**Opción A: Railway** (Recomendado)
```bash
# Instalar Railway CLI
npm i -g @railway/cli

# Login
railway login

# Crear proyecto
railway init

# Deploy
railway up

# Agregar MySQL
railway add mysql

# Configurar variables de entorno
railway variables set ConnectionStrings__DefaultConnection="..."
```

**Opción B: Render**
1. Conectar repositorio de GitHub
2. Seleccionar "Web Service"
3. Build Command: `dotnet publish -c Release`
4. Start Command: `dotnet G2rismBeta.API.dll`
5. Agregar MySQL desde Render (o usar CleverCloud gratis)

#### Tarea 3: Video Demo (30 minutos)
**Grabar demo de 5-10 minutos mostrando**:
1. Swagger UI (endpoints disponibles)
2. Flujo completo con Postman:
   - Login
   - Obtener catálogo
   - Crear reserva
   - Generar factura
   - Registrar pago
3. Verificar en base de datos
4. Mostrar respuestas JSON

---

## 🎯 RESULTADO FINAL (Domingo Noche)

### ✅ ENTREGABLES

**Backend Completado**:
- 🏆 24 tablas implementadas de 38 (63%)
- 🏆 ~20 controllers con ~120 endpoints
- 🏆 Sistema funcional end-to-end
- 🏆 Calidad de código: 8.0/10

**Funcionalidad Implementada**:
- ✅ Autenticación JWT completa
- ✅ Gestión de usuarios, roles, permisos
- ✅ CRM (Clientes + Empleados)
- ✅ Gestión de Proveedores
- ✅ Catálogo de Servicios (Vuelos, Hoteles, Paquetes, Servicios Adicionales)
- ✅ Sistema de Reservas completo (con todas las relaciones)
- ✅ Facturación básica
- ✅ Registro de Pagos
- ✅ Cálculo automático de totales

**Documentación**:
- ✅ README.md completo
- ✅ CLAUDE.md actualizado
- ✅ API_INTEGRATION_GUIDE.md para frontend
- ✅ Swagger completamente documentado
- ✅ api.js con ejemplos de consumo

**Deploy**:
- ✅ Backend desplegado (Railway o Render)
- ✅ Base de datos MySQL en la nube
- ✅ URL pública funcionando

### ⏸️ POSPUESTO PARA VERSIÓN 2.0

- ⏸️ Itinerarios detallados (3 tablas)
- ⏸️ Módulo Transporte completo (3 tablas)
- ⏸️ Cotizaciones (2 tablas)
- ⏸️ Notas de Crédito (1 tabla)
- ⏸️ Órdenes de Compra (1 tabla)
- ⏸️ Comunicación (2 tablas)
- ⏸️ Auditoría avanzada (1 tabla)
- ⏸️ Configuración Sistema (1 tabla)
- ⏸️ Tests exhaustivos
- ⏸️ Reportes elaborados

---

## 🎓 APRENDIZAJE ADICIONAL

### Skills que Desarrollarás Esta Semana

**Backend**:
- ✅ Diseño de APIs RESTful
- ✅ Entity Framework Core avanzado (relaciones complejas)
- ✅ Transactions para operaciones atómicas
- ✅ Cálculos de negocio complejos
- ✅ Manejo de JSON en base de datos
- ✅ Optimización de queries

**Frontend (Día 6-7)**:
- ✅ Fetch API
- ✅ Manejo de JWT en cliente
- ✅ LocalStorage
- ✅ Async/await
- ✅ Manejo de errores HTTP
- ✅ CORS troubleshooting

**DevOps**:
- ✅ Deploy de API .NET
- ✅ Configuración de MySQL en la nube
- ✅ Variables de entorno
- ✅ CI/CD básico

---

## 📞 CANALES DE SOPORTE

**Durante la Semana**:
- 💬 Chat actual para dudas puntuales
- 📝 Crear nuevo chat para cada día (recomendado)
- 🔍 Consultar CLAUDE.md ante dudas de arquitectura
- 📊 Revisar diagrama ER ante dudas de relaciones

**Formato de Nuevo Chat**:
```
Título: "🚀 DÍA X - [Módulo] - G2rism MVP"

Primera mensaje:
"Continuando con el plan MVP. Hoy toca implementar [módulo].
Contexto en: PLAN_MVP_SEMANA_FINAL.md
Día: X de 7"
```

---

## 🎯 MÉTRICAS DE ÉXITO

**Al final de la semana, debes poder**:
1. ✅ Mostrar Swagger con ~120 endpoints funcionando
2. ✅ Demostrar flujo completo: Login → Catálogo → Reserva → Factura → Pago
3. ✅ Explicar la arquitectura del proyecto
4. ✅ Consumir la API desde JavaScript básico
5. ✅ Tener el sistema desplegado y accesible online
6. ✅ Entregar documentación para que compañeros conecten frontend

---

## 💪 MOTIVACIÓN FINAL

**Estás a 7 días de tener**:
- 🏆 Un proyecto COMPLETO en tu portafolio
- 🏆 Skills de API REST (alta demanda laboral)
- 🏆 Conocimiento de arquitectura profesional
- 🏆 Experiencia con tecnologías modernas
- 🏆 Sistema funcional que puedes mostrar
- 🏆 Base sólida para seguir escalando

**Lo que tienes hasta ahora es EXCELENTE** (8.5/10).
**Esta semana lo llevas a 9.5/10** (MVP funcional).
**Después puedes llegar a 10/10** (versión 2.0).

---

## 🚀 SIGUIENTE PASO

**Crear nuevo chat con el título**:
```
🚀 DÍA 1 - Módulo Servicios (Vuelos + Hoteles) - G2rism MVP
```

**Primer mensaje en ese chat**:
```
Hola Claude, comenzando el Día 1 del plan MVP.

Objetivo de hoy: Completar Vuelos (tabla existe, falta código) + Implementar Hoteles

Contexto completo en: PLAN_MVP_SEMANA_FINAL.md

Empecemos con Vuelos. ¿Por dónde comenzamos?
```

---

**¿Listo para empezar? 💪🔥**

**Fecha límite**: 2025-12-11 (Domingo)
**Hoy es**: 2025-12-04 (Miércoles)
**Días disponibles**: 7

**¡VAMOS A CONSTRUIR ALGO INCREÍBLE!** 🚀