using AutoMapper;
using G2rismBeta.API.DTOs.FormaDePago;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services;

/// <summary>
/// Servicio de Formas de Pago.
/// Implementa la lógica de negocio para la gestión de formas de pago.
/// </summary>
public class FormaDePagoService : IFormaDePagoService
{
    private readonly IFormaDePagoRepository _formaDePagoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<FormaDePagoService> _logger;

    public FormaDePagoService(
        IFormaDePagoRepository formaDePagoRepository,
        IMapper mapper,
        ILogger<FormaDePagoService> logger)
    {
        _formaDePagoRepository = formaDePagoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    #region Consultas

    /// <summary>
    /// Obtener todas las formas de pago
    /// </summary>
    public async Task<IEnumerable<FormaDePagoResponseDto>> GetAllFormasDePagoAsync()
    {
        _logger.LogInformation("📋 Obteniendo todas las formas de pago");

        var formasDePago = await _formaDePagoRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<FormaDePagoResponseDto>>(formasDePago);
    }

    /// <summary>
    /// Obtener todas las formas de pago activas
    /// </summary>
    public async Task<IEnumerable<FormaDePagoResponseDto>> GetFormasDePagoActivasAsync()
    {
        _logger.LogInformation("📋 Obteniendo formas de pago activas");

        var formasDePago = await _formaDePagoRepository.GetFormasDePagoActivasAsync();
        return _mapper.Map<IEnumerable<FormaDePagoResponseDto>>(formasDePago);
    }

    /// <summary>
    /// Obtener forma de pago por ID
    /// </summary>
    public async Task<FormaDePagoResponseDto> GetFormaDePagoByIdAsync(int id)
    {
        _logger.LogInformation($"🔍 Buscando forma de pago con ID: {id}");

        var formaDePago = await _formaDePagoRepository.GetByIdAsync(id);

        if (formaDePago == null)
        {
            _logger.LogWarning($"⚠️ Forma de pago con ID {id} no encontrada");
            throw new KeyNotFoundException($"No se encontró una forma de pago con el ID {id}");
        }

        return _mapper.Map<FormaDePagoResponseDto>(formaDePago);
    }

    /// <summary>
    /// Obtener forma de pago por método
    /// </summary>
    public async Task<FormaDePagoResponseDto> GetFormaDePagoPorMetodoAsync(string metodo)
    {
        _logger.LogInformation($"🔍 Buscando forma de pago con método: {metodo}");

        var formaDePago = await _formaDePagoRepository.GetPorMetodoAsync(metodo);

        if (formaDePago == null)
        {
            _logger.LogWarning($"⚠️ Forma de pago con método '{metodo}' no encontrada");
            throw new KeyNotFoundException($"No se encontró una forma de pago con el método '{metodo}'");
        }

        return _mapper.Map<FormaDePagoResponseDto>(formaDePago);
    }

    /// <summary>
    /// Obtener formas de pago que requieren verificación
    /// </summary>
    public async Task<IEnumerable<FormaDePagoResponseDto>> GetFormasQueRequierenVerificacionAsync()
    {
        _logger.LogInformation("📋 Obteniendo formas de pago que requieren verificación");

        var formasDePago = await _formaDePagoRepository.GetFormasQueRequierenVerificacionAsync();
        return _mapper.Map<IEnumerable<FormaDePagoResponseDto>>(formasDePago);
    }

    #endregion

    #region Operaciones CRUD

    /// <summary>
    /// Crear una nueva forma de pago
    /// </summary>
    public async Task<FormaDePagoResponseDto> CreateFormaDePagoAsync(FormaDePagoCreateDto createDto)
    {
        _logger.LogInformation($"📝 Creando nueva forma de pago: {createDto.Metodo}");

        // Validar que el método no exista
        if (await _formaDePagoRepository.ExistePorMetodoAsync(createDto.Metodo))
        {
            _logger.LogWarning($"⚠️ Ya existe una forma de pago con el método: {createDto.Metodo}");
            throw new InvalidOperationException($"Ya existe una forma de pago con el método '{createDto.Metodo}'");
        }

        // Mapear y crear
        var formaDePago = _mapper.Map<FormaDePago>(createDto);
        formaDePago.FechaCreacion = DateTime.Now;

        await _formaDePagoRepository.AddAsync(formaDePago);
        await _formaDePagoRepository.SaveChangesAsync();

        _logger.LogInformation($"✅ Forma de pago creada exitosamente con ID: {formaDePago.IdFormaPago}");

        return _mapper.Map<FormaDePagoResponseDto>(formaDePago);
    }

    /// <summary>
    /// Actualizar una forma de pago existente
    /// </summary>
    public async Task<FormaDePagoResponseDto> UpdateFormaDePagoAsync(int id, FormaDePagoUpdateDto updateDto)
    {
        _logger.LogInformation($"📝 Actualizando forma de pago con ID: {id}");

        var formaDePago = await _formaDePagoRepository.GetByIdAsync(id);

        if (formaDePago == null)
        {
            _logger.LogWarning($"⚠️ Forma de pago con ID {id} no encontrada");
            throw new KeyNotFoundException($"No se encontró una forma de pago con el ID {id}");
        }

        // Si se está cambiando el método, validar que no exista
        if (updateDto.Metodo != null && updateDto.Metodo != formaDePago.Metodo)
        {
            if (await _formaDePagoRepository.ExistePorMetodoAsync(updateDto.Metodo, id))
            {
                _logger.LogWarning($"⚠️ Ya existe una forma de pago con el método: {updateDto.Metodo}");
                throw new InvalidOperationException($"Ya existe una forma de pago con el método '{updateDto.Metodo}'");
            }
        }

        // Actualizar campos individualmente solo si no son null (actualizaciones parciales)
        // IMPORTANTE: No usar AutoMapper aquí porque sobrescribe campos no enviados con valores por defecto
        if (updateDto.Metodo != null)
            formaDePago.Metodo = updateDto.Metodo;

        if (updateDto.RequiereVerificacion.HasValue)
            formaDePago.RequiereVerificacion = updateDto.RequiereVerificacion.Value;

        if (updateDto.Activo.HasValue)
            formaDePago.Activo = updateDto.Activo.Value;

        if (updateDto.Descripcion != null)
            formaDePago.Descripcion = updateDto.Descripcion;

        formaDePago.FechaModificacion = DateTime.Now;

        await _formaDePagoRepository.UpdateAsync(formaDePago);
        await _formaDePagoRepository.SaveChangesAsync();

        _logger.LogInformation($"✅ Forma de pago con ID {id} actualizada exitosamente");

        return _mapper.Map<FormaDePagoResponseDto>(formaDePago);
    }

    /// <summary>
    /// Eliminar una forma de pago
    /// </summary>
    public async Task DeleteFormaDePagoAsync(int id)
    {
        _logger.LogInformation($"🗑️ Eliminando forma de pago con ID: {id}");

        var formaDePago = await _formaDePagoRepository.GetByIdAsync(id);

        if (formaDePago == null)
        {
            _logger.LogWarning($"⚠️ Forma de pago con ID {id} no encontrada");
            throw new KeyNotFoundException($"No se encontró una forma de pago con el ID {id}");
        }

        // Verificar si tiene pagos asociados (incluir la navegación)
        var formaDePagoConPagos = await _formaDePagoRepository.GetByIdAsync(id);

        // Nota: Esto es una eliminación real, no soft delete
        // Si se requiere soft delete, cambiar Activo = false en lugar de DeleteAsync
        await _formaDePagoRepository.DeleteAsync(id);
        await _formaDePagoRepository.SaveChangesAsync();

        _logger.LogInformation($"✅ Forma de pago con ID {id} eliminada exitosamente");
    }

    #endregion
}