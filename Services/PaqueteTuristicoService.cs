using AutoMapper;
using G2rismBeta.API.DTOs.PaqueteTuristico;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Implementación del servicio de paquetes turísticos con lógica de negocio
/// </summary>
public class PaqueteTuristicoService : IPaqueteTuristicoService
{
    private readonly IPaqueteTuristicoRepository _paqueteRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PaqueteTuristicoService> _logger;

    public PaqueteTuristicoService(
        IPaqueteTuristicoRepository paqueteRepository,
        IMapper mapper,
        ILogger<PaqueteTuristicoService> logger)
    {
        _paqueteRepository = paqueteRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetAllAsync()
    {
        _logger.LogInformation("Obteniendo todos los paquetes turísticos");

        var paquetes = await _paqueteRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<PaqueteTuristicoResponseDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("Obteniendo paquete turístico con ID: {Id}", id);

        var paquete = await _paqueteRepository.GetByIdAsync(id);
        if (paquete == null)
        {
            _logger.LogWarning("Paquete turístico con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"No se encontró el paquete turístico con ID {id}");
        }

        return _mapper.Map<PaqueteTuristicoResponseDto>(paquete);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByDestinoAsync(string destino)
    {
        _logger.LogInformation("Buscando paquetes por destino: {Destino}", destino);

        var paquetes = await _paqueteRepository.GetByDestinoAsync(destino);
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByTipoAsync(string tipo)
    {
        _logger.LogInformation("Buscando paquetes por tipo: {Tipo}", tipo);

        var paquetes = await _paqueteRepository.GetByTipoAsync(tipo);
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetDisponiblesAsync()
    {
        _logger.LogInformation("Obteniendo paquetes disponibles");

        var paquetes = await _paqueteRepository.GetDisponiblesAsync();
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByDuracionAsync(int diasMin, int diasMax)
    {
        _logger.LogInformation("Buscando paquetes por duración: {Min}-{Max} días", diasMin, diasMax);

        if (diasMin > diasMax)
        {
            throw new ArgumentException("La duración mínima no puede ser mayor a la máxima");
        }

        var paquetes = await _paqueteRepository.GetByDuracionAsync(diasMin, diasMax);
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByRangoPrecioAsync(decimal precioMin, decimal precioMax)
    {
        _logger.LogInformation("Buscando paquetes por rango de precio: ${Min}-${Max}", precioMin, precioMax);

        if (precioMin > precioMax)
        {
            throw new ArgumentException("El precio mínimo no puede ser mayor al máximo");
        }

        var paquetes = await _paqueteRepository.GetByRangoPrecioAsync(precioMin, precioMax);
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetByNivelDificultadAsync(string nivel)
    {
        _logger.LogInformation("Buscando paquetes por nivel de dificultad: {Nivel}", nivel);

        var paquetes = await _paqueteRepository.GetByNivelDificultadAsync(nivel);
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetProximosAIniciarAsync()
    {
        _logger.LogInformation("Obteniendo paquetes próximos a iniciar");

        var paquetes = await _paqueteRepository.GetProximosAIniciarAsync();
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PaqueteTuristicoResponseDto>> GetVigentesAsync()
    {
        _logger.LogInformation("Obteniendo paquetes vigentes");

        var paquetes = await _paqueteRepository.GetVigentesAsync();
        return _mapper.Map<IEnumerable<PaqueteTuristicoResponseDto>>(paquetes);
    }

    /// <inheritdoc/>
    public async Task<PaqueteTuristicoResponseDto> CreateAsync(PaqueteTuristicoCreateDto paqueteDto)
    {
        _logger.LogInformation("Creando nuevo paquete turístico: {Nombre}", paqueteDto.Nombre);

        // Validar que no exista un paquete con el mismo nombre
        if (await _paqueteRepository.ExistePorNombreAsync(paqueteDto.Nombre))
        {
            _logger.LogWarning("Ya existe un paquete con el nombre: {Nombre}", paqueteDto.Nombre);
            throw new ArgumentException($"Ya existe un paquete turístico con el nombre '{paqueteDto.Nombre}'");
        }

        // Mapear DTO a entidad
        var paquete = _mapper.Map<PaqueteTuristico>(paqueteDto);
        paquete.FechaCreacion = DateTime.Now;

        // Guardar en base de datos
        await _paqueteRepository.AddAsync(paquete);
        await _paqueteRepository.SaveChangesAsync();

        _logger.LogInformation("Paquete turístico creado exitosamente con ID: {Id}", paquete.IdPaquete);

        return _mapper.Map<PaqueteTuristicoResponseDto>(paquete);
    }

    /// <inheritdoc/>
    public async Task<PaqueteTuristicoResponseDto> UpdateAsync(int id, PaqueteTuristicoUpdateDto paqueteDto)
    {
        _logger.LogInformation("Actualizando paquete turístico con ID: {Id}", id);

        // Verificar que el paquete existe
        var paquete = await _paqueteRepository.GetByIdAsync(id);
        if (paquete == null)
        {
            _logger.LogWarning("Paquete turístico con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"No se encontró el paquete turístico con ID {id}");
        }

        // Validar nombre único si se está actualizando
        if (!string.IsNullOrEmpty(paqueteDto.Nombre) && paqueteDto.Nombre != paquete.Nombre)
        {
            if (await _paqueteRepository.ExistePorNombreAsync(paqueteDto.Nombre, id))
            {
                _logger.LogWarning("Ya existe otro paquete con el nombre: {Nombre}", paqueteDto.Nombre);
                throw new ArgumentException($"Ya existe otro paquete turístico con el nombre '{paqueteDto.Nombre}'");
            }
        }

        // Mapear cambios (AutoMapper ignorará valores null)
        _mapper.Map(paqueteDto, paquete);
        paquete.FechaModificacion = DateTime.Now;

        // Guardar cambios
        await _paqueteRepository.UpdateAsync(paquete);
        await _paqueteRepository.SaveChangesAsync();

        _logger.LogInformation("Paquete turístico con ID {Id} actualizado exitosamente", id);

        return _mapper.Map<PaqueteTuristicoResponseDto>(paquete);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Eliminando paquete turístico con ID: {Id}", id);

        // Verificar que el paquete existe
        var paquete = await _paqueteRepository.GetByIdAsync(id);
        if (paquete == null)
        {
            _logger.LogWarning("Paquete turístico con ID {Id} no encontrado", id);
            throw new KeyNotFoundException($"No se encontró el paquete turístico con ID {id}");
        }

        // Soft delete - cambiar estado a inactivo
        paquete.Estado = false;
        paquete.FechaModificacion = DateTime.Now;

        await _paqueteRepository.UpdateAsync(paquete);
        await _paqueteRepository.SaveChangesAsync();

        _logger.LogInformation("Paquete turístico con ID {Id} eliminado exitosamente (soft delete)", id);

        return true;
    }
}
