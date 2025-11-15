using Microsoft.AspNetCore.Mvc;
using G2rismBeta.API.DTOs.CategoriaCliente;
using G2rismBeta.API.Interfaces;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Categorías de Cliente (CRM)
/// Endpoints para operaciones CRUD de categorías y segmentación de clientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriasClienteController : ControllerBase
{
    private readonly ICategoriaClienteService _categoriaService;

    /// <summary>
    /// Constructor: Recibe el servicio de categorías por inyección de dependencias
    /// </summary>
    public CategoriasClienteController(ICategoriaClienteService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    // ========================================
    // ENDPOINTS DE CONSULTA (GET)
    // ========================================

    /// <summary>
    /// Obtener todas las categorías de cliente del sistema
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     GET /api/categoriascliente
    /// 
    /// </remarks>
    /// <response code="200">Lista de categorías obtenida exitosamente</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaClienteResponseDto>>> GetAllCategorias()
    {
        try
        {
            var categorias = await _categoriaService.GetAllCategoriasAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las categorías", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener solo categorías activas
    /// </summary>
    /// <response code="200">Lista de categorías activas obtenida exitosamente</response>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaClienteResponseDto>>> GetCategoriasActivas()
    {
        try
        {
            var categorias = await _categoriaService.GetCategoriasActivasAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las categorías activas", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener categorías ordenadas por descuento (mayor a menor)
    /// </summary>
    /// <response code="200">Lista de categorías ordenadas por descuento</response>
    [HttpGet("ordenadas-por-descuento")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaClienteResponseDto>>> GetCategoriasOrdenadaPorDescuento()
    {
        try
        {
            var categorias = await _categoriaService.GetCategoriasOrdenadaPorDescuentoAsync();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las categorías ordenadas", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener una categoría específica por su ID
    /// </summary>
    /// <param name="id">ID de la categoría a buscar</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     GET /api/categoriascliente/1
    /// 
    /// </remarks>
    /// <response code="200">Categoría encontrada</response>
    /// <response code="404">Categoría no encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoriaClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaClienteResponseDto>> GetCategoriaById(int id)
    {
        try
        {
            var categoria = await _categoriaService.GetCategoriaByIdAsync(id);

            if (categoria == null)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID {id}" });
            }

            return Ok(categoria);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener la categoría", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE CREACIÓN (POST)
    // ========================================

    /// <summary>
    /// Crear una nueva categoría de cliente
    /// </summary>
    /// <param name="categoriaCreateDto">Datos de la categoría a crear</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/categoriascliente
    ///     {
    ///       "nombre": "Oro",
    ///       "descripcion": "Categoría premium con beneficios exclusivos",
    ///       "colorHex": "#FFD700",
    ///       "beneficios": "Descuentos del 15%, prioridad en atención",
    ///       "criteriosClasificacion": "Más de 10 reservas anuales",
    ///       "descuentoPorcentaje": 15,
    ///       "estado": true
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Categoría creada exitosamente</response>
    /// <response code="400">Datos inválidos o nombre duplicado</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoriaClienteResponseDto>> CreateCategoria(
        [FromBody] CategoriaClienteCreateDto categoriaCreateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _categoriaService.CreateCategoriaAsync(categoriaCreateDto);

            return CreatedAtAction(
                nameof(GetCategoriaById),
                new { id = categoria.IdCategoria },
                categoria
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear la categoría", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE ACTUALIZACIÓN (PUT)
    // ========================================

    /// <summary>
    /// Actualizar una categoría existente
    /// </summary>
    /// <param name="id">ID de la categoría a actualizar</param>
    /// <param name="categoriaUpdateDto">Nuevos datos de la categoría</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     PUT /api/categoriascliente/1
    ///     {
    ///       "idCategoria": 1,
    ///       "nombre": "Oro Premium",
    ///       "descripcion": "Categoría premium actualizada",
    ///       "colorHex": "#FFD700",
    ///       "beneficios": "Descuentos del 20%",
    ///       "criteriosClasificacion": "Más de 15 reservas anuales",
    ///       "descuentoPorcentaje": 20,
    ///       "estado": true
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Categoría actualizada exitosamente</response>
    /// <response code="400">Datos inválidos o ID no coincide</response>
    /// <response code="404">Categoría no encontrada</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoriaClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaClienteResponseDto>> UpdateCategoria(
        int id,
        [FromBody] CategoriaClienteUpdateDto categoriaUpdateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaUpdateDto.IdCategoria)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del cuerpo de la petición" });
            }

            var categoria = await _categoriaService.UpdateCategoriaAsync(id, categoriaUpdateDto);

            return Ok(categoria);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar la categoría", error = ex.Message });
        }
    }

    /// <summary>
    /// Cambiar el estado de una categoría (activar/desactivar)
    /// </summary>
    /// <param name="id">ID de la categoría</param>
    /// <param name="estado">Nuevo estado (true = activo, false = inactivo)</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     PUT /api/categoriascliente/1/estado?estado=false
    /// 
    /// </remarks>
    /// <response code="200">Estado cambiado exitosamente</response>
    /// <response code="404">Categoría no encontrada</response>
    [HttpPatch("{id}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CambiarEstadoCategoria(int id, [FromQuery] bool estado)
    {
        try
        {
            var resultado = await _categoriaService.CambiarEstadoCategoriaAsync(id, estado);

            if (!resultado)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID {id}" });
            }

            return Ok(new
            {
                message = $"Categoría {(estado ? "activada" : "desactivada")} exitosamente",
                idCategoria = id,
                nuevoEstado = estado
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al cambiar el estado de la categoría", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE ELIMINACIÓN (DELETE)
    // ========================================

    /// <summary>
    /// Eliminar una categoría (solo si no tiene clientes asignados)
    /// </summary>
    /// <param name="id">ID de la categoría a eliminar</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     DELETE /api/categoriascliente/1
    /// 
    /// </remarks>
    /// <response code="200">Categoría eliminada exitosamente</response>
    /// <response code="400">No se puede eliminar porque tiene clientes asignados</response>
    /// <response code="404">Categoría no encontrada</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategoria(int id)
    {
        try
        {
            var resultado = await _categoriaService.DeleteCategoriaAsync(id);

            if (!resultado)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID {id}" });
            }

            return Ok(new { message = "Categoría eliminada exitosamente", idCategoria = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar la categoría", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE VALIDACIÓN
    // ========================================

    /// <summary>
    /// Verificar si existe una categoría con el nombre especificado
    /// </summary>
    /// <param name="nombre">Nombre a verificar</param>
    /// <param name="idExcluir">ID a excluir de la búsqueda (opcional, para actualización)</param>
    /// <response code="200">Retorna true si existe, false si no existe</response>
    [HttpGet("existe-nombre")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ExisteNombre(
        [FromQuery] string nombre,
        [FromQuery] int? idExcluir = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { message = "El nombre es obligatorio" });
            }

            var existe = await _categoriaService.NombreCategoriaExisteAsync(nombre, idExcluir);
            return Ok(existe);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al verificar el nombre", error = ex.Message });
        }
    }
}
