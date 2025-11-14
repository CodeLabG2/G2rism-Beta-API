using AutoMapper;
using G2rismBeta.API.DTOs.Aerolinea;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio de aerolíneas con lógica de negocio
/// </summary>
public class AerolineaService : IAerolineaService
{
    private readonly IAerolineaRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AerolineaService> _logger;

    public AerolineaService(
        IAerolineaRepository repository,
        IMapper mapper,
        ILogger<AerolineaService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AerolineaResponseDto>> GetAllAsync()
    {
        try
        {
            var aerolineas = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<AerolineaResponseDto>>(aerolineas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las aerolíneas");
            throw;
        }
    }

    public async Task<AerolineaResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {Id}", id);
                return null;
            }

            var aerolinea = await _repository.GetByIdAsync(id);
            return aerolinea != null ? _mapper.Map<AerolineaResponseDto>(aerolinea) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aerolínea con ID: {Id}", id);
            throw;
        }
    }

    public async Task<AerolineaResponseDto?> GetByCodigoIataAsync(string codigoIata)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigoIata))
            {
                _logger.LogWarning("Código IATA vacío o nulo");
                return null;
            }

            var aerolinea = await _repository.GetByCodigoIataAsync(codigoIata);
            return aerolinea != null ? _mapper.Map<AerolineaResponseDto>(aerolinea) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar aerolínea con código IATA: {Codigo}", codigoIata);
            throw;
        }
    }

    public async Task<AerolineaResponseDto?> GetByCodigoIcaoAsync(string codigoIcao)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigoIcao))
            {
                _logger.LogWarning("Código ICAO vacío o nulo");
                return null;
            }

            var aerolinea = await _repository.GetByCodigoIcaoAsync(codigoIcao);
            return aerolinea != null ? _mapper.Map<AerolineaResponseDto>(aerolinea) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar aerolínea con código ICAO: {Codigo}", codigoIcao);
            throw;
        }
    }

    public async Task<IEnumerable<AerolineaResponseDto>> GetByPaisAsync(string pais)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pais))
            {
                _logger.LogWarning("País vacío o nulo");
                return Enumerable.Empty<AerolineaResponseDto>();
            }

            var aerolineas = await _repository.GetByPaisAsync(pais);
            return _mapper.Map<IEnumerable<AerolineaResponseDto>>(aerolineas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aerolíneas del país: {Pais}", pais);
            throw;
        }
    }

    public async Task<IEnumerable<AerolineaResponseDto>> GetByEstadoAsync(string estado)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                _logger.LogWarning("Estado vacío o nulo");
                return Enumerable.Empty<AerolineaResponseDto>();
            }

            var aerolineas = await _repository.GetByEstadoAsync(estado);
            return _mapper.Map<IEnumerable<AerolineaResponseDto>>(aerolineas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener aerolíneas con estado: {Estado}", estado);
            throw;
        }
    }

    public async Task<AerolineaResponseDto> CreateAsync(AerolineaCreateDto createDto)
    {
        try
        {
            // Validar códigos únicos
            if (await _repository.ExistsCodigoIataAsync(createDto.CodigoIata))
            {
                _logger.LogWarning("Ya existe una aerolínea con código IATA: {Codigo}", createDto.CodigoIata);
                throw new InvalidOperationException($"Ya existe una aerolínea con el código IATA '{createDto.CodigoIata}'");
            }

            if (await _repository.ExistsCodigoIcaoAsync(createDto.CodigoIcao))
            {
                _logger.LogWarning("Ya existe una aerolínea con código ICAO: {Codigo}", createDto.CodigoIcao);
                throw new InvalidOperationException($"Ya existe una aerolínea con el código ICAO '{createDto.CodigoIcao}'");
            }

            var aerolinea = _mapper.Map<Aerolinea>(createDto);
            var created = await _repository.CreateAsync(aerolinea);

            _logger.LogInformation("Aerolínea creada exitosamente: {Nombre} ({Iata})",
                created.Nombre, created.CodigoIata);

            return _mapper.Map<AerolineaResponseDto>(created);
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw para que el controller lo maneje
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear aerolínea: {Nombre}", createDto.Nombre);
            throw;
        }
    }

    public async Task<AerolineaResponseDto?> UpdateAsync(int id, AerolineaUpdateDto updateDto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Aerolínea no encontrada con ID: {Id}", id);
                return null;
            }

            // Validar email si se está actualizando
            if (!string.IsNullOrWhiteSpace(updateDto.EmailContacto))
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(updateDto.EmailContacto))
                {
                    throw new InvalidOperationException("El formato del email no es válido");
                }
            }

            // Actualizar solo campos no nulos
            if (!string.IsNullOrWhiteSpace(updateDto.Nombre))
                existing.Nombre = updateDto.Nombre;

            if (!string.IsNullOrWhiteSpace(updateDto.Pais))
                existing.Pais = updateDto.Pais;

            if (updateDto.SitioWeb != null)
                existing.SitioWeb = updateDto.SitioWeb;

            if (!string.IsNullOrWhiteSpace(updateDto.TelefonoContacto))
                existing.TelefonoContacto = updateDto.TelefonoContacto;

            if (!string.IsNullOrWhiteSpace(updateDto.EmailContacto))
                existing.EmailContacto = updateDto.EmailContacto;

            if (updateDto.PoliticasEquipaje != null)
                existing.PoliticasEquipaje = updateDto.PoliticasEquipaje;

            if (!string.IsNullOrWhiteSpace(updateDto.Estado))
                existing.Estado = updateDto.Estado;

            var updated = await _repository.UpdateAsync(existing);
            if (!updated)
            {
                _logger.LogWarning("No se pudo actualizar la aerolínea con ID: {Id}", id);
                return null;
            }

            _logger.LogInformation("Aerolínea actualizada exitosamente: ID {Id}", id);
            return _mapper.Map<AerolineaResponseDto>(existing);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar aerolínea con ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var exists = await _repository.GetByIdAsync(id);
            if (exists == null)
            {
                _logger.LogWarning("Aerolínea no encontrada con ID: {Id}", id);
                return false;
            }

            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                _logger.LogInformation("Aerolínea eliminada (inactivada) exitosamente: ID {Id}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar aerolínea con ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsCodigoIataAsync(string codigoIata, int? excludeId = null)
    {
        try
        {
            return await _repository.ExistsCodigoIataAsync(codigoIata, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia del código IATA: {Codigo}", codigoIata);
            throw;
        }
    }

    public async Task<bool> ExistsCodigoIcaoAsync(string codigoIcao, int? excludeId = null)
    {
        try
        {
            return await _repository.ExistsCodigoIcaoAsync(codigoIcao, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia del código ICAO: {Codigo}", codigoIcao);
            throw;
        }
    }
}
