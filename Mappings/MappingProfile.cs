using AutoMapper;
using G2rismBeta.API.Models;
using G2rismBeta.API.DTOs.Rol;
using G2rismBeta.API.DTOs.Permiso;
using G2rismBeta.API.DTOs.RolPermiso;
using G2rismBeta.API.DTOs.Usuario;
using G2rismBeta.API.DTOs.Auth;
using G2rismBeta.API.DTOs.CategoriaCliente;
using G2rismBeta.API.DTOs.Cliente;
using G2rismBeta.API.DTOs.PreferenciaCliente;
using G2rismBeta.API.DTOs.Empleado;
using G2rismBeta.API.DTOs.Proveedor;
using G2rismBeta.API.DTOs.ContratoProveedor;
using G2rismBeta.API.DTOs.Aerolinea;
using G2rismBeta.API.DTOs.Vuelo;
using G2rismBeta.API.DTOs.Hotel;
using G2rismBeta.API.DTOs.ServicioAdicional;
using G2rismBeta.API.DTOs.PaqueteTuristico;
using G2rismBeta.API.DTOs.Reserva;
using G2rismBeta.API.DTOs.ReservaHotel;
using G2rismBeta.API.DTOs.ReservaVuelo;
using G2rismBeta.API.DTOs.ReservaPaquete;
using G2rismBeta.API.DTOs.ReservaServicio;
using G2rismBeta.API.DTOs.Factura;
using G2rismBeta.API.DTOs.FormaDePago;
using G2rismBeta.API.DTOs.Pago;

namespace G2rismBeta.API.Mappings;

/// <summary>
/// Perfil de mapeo de AutoMapper
/// Define cómo convertir entre Modelos y DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ========================================
        // MAPEOS PARA ROL
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<RolCreateDto, Rol>()
            .ForMember(dest => dest.IdRol, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.RolesPermisos, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<RolUpdateDto, Rol>()
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // No se modifica
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.RolesPermisos, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<Rol, RolResponseDto>()
            .ForMember(dest => dest.CantidadPermisos, opt => opt.MapFrom(src => src.RolesPermisos.Count));

        // ========================================
        // MAPEOS PARA PERMISO
        // ========================================

        // CreateDto → Modelo
        CreateMap<PermisoCreateDto, Permiso>()
            .ForMember(dest => dest.IdPermiso, opt => opt.Ignore())
            .ForMember(dest => dest.NombrePermiso, opt => opt.Ignore()) // Se genera en el servicio
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.RolesPermisos, opt => opt.Ignore());

        // UpdateDto → Modelo (soporta actualizaciones parciales)
        CreateMap<PermisoUpdateDto, Permiso>()
            .ForMember(dest => dest.IdPermiso, opt => opt.Ignore())
            .ForMember(dest => dest.NombrePermiso, opt => opt.Ignore()) // Se regenera en el servicio
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.RolesPermisos, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto
        CreateMap<Permiso, PermisoResponseDto>()
            .ForMember(dest => dest.CantidadRoles, opt => opt.MapFrom(src => src.RolesPermisos.Count));

        // ========================================
        // MAPEOS PARA ROL CON PERMISOS
        // ========================================

        // Rol → RolConPermisosDto (incluye la lista de permisos)
        CreateMap<Rol, RolConPermisosDto>()
            .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src =>
                src.RolesPermisos.Select(rp => rp.Permiso).ToList()));

        // ========================================
        // MAPEOS PARA USUARIO
        // ========================================

        // CreateDto → Modelo (para crear)
        // NOTA: El password NO se mapea aquí, se hashea en el Service antes de crear
        CreateMap<UsuarioCreateDto, Usuario>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se maneja en el Service
            .ForMember(dest => dest.UltimoAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.IntentosFallidos, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Bloqueado, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.UsuariosRoles, opt => opt.Ignore())
            .ForMember(dest => dest.TokensRecuperacion, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        // Solo mapea los campos que vienen en el DTO (pueden ser null)
        CreateMap<UsuarioUpdateDto, Usuario>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.UltimoAcceso, opt => opt.Ignore())
            .ForMember(dest => dest.IntentosFallidos, opt => opt.Ignore())
            .ForMember(dest => dest.Bloqueado, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.UsuariosRoles, opt => opt.Ignore())
            .ForMember(dest => dest.TokensRecuperacion, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver - sin datos sensibles)
        CreateMap<Usuario, UsuarioResponseDto>();

        // Modelo → UsuarioConRolesDto (incluye roles)
        CreateMap<Usuario, UsuarioConRolesDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.UsuariosRoles.Select(ur => ur.Rol).ToList()));

        // Modelo → UsuarioLoginDto (para respuesta de login)
        CreateMap<Usuario, UsuarioLoginDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                src.UsuariosRoles.Select(ur => ur.Rol.Nombre).ToList()))
            .ForMember(dest => dest.Permisos, opt => opt.MapFrom(src =>
                src.UsuariosRoles
                    .SelectMany(ur => ur.Rol.RolesPermisos)
                    .Select(rp => rp.Permiso.NombrePermiso)
                    .Distinct()
                    .ToList()));

        // ========================================
        // MAPEOS PARA AUTH (LOGIN RESPONSE)
        // ========================================

        // Usuario → LoginResponseDto
        CreateMap<Usuario, LoginResponseDto>()
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => $"¡Bienvenido de vuelta, {src.Username}!"))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.TokenExpiration, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.FechaLogin, opt => opt.MapFrom(src => DateTime.Now));

        // ========================================
        // MAPEOS PARA CATEGORIA CLIENTE (CRM)
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<CategoriaClienteCreateDto, CategoriaCliente>()
            .ForMember(dest => dest.IdCategoria, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.Clientes, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<CategoriaClienteUpdateDto, CategoriaCliente>()
            .ForMember(dest => dest.Clientes, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<CategoriaCliente, CategoriaClienteResponseDto>()
            .ForMember(dest => dest.CantidadClientes, opt => opt.Ignore()); // Se calcula en el Service

        // ========================================
        // MAPEOS PARA CLIENTE (CRM)
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ClienteCreateDto, Cliente>()
            .ForMember(dest => dest.IdCliente, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Preferencia, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<ClienteUpdateDto, Cliente>()
            .ForMember(dest => dest.IdCliente, opt => opt.Ignore())
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore()) // No se puede cambiar el usuario
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore()) // No se modifica
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Categoria, opt => opt.Ignore())
            .ForMember(dest => dest.Preferencia, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<Cliente, ClienteResponseDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
            .ForMember(dest => dest.Edad, opt => opt.MapFrom(src => src.Edad))
            .ForMember(dest => dest.NombreCategoria, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : null))
            .ForMember(dest => dest.DescuentoCategoria, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.DescuentoPorcentaje : 0));

        // Modelo → ClienteConCategoriaDto (incluye detalles completos de categoría)
        CreateMap<Cliente, ClienteConCategoriaDto>()
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.Categoria != null ? new ClienteConCategoriaDto.CategoriaInfo
            {
                IdCategoria = src.Categoria.IdCategoria,
                Nombre = src.Categoria.Nombre,
                ColorHex = src.Categoria.ColorHex,
                DescuentoPorcentaje = src.Categoria.DescuentoPorcentaje,
                Beneficios = src.Categoria.Beneficios
            } : null));

        // CategoriaCliente → ClienteConCategoriaDto.CategoriaInfo (mapeo auxiliar)
        CreateMap<CategoriaCliente, ClienteConCategoriaDto.CategoriaInfo>();

        // ========================================
        // MAPEOS PARA PREFERENCIA CLIENTE (CRM)
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<PreferenciaClienteCreateDto, PreferenciaCliente>()
            .ForMember(dest => dest.IdPreferencia, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Intereses, opt => opt.Ignore()) // Se maneja en el servicio (conversión a JSON)
            .ForMember(dest => dest.Cliente, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - soporta actualizaciones parciales)
        CreateMap<PreferenciaClienteUpdateDto, PreferenciaCliente>()
            .ForMember(dest => dest.IdPreferencia, opt => opt.Ignore())
            .ForMember(dest => dest.IdCliente, opt => opt.Ignore()) // No se puede cambiar el cliente
            .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Intereses, opt => opt.Ignore()) // Se maneja en el servicio (conversión a JSON)
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Solo actualiza campos no nulos

        // Modelo → ResponseDto (para devolver)
        CreateMap<PreferenciaCliente, PreferenciaClienteResponseDto>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src =>
                src.Cliente != null ? $"{src.Cliente.Nombre} {src.Cliente.Apellido}" : null))
            .ForMember(dest => dest.Intereses, opt => opt.MapFrom(src =>
                DeserializeIntereses(src.Intereses)));

        // ========================================
        // MAPEOS PARA EMPLEADO (CRM)
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<EmpleadoCreateDto, Empleado>()
            .ForMember(dest => dest.IdEmpleado, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Jefe, opt => opt.Ignore())
            .ForMember(dest => dest.Subordinados, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<EmpleadoUpdateDto, Empleado>()
            .ForMember(dest => dest.IdEmpleado, opt => opt.Ignore())
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore()) // No se puede cambiar el usuario asociado
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Jefe, opt => opt.Ignore())
            .ForMember(dest => dest.Subordinados, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<Empleado, EmpleadoResponseDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
            .ForMember(dest => dest.Edad, opt => opt.MapFrom(src => src.Edad))
            .ForMember(dest => dest.AntiguedadAnios, opt => opt.MapFrom(src => src.AntiguedadAnios))
            .ForMember(dest => dest.AntiguedadMeses, opt => opt.MapFrom(src => src.AntiguedadMeses))
            .ForMember(dest => dest.EsJefe, opt => opt.MapFrom(src => src.EsJefe))
            .ForMember(dest => dest.CantidadSubordinados, opt => opt.MapFrom(src => src.CantidadSubordinados))
            .ForMember(dest => dest.Jefe, opt => opt.MapFrom(src => src.Jefe != null ? new JefeBasicInfoDto
            {
                IdEmpleado = src.Jefe.IdEmpleado,
                NombreCompleto = src.Jefe.NombreCompleto,
                Cargo = src.Jefe.Cargo
            } : null))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Username : null))
            // IMPORTANTE: El salario puede ser null si el usuario no tiene permisos
            // El servicio decide si incluirlo o no según permisos
            .ForMember(dest => dest.Salario, opt => opt.MapFrom(src => src.Salario));

        // Mapeo de Empleado a JefeBasicInfoDto (para DTO anidados)
        CreateMap<Empleado, JefeBasicInfoDto>()
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto));

        // ========================================
        // MAPEOS PARA PROVEEDOR
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ProveedorCreateDto, Proveedor>()
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Contratos, opt => opt.Ignore()); // Relación 1:N

        // UpdateDto → Modelo (para actualizar)
        CreateMap<ProveedorUpdateDto, Proveedor>()
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore())
            .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore()) // No se modifica
            .ForMember(dest => dest.Contratos, opt => opt.Ignore()); // Relación 1:N

        // Modelo → ResponseDto (para devolver)
        CreateMap<Proveedor, ProveedorResponseDto>()
            .ForMember(dest => dest.ContratosActivos, opt => opt.Ignore()) // Se calcula en el Service
            .ForMember(dest => dest.TieneContratosVigentes, opt => opt.Ignore()); // Se calcula en el Service

        // ========================================
        // MAPEOS PARA CONTRATO PROVEEDOR
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ContratoProveedorCreateDto, ContratoProveedor>()
            .ForMember(dest => dest.IdContrato, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<ContratoProveedorUpdateDto, ContratoProveedor>()
            .ForMember(dest => dest.IdContrato, opt => opt.Ignore())
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore()) // No se puede cambiar el proveedor
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // No se modifica
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<ContratoProveedor, ContratoProveedorResponseDto>()
            .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src =>
                src.Proveedor != null ? src.Proveedor.NombreEmpresa : string.Empty))
            .ForMember(dest => dest.EstaVigente, opt => opt.MapFrom(src => src.EstaVigente))
            .ForMember(dest => dest.DiasRestantes, opt => opt.MapFrom(src => src.DiasRestantes))
            .ForMember(dest => dest.ProximoAVencer, opt => opt.MapFrom(src => src.ProximoAVencer))
            .ForMember(dest => dest.DuracionDias, opt => opt.MapFrom(src => src.DuracionDias));

        // ========================================
        // MAPEOS PARA AEROLINEA
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<AerolineaCreateDto, Aerolinea>()
            .ForMember(dest => dest.IdAerolinea, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Vuelos, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<AerolineaUpdateDto, Aerolinea>()
            .ForMember(dest => dest.IdAerolinea, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Vuelos, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Aerolinea, AerolineaResponseDto>()
            .ForMember(dest => dest.EstaActiva, opt => opt.MapFrom(src => src.EstaActiva))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
            .ForMember(dest => dest.TienePoliticasEquipaje, opt => opt.MapFrom(src => src.TienePoliticasEquipaje));

        // ========================================
        // MAPEOS PARA VUELO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<VueloCreateDto, Vuelo>()
            .ForMember(dest => dest.IdVuelo, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.Aerolinea, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - solo campos no nulos)
        CreateMap<VueloUpdateDto, Vuelo>()
            .ForMember(dest => dest.IdVuelo, opt => opt.Ignore())
            .ForMember(dest => dest.IdAerolinea, opt => opt.Ignore())
            .ForMember(dest => dest.IdProveedor, opt => opt.Ignore())
            .ForMember(dest => dest.Aerolinea, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Vuelo, VueloResponseDto>()
            .ForMember(dest => dest.NombreAerolinea, opt => opt.MapFrom(src => src.Aerolinea != null ? src.Aerolinea.Nombre : null))
            .ForMember(dest => dest.CodigoIataAerolinea, opt => opt.MapFrom(src => src.Aerolinea != null ? src.Aerolinea.CodigoIata : null))
            .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.NombreEmpresa : null))
            .ForMember(dest => dest.TieneDisponibilidad, opt => opt.MapFrom(src => src.TieneDisponibilidad))
            .ForMember(dest => dest.EsVueloDirecto, opt => opt.MapFrom(src => src.EsVueloDirecto))
            .ForMember(dest => dest.DuracionFormateada, opt => opt.MapFrom(src => src.DuracionFormateada))
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => src.EstaActivo));

        // ========================================
        // MAPEOS PARA HOTEL
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<HotelCreateDto, Hotel>()
            .ForMember(dest => dest.IdHotel, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.CheckInHora, opt => opt.Ignore()) // Se mapea manualmente en el service
            .ForMember(dest => dest.CheckOutHora, opt => opt.Ignore()) // Se mapea manualmente en el service
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - solo campos no nulos)
        CreateMap<HotelUpdateDto, Hotel>()
            .ForMember(dest => dest.IdHotel, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.CheckInHora, opt => opt.Ignore()) // Se mapea manualmente en el service
            .ForMember(dest => dest.CheckOutHora, opt => opt.Ignore()) // Se mapea manualmente en el service
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore())
            .ForMember(dest => dest.IdProveedor, opt => opt.Condition(src => src.IdProveedor.HasValue && src.IdProveedor.Value > 0))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
            {
                // Si es un tipo nullable, verificar si tiene valor
                if (srcMember == null) return false;

                // Permitir actualización de booleanos nullable incluso si son false
                var srcType = srcMember.GetType();
                if (srcType == typeof(bool?) && srcMember != null) return true;

                return true;
            }));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Hotel, HotelResponseDto>()
            .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.NombreEmpresa : string.Empty))
            .ForMember(dest => dest.CheckInHora, opt => opt.MapFrom(src => src.CheckInHora.HasValue ? src.CheckInHora.Value.ToString(@"hh\:mm") : null))
            .ForMember(dest => dest.CheckOutHora, opt => opt.MapFrom(src => src.CheckOutHora.HasValue ? src.CheckOutHora.Value.ToString(@"hh\:mm") : null))
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => src.EstaActivo))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
            .ForMember(dest => dest.TieneServiciosPremium, opt => opt.MapFrom(src => src.TieneServiciosPremium))
            .ForMember(dest => dest.ClasificacionTexto, opt => opt.MapFrom(src => src.ClasificacionTexto));

        // ========================================
        // MAPEOS PARA SERVICIO ADICIONAL
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ServicioAdicionalCreateDto, ServicioAdicional>()
            .ForMember(dest => dest.IdServicio, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.TiempoEstimado, opt => opt.Ignore()) // Se convierte manualmente en el service (string → int)
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se asigna en el service
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - solo actualiza campos no nulos)
        CreateMap<ServicioAdicionalUpdateDto, ServicioAdicional>()
            .ForMember(dest => dest.IdServicio, opt => opt.Ignore())
            .ForMember(dest => dest.TiempoEstimado, opt => opt.Ignore()) // Se convierte manualmente en el service (string → int)
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore()) // Se asigna en el service
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore())
            // Condiciones explícitas para tipos de valor nullable (evitar que 0/false se mapeen cuando no se envían)
            .ForMember(dest => dest.IdProveedor, opt => opt.Condition(src => src.IdProveedor.HasValue && src.IdProveedor.Value > 0))
            .ForMember(dest => dest.Precio, opt => opt.Condition(src => src.Precio.HasValue))
            .ForMember(dest => dest.Disponibilidad, opt => opt.Condition(src => src.Disponibilidad.HasValue))
            .ForMember(dest => dest.CapacidadMaxima, opt => opt.Condition(src => src.CapacidadMaxima.HasValue))
            .ForMember(dest => dest.EdadMinima, opt => opt.Condition(src => src.EdadMinima.HasValue))
            .ForMember(dest => dest.Estado, opt => opt.Condition(src => src.Estado.HasValue))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
            {
                // Si es null, no mapear
                if (srcMember == null) return false;

                // Los tipos de valor nullable ya están controlados explícitamente arriba
                return true;
            }));

        // Modelo → ResponseDto (para devolver)
        CreateMap<ServicioAdicional, ServicioAdicionalResponseDto>()
            .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.NombreEmpresa : string.Empty))
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => src.EstaActivo))
            .ForMember(dest => dest.EstaDisponible, opt => opt.MapFrom(src => src.EstaDisponible))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
            .ForMember(dest => dest.PrecioFormateado, opt => opt.MapFrom(src => src.PrecioFormateado))
            .ForMember(dest => dest.DuracionFormateada, opt => opt.MapFrom(src => src.DuracionFormateada));

        // ========================================
        // MAPEOS PARA PAQUETE TURÍSTICO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<PaqueteTuristicoCreateDto, PaqueteTuristico>()
            .ForMember(dest => dest.IdPaquete, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se asigna en el service
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - NO SE USA, se hace mapeo manual en el service)
        // Se usa asignación manual campo por campo en PaqueteTuristicoService.UpdateAsync para mayor control
        CreateMap<PaqueteTuristicoUpdateDto, PaqueteTuristico>()
            .ForMember(dest => dest.IdPaquete, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore()); // Se asigna en el service

        // Modelo → ResponseDto (para devolver)
        CreateMap<PaqueteTuristico, PaqueteTuristicoResponseDto>()
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => src.EstaActivo))
            .ForMember(dest => dest.EstaDisponible, opt => opt.MapFrom(src => src.EstaDisponible))
            .ForMember(dest => dest.TieneFechasDefinidas, opt => opt.MapFrom(src => src.TieneFechasDefinidas))
            .ForMember(dest => dest.EstaVigente, opt => opt.MapFrom(src => src.EstaVigente))
            .ForMember(dest => dest.ProximoAIniciar, opt => opt.MapFrom(src => src.ProximoAIniciar))
            .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto))
            .ForMember(dest => dest.DuracionFormateada, opt => opt.MapFrom(src => src.DuracionFormateada))
            .ForMember(dest => dest.PrecioFormateado, opt => opt.MapFrom(src => src.PrecioFormateado))
            .ForMember(dest => dest.EstadoDisponibilidad, opt => opt.MapFrom(src => src.EstadoDisponibilidad));

        // ========================================
        // MAPEOS PARA RESERVA
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ReservaCreateDto, Reserva>()
            .ForMember(dest => dest.IdReserva, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.MontoTotal, opt => opt.MapFrom(src => 0)) // Se calcula al agregar servicios
            .ForMember(dest => dest.MontoPagado, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.SaldoPendiente, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.FechaHora, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Empleado, opt => opt.Ignore());
            // TODO: Descomentar en Día 3 Tarea 2-4
            // .ForMember(dest => dest.ReservasHoteles, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasVuelos, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasPaquetes, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasServicios, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - soporta actualizaciones parciales)
        CreateMap<ReservaUpdateDto, Reserva>()
            .ForMember(dest => dest.IdReserva, opt => opt.Ignore())
            .ForMember(dest => dest.IdCliente, opt => opt.Ignore()) // No se puede cambiar el cliente
            .ForMember(dest => dest.MontoTotal, opt => opt.Ignore()) // Se calcula automáticamente
            .ForMember(dest => dest.MontoPagado, opt => opt.Ignore()) // Se actualiza con pagos
            .ForMember(dest => dest.SaldoPendiente, opt => opt.Ignore()) // Se calcula automáticamente
            .ForMember(dest => dest.FechaHora, opt => opt.Ignore()) // No se modifica
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore())
            .ForMember(dest => dest.Empleado, opt => opt.Ignore())
            // TODO: Descomentar en Día 3 Tarea 2-4
            // .ForMember(dest => dest.ReservasHoteles, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasVuelos, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasPaquetes, opt => opt.Ignore())
            // .ForMember(dest => dest.ReservasServicios, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Reserva, ReservaResponseDto>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src =>
                src.Cliente != null ? $"{src.Cliente.Nombre} {src.Cliente.Apellido}" : ""))
            .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src =>
                src.Empleado != null ? $"{src.Empleado.Nombre} {src.Empleado.Apellido}" : ""))
            .ForMember(dest => dest.DuracionDias, opt => opt.MapFrom(src => src.DuracionDias))
            .ForMember(dest => dest.PorcentajePagado, opt => opt.MapFrom(src => src.PorcentajePagado))
            .ForMember(dest => dest.EstaPagada, opt => opt.MapFrom(src => src.EstaPagada))
            .ForMember(dest => dest.TieneSaldoPendiente, opt => opt.MapFrom(src => src.TieneSaldoPendiente))
            .ForMember(dest => dest.ViajeIniciado, opt => opt.MapFrom(src => src.ViajeIniciado))
            .ForMember(dest => dest.ViajeCompleto, opt => opt.MapFrom(src => src.ViajeCompleto))
            .ForMember(dest => dest.DiasHastaViaje, opt => opt.MapFrom(src => src.DiasHastaViaje));

        // ========================================
        // MAPEOS PARA RESERVA_HOTEL
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ReservaHotelCreateDto, ReservaHotel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.IdReserva, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.PrecioPorNoche, opt => opt.Ignore()) // Se obtiene del hotel
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Hotel, opt => opt.Ignore()); // Navegación

        // Modelo → ResponseDto (para devolver)
        CreateMap<ReservaHotel, ReservaHotelResponseDto>()
            .ForMember(dest => dest.NombreHotel, opt => opt.MapFrom(src =>
                src.Hotel != null ? src.Hotel.Nombre : "N/A"))
            .ForMember(dest => dest.CiudadHotel, opt => opt.MapFrom(src =>
                src.Hotel != null ? src.Hotel.Ciudad : "N/A"))
            .ForMember(dest => dest.NumeroNoches, opt => opt.MapFrom(src => src.NumeroNoches))
            .ForMember(dest => dest.CostoPorHabitacion, opt => opt.MapFrom(src => src.CostoPorHabitacion))
            .ForMember(dest => dest.EstadiaActiva, opt => opt.MapFrom(src => src.EstadiaActiva))
            .ForMember(dest => dest.DiasHastaCheckin, opt => opt.MapFrom(src => src.DiasHastaCheckin));

        // ========================================
        // MAPEOS PARA RESERVA_VUELO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ReservaVueloCreateDto, ReservaVuelo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.PrecioPorPasajero, opt => opt.Ignore()) // Se calcula según clase
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.FechaAgregado, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Vuelo, opt => opt.Ignore()); // Navegación

        // Modelo → ResponseDto (para devolver)
        CreateMap<ReservaVuelo, ReservaVueloResponseDto>()
            .ForMember(dest => dest.NumeroVuelo, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.NumeroVuelo : null))
            .ForMember(dest => dest.Origen, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.Origen : null))
            .ForMember(dest => dest.Destino, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.Destino : null))
            .ForMember(dest => dest.FechaSalida, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.FechaSalida : (DateTime?)null))
            .ForMember(dest => dest.HoraSalida, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.HoraSalida : (TimeSpan?)null))
            .ForMember(dest => dest.FechaLlegada, opt => opt.MapFrom(src =>
                src.Vuelo != null ? src.Vuelo.FechaLlegada : (DateTime?)null))
            .ForMember(dest => dest.NombreAerolinea, opt => opt.MapFrom(src =>
                src.Vuelo != null && src.Vuelo.Aerolinea != null ? src.Vuelo.Aerolinea.Nombre : null))
            .ForMember(dest => dest.CostoTotal, opt => opt.MapFrom(src => src.CostoTotal))
            .ForMember(dest => dest.EsClaseEjecutiva, opt => opt.MapFrom(src => src.EsClaseEjecutiva))
            .ForMember(dest => dest.TieneEquipajeExtra, opt => opt.MapFrom(src => src.TieneEquipajeExtra));

        // ========================================
        // MAPEOS PARA RESERVA_PAQUETE
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ReservaPaqueteCreateDto, ReservaPaquete>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.IdReserva, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.PrecioPorPersona, opt => opt.Ignore()) // Se obtiene del paquete
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.FechaAgregado, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Paquete, opt => opt.Ignore()); // Navegación

        // Modelo → ResponseDto (para devolver)
        CreateMap<ReservaPaquete, ReservaPaqueteResponseDto>()
            .ForMember(dest => dest.NombrePaquete, opt => opt.MapFrom(src =>
                src.Paquete != null ? src.Paquete.Nombre : null))
            .ForMember(dest => dest.DestinoPrincipal, opt => opt.MapFrom(src =>
                src.Paquete != null ? src.Paquete.DestinoPrincipal : null))
            .ForMember(dest => dest.DuracionDias, opt => opt.MapFrom(src => src.DuracionDias))
            .ForMember(dest => dest.SubtotalFormateado, opt => opt.MapFrom(src => src.SubtotalFormateado))
            .ForMember(dest => dest.PaqueteIniciado, opt => opt.MapFrom(src => src.PaqueteIniciado))
            .ForMember(dest => dest.PaqueteCompletado, opt => opt.MapFrom(src => src.PaqueteCompletado))
            .ForMember(dest => dest.DiasHastaInicio, opt => opt.MapFrom(src => src.DiasHastaInicio));

        // ========================================
        // MAPEOS PARA RESERVA_SERVICIO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<ReservaServicioCreateDto, ReservaServicio>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.PrecioUnitario, opt => opt.Ignore()) // Se obtiene del servicio
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.FechaAgregado, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Servicio, opt => opt.Ignore()); // Navegación

        // Modelo → ResponseDto (para devolver)
        CreateMap<ReservaServicio, ReservaServicioResponseDto>()
            .ForMember(dest => dest.NombreServicio, opt => opt.MapFrom(src =>
                src.Servicio != null ? src.Servicio.Nombre : null))
            .ForMember(dest => dest.TipoServicio, opt => opt.MapFrom(src =>
                src.Servicio != null ? src.Servicio.Tipo : null))
            .ForMember(dest => dest.UnidadServicio, opt => opt.MapFrom(src =>
                src.Servicio != null ? src.Servicio.Unidad : null))
            .ForMember(dest => dest.EstaConfirmado, opt => opt.MapFrom(src => src.EstaConfirmado))
            .ForMember(dest => dest.EstaCompletado, opt => opt.MapFrom(src => src.EstaCompletado))
            .ForMember(dest => dest.EstaCancelado, opt => opt.MapFrom(src => src.EstaCancelado))
            .ForMember(dest => dest.ServicioEjecutado, opt => opt.MapFrom(src => src.ServicioEjecutado))
            .ForMember(dest => dest.DiasHastaServicio, opt => opt.MapFrom(src => src.DiasHastaServicio));

        // ========================================
        // MAPEOS PARA FACTURA
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<FacturaCreateDto, Factura>()
            .ForMember(dest => dest.IdFactura, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.NumeroFactura, opt => opt.Ignore()) // Se genera en el servicio
            .ForMember(dest => dest.FechaEmision, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.CufeCude, opt => opt.Ignore()) // Se genera después si es necesario
            .ForMember(dest => dest.TipoFactura, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Estado, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.Impuestos, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.Descuentos, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.Total, opt => opt.Ignore()) // Se calcula en el servicio
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Pagos, opt => opt.Ignore()); // Navegación

        // UpdateDto → Modelo (para actualizar - soporta actualizaciones parciales)
        CreateMap<FacturaUpdateDto, Factura>()
            .ForMember(dest => dest.IdFactura, opt => opt.Ignore())
            .ForMember(dest => dest.IdReserva, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.NumeroFactura, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.FechaEmision, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.TipoFactura, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.Impuestos, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.Descuentos, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.Total, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.PorcentajeIva, opt => opt.Ignore()) // No se puede cambiar
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Reserva, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.Pagos, opt => opt.Ignore()) // Navegación
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Factura, FacturaResponseDto>()
            // Propiedades básicas
            .ForMember(dest => dest.IdFactura, opt => opt.MapFrom(src => src.IdFactura))
            .ForMember(dest => dest.IdReserva, opt => opt.MapFrom(src => src.IdReserva))
            .ForMember(dest => dest.NumeroFactura, opt => opt.MapFrom(src => src.NumeroFactura))
            .ForMember(dest => dest.FechaEmision, opt => opt.MapFrom(src => src.FechaEmision))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento))
            .ForMember(dest => dest.ResolucionDian, opt => opt.MapFrom(src => src.ResolucionDian))
            .ForMember(dest => dest.CufeCude, opt => opt.MapFrom(src => src.CufeCude))
            .ForMember(dest => dest.TipoFactura, opt => opt.MapFrom(src => src.TipoFactura))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
            .ForMember(dest => dest.Impuestos, opt => opt.MapFrom(src => src.Impuestos))
            .ForMember(dest => dest.PorcentajeIva, opt => opt.MapFrom(src => src.PorcentajeIva))
            .ForMember(dest => dest.Descuentos, opt => opt.MapFrom(src => src.Descuentos))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Observaciones, opt => opt.MapFrom(src => src.Observaciones))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))
            .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaModificacion))
            // Propiedades computadas
            .ForMember(dest => dest.EstaPagada, opt => opt.MapFrom(src => src.EstaPagada))
            .ForMember(dest => dest.EstaPendiente, opt => opt.MapFrom(src => src.EstaPendiente))
            .ForMember(dest => dest.EstaCancelada, opt => opt.MapFrom(src => src.EstaCancelada))
            .ForMember(dest => dest.EstaVencida, opt => opt.MapFrom(src => src.EstaVencida))
            .ForMember(dest => dest.DiasHastaVencimiento, opt => opt.MapFrom(src => src.DiasHastaVencimiento))
            .ForMember(dest => dest.MontoPagado, opt => opt.MapFrom(src => src.MontoPagado))
            .ForMember(dest => dest.SaldoPendiente, opt => opt.MapFrom(src => src.SaldoPendiente))
            .ForMember(dest => dest.PorcentajePagado, opt => opt.MapFrom(src => src.PorcentajePagado))
            .ForMember(dest => dest.TienePagosParciales, opt => opt.MapFrom(src => src.TienePagosParciales))
            .ForMember(dest => dest.BaseGravable, opt => opt.MapFrom(src => src.BaseGravable))
            // Información relacionada (opcional, se mapea manualmente si es necesario)
            .ForMember(dest => dest.Reserva, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore());

        // Mapeo para información básica de reserva (para incluir en FacturaResponseDto)
        CreateMap<Reserva, ReservaBasicInfoDto>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src =>
                src.Cliente != null ? $"{src.Cliente.Nombre} {src.Cliente.Apellido}" : null));

        // ========================================
        // MAPEOS PARA FORMA_DE_PAGO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<FormaDePagoCreateDto, FormaDePago>()
            .ForMember(dest => dest.IdFormaPago, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore()); // Navegación

        // UpdateDto → Modelo (para actualizar - soporta actualizaciones parciales)
        CreateMap<FormaDePagoUpdateDto, FormaDePago>()
            .ForMember(dest => dest.IdFormaPago, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<FormaDePago, FormaDePagoResponseDto>()
            .ForMember(dest => dest.EsMetodoElectronico, opt => opt.MapFrom(src => src.EsMetodoElectronico))
            .ForMember(dest => dest.EsEfectivo, opt => opt.MapFrom(src => src.EsEfectivo));

        // ========================================
        // MAPEOS PARA PAGO
        // ========================================

        // CreateDto → Modelo (para crear)
        CreateMap<PagoCreateDto, Pago>()
            .ForMember(dest => dest.IdPago, opt => opt.Ignore()) // El ID lo genera la BD
            .ForMember(dest => dest.FechaPago, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Estado, opt => opt.Ignore()) // Se normaliza en el servicio
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Factura, opt => opt.Ignore()) // Navegación
            .ForMember(dest => dest.FormaDePago, opt => opt.Ignore()); // Navegación

        // UpdateDto → Modelo (para actualizar - soporta actualizaciones parciales)
        CreateMap<PagoUpdateDto, Pago>()
            .ForMember(dest => dest.IdPago, opt => opt.Ignore())
            .ForMember(dest => dest.IdFactura, opt => opt.Ignore()) // No se puede cambiar la factura
            .ForMember(dest => dest.IdFormaPago, opt => opt.Ignore()) // No se puede cambiar la forma de pago
            .ForMember(dest => dest.FechaPago, opt => opt.Ignore()) // No se puede cambiar la fecha de pago
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore()) // Se establece en el servicio
            .ForMember(dest => dest.Factura, opt => opt.Ignore())
            .ForMember(dest => dest.FormaDePago, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Modelo → ResponseDto (para devolver)
        CreateMap<Pago, PagoResponseDto>()
            .ForMember(dest => dest.NumeroFactura, opt => opt.MapFrom(src => src.Factura != null ? src.Factura.NumeroFactura : null))
            .ForMember(dest => dest.MetodoPago, opt => opt.MapFrom(src => src.FormaDePago != null ? src.FormaDePago.Metodo : null))
            .ForMember(dest => dest.EstaAprobado, opt => opt.MapFrom(src => src.EstaAprobado))
            .ForMember(dest => dest.EstaPendiente, opt => opt.MapFrom(src => src.EstaPendiente))
            .ForMember(dest => dest.EstaRechazado, opt => opt.MapFrom(src => src.EstaRechazado))
            .ForMember(dest => dest.DiasDesdeElPago, opt => opt.MapFrom(src => src.DiasDesdeElPago))
            .ForMember(dest => dest.TieneComprobante, opt => opt.MapFrom(src => src.TieneComprobante));
    }

    // ========================================
    // MÉTODOS AUXILIARES PARA MAPEOS
    // ========================================

    /// <summary>
    /// Deserializa el campo JSON Intereses del modelo a una lista de strings
    /// </summary>
    private static List<string>? DeserializeIntereses(string? interesesJson)
    {
        if (string.IsNullOrEmpty(interesesJson))
        {
            return null;
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(interesesJson);
        }
        catch
        {
            return null;
        }
    }
}
