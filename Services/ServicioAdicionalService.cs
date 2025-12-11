using AutoMapper;
using G2rismBeta.API.DTOs.ServicioAdicional;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using Microsoft.Extensions.Logging;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio de servicios adicionales con lógica de negocio y validaciones
/// </summary>
public class ServicioAdicionalService : IServicioAdicionalService
{
    private readonly IServicioAdicionalRepository _servicioRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ServicioAdicionalService> _logger;

    public ServicioAdicionalService(
        IServicioAdicionalRepository servicioRepository,
        IProveedorRepository proveedorRepository,
        IMapper mapper,
        ILogger<ServicioAdicionalService> logger)
    {
        _servicioRepository = servicioRepository;
        _proveedorRepository = proveedorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetAllAsync()
    {
        _logger.LogInformation("🎯 Obteniendo todos los servicios adicionales");
        var servicios = await _servicioRepository.GetAllConProveedorAsync();
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<ServicioAdicionalResponseDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("🔍 Buscando servicio adicional con ID: {Id}", id);

        var servicio = await _servicioRepository.GetByIdConProveedorAsync(id);
        if (servicio == null)
        {
            _logger.LogWarning("⚠️ Servicio adicional con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"Servicio adicional con ID {id} no encontrado");
        }

        return _mapper.Map<ServicioAdicionalResponseDto>(servicio);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetByTipoAsync(string tipo)
    {
        var tiposValidos = new[] { "tour", "guia", "actividad", "transporte_interno" };
        if (!tiposValidos.Contains(tipo.ToLower()))
        {
            _logger.LogWarning("⚠️ Tipo de servicio inválido: {Tipo}", tipo);
            throw new ArgumentException(
                $"Tipo de servicio inválido. Valores permitidos: {string.Join(", ", tiposValidos)}",
                nameof(tipo));
        }

        _logger.LogInformation("🔍 Buscando servicios de tipo: {Tipo}", tipo);
        var servicios = await _servicioRepository.GetByTipoAsync(tipo);
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetDisponiblesAsync()
    {
        _logger.LogInformation("✅ Obteniendo servicios disponibles");
        var servicios = await _servicioRepository.GetDisponiblesAsync();
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetActivosAsync()
    {
        _logger.LogInformation("🟢 Obteniendo servicios activos");
        var servicios = await _servicioRepository.GetActivosAsync();
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
    {
        if (precioMin < 0)
        {
            _logger.LogWarning("⚠️ Precio mínimo inválido: {PrecioMin}", precioMin);
            throw new ArgumentException("El precio mínimo no puede ser negativo", nameof(precioMin));
        }

        if (precioMax < precioMin)
        {
            _logger.LogWarning("⚠️ Precio máximo menor que precio mínimo");
            throw new ArgumentException("El precio máximo debe ser mayor o igual al precio mínimo", nameof(precioMax));
        }

        _logger.LogInformation("💰 Buscando servicios en rango de precio: ${PrecioMin} - ${PrecioMax}", precioMin, precioMax);
        var servicios = await _servicioRepository.GetByRangoPrecioAsync(precioMin, precioMax);
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetByUnidadAsync(string unidad)
    {
        var unidadesValidas = new[] { "persona", "grupo", "hora", "dia" };
        if (!unidadesValidas.Contains(unidad.ToLower()))
        {
            _logger.LogWarning("⚠️ Unidad de medida inválida: {Unidad}", unidad);
            throw new ArgumentException(
                $"Unidad inválida. Valores permitidos: {string.Join(", ", unidadesValidas)}",
                nameof(unidad));
        }

        _logger.LogInformation("🔍 Buscando servicios por unidad: {Unidad}", unidad);
        var servicios = await _servicioRepository.GetByUnidadAsync(unidad);
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetByDuracionMaximaAsync(int duracionMaxima)
    {
        if (duracionMaxima <= 0)
        {
            _logger.LogWarning("⚠️ Duración máxima inválida: {DuracionMaxima}", duracionMaxima);
            throw new ArgumentException("La duración máxima debe ser mayor a 0", nameof(duracionMaxima));
        }

        _logger.LogInformation("⏱️ Buscando servicios con duración máxima de {Duracion} minutos", duracionMaxima);
        var servicios = await _servicioRepository.GetByDuracionMaximaAsync(duracionMaxima);
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ServicioAdicionalResponseDto>> GetByProveedorAsync(int idProveedor)
    {
        if (idProveedor <= 0)
        {
            _logger.LogWarning("⚠️ ID de proveedor inválido: {IdProveedor}", idProveedor);
            throw new ArgumentException("El ID del proveedor debe ser mayor a 0", nameof(idProveedor));
        }

        _logger.LogInformation("🔍 Buscando servicios del proveedor ID: {IdProveedor}", idProveedor);
        var servicios = await _servicioRepository.GetByProveedorAsync(idProveedor);
        return _mapper.Map<IEnumerable<ServicioAdicionalResponseDto>>(servicios);
    }

    /// <inheritdoc/>
    public async Task<ServicioAdicionalResponseDto> CreateAsync(ServicioAdicionalCreateDto servicioDto)
    {
        _logger.LogInformation("📝 Creando nuevo servicio adicional: {Nombre}", servicioDto.Nombre);

        // Validar que el proveedor exista
        var proveedor = await _proveedorRepository.GetByIdAsync(servicioDto.IdProveedor);
        if (proveedor == null)
        {
            _logger.LogWarning("⚠️ Proveedor con ID {IdProveedor} no encontrado", servicioDto.IdProveedor);
            throw new ArgumentException($"Proveedor con ID {servicioDto.IdProveedor} no encontrado", nameof(servicioDto.IdProveedor));
        }

        // Validar que no exista un servicio con el mismo nombre para el mismo proveedor
        var existe = await _servicioRepository.ExistePorNombreYProveedorAsync(servicioDto.Nombre, servicioDto.IdProveedor);
        if (existe)
        {
            _logger.LogWarning("⚠️ Ya existe un servicio con nombre '{Nombre}' para el proveedor {Proveedor}",
                servicioDto.Nombre, proveedor.NombreEmpresa);
            throw new ArgumentException(
                $"Ya existe un servicio con el nombre '{servicioDto.Nombre}' para el proveedor '{proveedor.NombreEmpresa}'");
        }

        // Mapear y crear
        var servicio = _mapper.Map<ServicioAdicional>(servicioDto);

        // Convertir tiempo estimado de formato "H:mm" a minutos
        if (!string.IsNullOrEmpty(servicioDto.TiempoEstimado))
        {
            servicio.TiempoEstimado = ConvertirTiempoAMinutos(servicioDto.TiempoEstimado);
        }

        servicio.FechaCreacion = DateTime.Now;

        await _servicioRepository.AddAsync(servicio);
        await _servicioRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Servicio adicional creado exitosamente con ID: {Id}", servicio.IdServicio);

        // Obtener el servicio creado con el proveedor incluido
        var servicioCreado = await _servicioRepository.GetByIdConProveedorAsync(servicio.IdServicio);
        return _mapper.Map<ServicioAdicionalResponseDto>(servicioCreado);
    }

    /// <inheritdoc/>
    public async Task<ServicioAdicionalResponseDto> UpdateAsync(int id, ServicioAdicionalUpdateDto servicioDto)
    {
        _logger.LogInformation("🔄 Actualizando servicio adicional con ID: {Id}", id);

        // Validar que el servicio exista
        var servicio = await _servicioRepository.GetByIdAsync(id);
        if (servicio == null)
        {
            _logger.LogWarning("⚠️ Servicio adicional con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"Servicio adicional con ID {id} no encontrado");
        }

        // Validar proveedor si se está actualizando
        if (servicioDto.IdProveedor.HasValue && servicioDto.IdProveedor.Value != servicio.IdProveedor)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(servicioDto.IdProveedor.Value);
            if (proveedor == null)
            {
                _logger.LogWarning("⚠️ Proveedor con ID {IdProveedor} no encontrado", servicioDto.IdProveedor.Value);
                throw new ArgumentException($"Proveedor con ID {servicioDto.IdProveedor.Value} no encontrado");
            }
        }

        // Validar unicidad de nombre si se está actualizando
        if (!string.IsNullOrEmpty(servicioDto.Nombre))
        {
            var idProveedorValidar = servicioDto.IdProveedor ?? servicio.IdProveedor;
            var existe = await _servicioRepository.ExistePorNombreYProveedorAsync(servicioDto.Nombre, idProveedorValidar, id);
            if (existe)
            {
                _logger.LogWarning("⚠️ Ya existe otro servicio con nombre '{Nombre}' para el mismo proveedor", servicioDto.Nombre);
                throw new ArgumentException($"Ya existe otro servicio con el nombre '{servicioDto.Nombre}' para el mismo proveedor");
            }
        }

        // Mapear cambios (solo los campos no nulos)
        _mapper.Map(servicioDto, servicio);

        // Convertir tiempo estimado de formato "H:mm" a minutos si se está actualizando
        if (!string.IsNullOrEmpty(servicioDto.TiempoEstimado))
        {
            servicio.TiempoEstimado = ConvertirTiempoAMinutos(servicioDto.TiempoEstimado);
        }

        servicio.FechaModificacion = DateTime.Now;

        await _servicioRepository.UpdateAsync(servicio);
        await _servicioRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Servicio adicional actualizado exitosamente");

        // Obtener el servicio actualizado con el proveedor incluido
        var servicioActualizado = await _servicioRepository.GetByIdConProveedorAsync(id);
        return _mapper.Map<ServicioAdicionalResponseDto>(servicioActualizado);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("🗑️ Eliminando servicio adicional con ID: {Id}", id);

        var servicio = await _servicioRepository.GetByIdAsync(id);
        if (servicio == null)
        {
            _logger.LogWarning("⚠️ Servicio adicional con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"Servicio adicional con ID {id} no encontrado");
        }

        // Soft delete - cambiar estado a inactivo
        servicio.Estado = false;
        servicio.FechaModificacion = DateTime.Now;

        await _servicioRepository.UpdateAsync(servicio);
        await _servicioRepository.SaveChangesAsync();

        _logger.LogInformation("✅ Servicio adicional eliminado (soft delete) exitosamente");
        return true;
    }

    /// <summary>
    /// Convierte un tiempo en formato "H:mm" o "HH:mm" a minutos totales
    /// </summary>
    /// <param name="tiempo">Tiempo en formato "H:mm" (ejemplo: "2:30" para 2 horas y 30 minutos)</param>
    /// <returns>Total de minutos</returns>
    private int? ConvertirTiempoAMinutos(string tiempo)
    {
        if (string.IsNullOrEmpty(tiempo))
            return null;

        var parts = tiempo.Split(':');
        if (parts.Length != 2)
            return null;

        if (!int.TryParse(parts[0], out int horas) || !int.TryParse(parts[1], out int minutos))
            return null;

        return (horas * 60) + minutos;
    }
}
