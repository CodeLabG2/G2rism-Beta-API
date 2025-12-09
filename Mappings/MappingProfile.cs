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
            .ForMember(dest => dest.Cliente, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar)
        CreateMap<PreferenciaClienteUpdateDto, PreferenciaCliente>()
            .ForMember(dest => dest.IdPreferencia, opt => opt.Ignore())
            .ForMember(dest => dest.IdCliente, opt => opt.Ignore()) // No se puede cambiar el cliente
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Cliente, opt => opt.Ignore());

        // Modelo → ResponseDto (para devolver)
        CreateMap<PreferenciaCliente, PreferenciaClienteResponseDto>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src =>
                src.Cliente != null ? $"{src.Cliente.Nombre} {src.Cliente.Apellido}" : null));

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
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore()) // Se asigna en el service
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore());

        // UpdateDto → Modelo (para actualizar - solo actualiza campos no nulos)
        CreateMap<ServicioAdicionalUpdateDto, ServicioAdicional>()
            .ForMember(dest => dest.IdServicio, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore()) // Se asigna en el service
            .ForMember(dest => dest.Proveedor, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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

        // UpdateDto → Modelo (para actualizar - solo actualiza campos no nulos)
        CreateMap<PaqueteTuristicoUpdateDto, PaqueteTuristico>()
            .ForMember(dest => dest.IdPaquete, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore()) // Se asigna en el service
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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
    }
}
