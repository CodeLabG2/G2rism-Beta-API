using AutoMapper;
using G2rismBeta.API.DTOs.Vuelo;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio de vuelos con lógica de negocio
/// </summary>
public class VueloService : IVueloService
{
    private readonly IVueloRepository _vueloRepository;
    private readonly IAerolineaRepository _aerolineaRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<VueloService> _logger;

    public VueloService(
        IVueloRepository vueloRepository,
        IAerolineaRepository aerolineaRepository,
        IProveedorRepository proveedorRepository,
        IMapper mapper,
        ILogger<VueloService> logger)
    {
        _vueloRepository = vueloRepository;
        _aerolineaRepository = aerolineaRepository;
        _proveedorRepository = proveedorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<VueloResponseDto>> GetAllAsync()
    {
        try
        {
            var vuelos = await _vueloRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VueloResponseDto>>(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los vuelos");
            throw;
        }
    }

    public async Task<VueloResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {Id}", id);
                return null;
            }

            var vuelo = await _vueloRepository.GetByIdAsync(id);
            return vuelo != null ? _mapper.Map<VueloResponseDto>(vuelo) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vuelo con ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<VueloResponseDto>> GetByOrigenDestinoAsync(string origen, string destino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
            {
                _logger.LogWarning("Origen o destino vacío");
                return Enumerable.Empty<VueloResponseDto>();
            }

            var vuelos = await _vueloRepository.GetByOrigenDestinoAsync(origen, destino);
            return _mapper.Map<IEnumerable<VueloResponseDto>>(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar vuelos de {Origen} a {Destino}", origen, destino);
            throw;
        }
    }

    public async Task<IEnumerable<VueloResponseDto>> GetDisponiblesAsync()
    {
        try
        {
            var vuelos = await _vueloRepository.GetDisponiblesAsync();
            return _mapper.Map<IEnumerable<VueloResponseDto>>(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vuelos disponibles");
            throw;
        }
    }

    public async Task<IEnumerable<VueloResponseDto>> BuscarAsync(string? origen, string? destino, DateTime? fecha)
    {
        try
        {
            var vuelos = await _vueloRepository.BuscarAsync(origen, destino, fecha);
            return _mapper.Map<IEnumerable<VueloResponseDto>>(vuelos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar vuelos con filtros");
            throw;
        }
    }

    public async Task<VueloResponseDto> CreateAsync(VueloCreateDto createDto)
    {
        try
        {
            // Validar que la aerolínea existe
            var aerolinea = await _aerolineaRepository.GetByIdAsync(createDto.IdAerolinea);
            if (aerolinea == null)
            {
                _logger.LogWarning("Aerolínea no encontrada: {IdAerolinea}", createDto.IdAerolinea);
                throw new KeyNotFoundException($"No se encontró la aerolínea con ID {createDto.IdAerolinea}");
            }

            // Validar que el proveedor existe
            var proveedor = await _proveedorRepository.GetByIdAsync(createDto.IdProveedor);
            if (proveedor == null)
            {
                _logger.LogWarning("Proveedor no encontrado: {IdProveedor}", createDto.IdProveedor);
                throw new KeyNotFoundException($"No se encontró el proveedor con ID {createDto.IdProveedor}");
            }

            // Validar que el número de vuelo no exista
            var existeNumero = await _vueloRepository.ExistsNumeroVueloAsync(createDto.NumeroVuelo);
            if (existeNumero)
            {
                _logger.LogWarning("Número de vuelo duplicado: {NumeroVuelo}", createDto.NumeroVuelo);
                throw new InvalidOperationException($"Ya existe un vuelo con el número {createDto.NumeroVuelo}");
            }

            // Crear vuelo
            var vuelo = _mapper.Map<Vuelo>(createDto);
            vuelo.Estado = true; // Activo por defecto

            var vueloCreado = await _vueloRepository.CreateAsync(vuelo);
            _logger.LogInformation("Vuelo creado exitosamente: {NumeroVuelo}", vueloCreado.NumeroVuelo);

            return _mapper.Map<VueloResponseDto>(vueloCreado);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error al crear vuelo");
            throw;
        }
    }

    public async Task<VueloResponseDto> UpdateAsync(int id, VueloUpdateDto updateDto)
    {
        try
        {
            // Buscar vuelo existente
            var vueloExistente = await _vueloRepository.GetByIdAsync(id);
            if (vueloExistente == null)
            {
                _logger.LogWarning("Vuelo no encontrado: {Id}", id);
                throw new KeyNotFoundException($"No se encontró el vuelo con ID {id}");
            }

            // Validar número de vuelo si se está actualizando
            if (!string.IsNullOrWhiteSpace(updateDto.NumeroVuelo) &&
                updateDto.NumeroVuelo != vueloExistente.NumeroVuelo)
            {
                var existeNumero = await _vueloRepository.ExistsNumeroVueloAsync(updateDto.NumeroVuelo, id);
                if (existeNumero)
                {
                    _logger.LogWarning("Número de vuelo duplicado: {NumeroVuelo}", updateDto.NumeroVuelo);
                    throw new InvalidOperationException($"Ya existe un vuelo con el número {updateDto.NumeroVuelo}");
                }
            }

            // Aplicar cambios (solo campos no nulos)
            _mapper.Map(updateDto, vueloExistente);

            await _vueloRepository.UpdateAsync(vueloExistente);
            _logger.LogInformation("Vuelo actualizado exitosamente: {Id}", id);

            // Retornar vuelo actualizado
            var vueloActualizado = await _vueloRepository.GetByIdAsync(id);
            return _mapper.Map<VueloResponseDto>(vueloActualizado);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error al actualizar vuelo con ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var vuelo = await _vueloRepository.GetByIdAsync(id);
            if (vuelo == null)
            {
                _logger.LogWarning("Vuelo no encontrado para eliminar: {Id}", id);
                throw new KeyNotFoundException($"No se encontró el vuelo con ID {id}");
            }

            var resultado = await _vueloRepository.DeleteAsync(id);
            if (resultado)
            {
                _logger.LogInformation("Vuelo eliminado exitosamente: {Id}", id);
            }

            return resultado;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error al eliminar vuelo con ID: {Id}", id);
            throw;
        }
    }
}
