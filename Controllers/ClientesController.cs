using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using G2rismBeta.API.DTOs.Cliente;
using G2rismBeta.API.Interfaces;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador para la gestión de Clientes (CRM)
/// Endpoints para operaciones CRUD de clientes y gestión de categorización
/// Requiere autenticación. Accesible para empleados (Super Admin, Admin, Empleado).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Super Administrador,Administrador,Empleado")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    /// <summary>
    /// Constructor: Recibe el servicio de clientes por inyección de dependencias
    /// </summary>
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    // ========================================
    // ENDPOINTS DE CONSULTA (GET)
    // ========================================

    /// <summary>
    /// Obtener todos los clientes del sistema
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     GET /api/clientes
    /// 
    /// </remarks>
    /// <response code="200">Lista de clientes obtenida exitosamente</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetAllClientes()
    {
        try
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener los clientes", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener solo clientes activos
    /// </summary>
    /// <response code="200">Lista de clientes activos obtenida exitosamente</response>
    [HttpGet("activos")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientesActivos()
    {
        try
        {
            var clientes = await _clienteService.GetClientesActivosAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener los clientes activos", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un cliente específico por su ID
    /// </summary>
    /// <param name="id">ID del cliente a buscar</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     GET /api/clientes/1
    /// 
    /// </remarks>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponseDto>> GetClienteById(int id)
    {
        try
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);

            if (cliente == null)
            {
                return NotFound(new { message = $"No se encontró el cliente con ID {id}" });
            }

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un cliente con información detallada de su categoría
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <response code="200">Cliente con categoría obtenido exitosamente</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("{id}/con-categoria")]
    [ProducesResponseType(typeof(ClienteConCategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteConCategoriaDto>> GetClienteConCategoria(int id)
    {
        try
        {
            var cliente = await _clienteService.GetClienteConCategoriaAsync(id);

            if (cliente == null)
            {
                return NotFound(new { message = $"No se encontró el cliente con ID {id}" });
            }

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el cliente con categoría", error = ex.Message });
        }
    }

    /// <summary>
    /// Buscar cliente por documento de identidad
    /// </summary>
    /// <param name="documento">Documento de identidad a buscar</param>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("por-documento/{documento}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponseDto>> GetClienteByDocumento(string documento)
    {
        try
        {
            var cliente = await _clienteService.GetClienteByDocumentoAsync(documento);

            if (cliente == null)
            {
                return NotFound(new { message = $"No se encontró el cliente con documento {documento}" });
            }

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al buscar el cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Buscar cliente por ID de usuario
    /// </summary>
    /// <param name="idUsuario">ID del usuario asociado</param>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpGet("por-usuario/{idUsuario}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponseDto>> GetClienteByUsuarioId(int idUsuario)
    {
        try
        {
            var cliente = await _clienteService.GetClienteByUsuarioIdAsync(idUsuario);

            if (cliente == null)
            {
                return NotFound(new { message = $"No se encontró el cliente asociado al usuario {idUsuario}" });
            }

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al buscar el cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Buscar clientes por nombre o apellido
    /// </summary>
    /// <param name="termino">Término de búsqueda (nombre o apellido)</param>
    /// <response code="200">Lista de clientes encontrados</response>
    [HttpGet("buscar/{termino}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> BuscarClientesPorNombre(string termino)
    {
        try
        {
            var clientes = await _clienteService.BuscarClientesPorNombreAsync(termino);
            var clientesList = clientes.ToList();

            if (!clientesList.Any())
            {
                return Ok(new
                {
                    message = $"No se encontraron clientes que coincidan con '{termino}'",
                    data = clientesList,
                    count = 0
                });
            }

            return Ok(new
            {
                message = $"Se encontraron {clientesList.Count} cliente(s)",
                data = clientesList,
                count = clientesList.Count
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al buscar clientes", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener clientes filtrados por categoría
    /// </summary>
    /// <param name="idCategoria">ID de la categoría</param>
    /// <response code="200">Lista de clientes de la categoría</response>
    /// <response code="404">Categoría no encontrada</response>
    [HttpGet("por-categoria/{idCategoria}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientesPorCategoria(int idCategoria)
    {
        try
        {
            var clientes = await _clienteService.GetClientesPorCategoriaAsync(idCategoria);
            var clientesList = clientes.ToList();

            if (!clientesList.Any())
            {
                return Ok(new
                {
                    message = $"No hay clientes registrados en la categoría con ID {idCategoria}",
                    data = clientesList,
                    count = 0
                });
            }

            return Ok(new
            {
                message = $"Se encontraron {clientesList.Count} cliente(s) en la categoría",
                data = clientesList,
                count = clientesList.Count
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener clientes por categoría", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener clientes filtrados por ciudad
    /// </summary>
    /// <param name="ciudad">Nombre de la ciudad</param>
    /// <response code="200">Lista de clientes de la ciudad</response>
    [HttpGet("por-ciudad/{ciudad}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientesPorCiudad(string ciudad)
    {
        try
        {
            var clientes = await _clienteService.GetClientesPorCiudadAsync(ciudad);
            var clientesList = clientes.ToList();

            if (!clientesList.Any())
            {
                return Ok(new
                {
                    message = $"No se encontraron clientes en la ciudad '{ciudad}'",
                    data = clientesList,
                    count = 0
                });
            }

            return Ok(new
            {
                message = $"Se encontraron {clientesList.Count} cliente(s) en {ciudad}",
                data = clientesList,
                count = clientesList.Count
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener clientes por ciudad", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener clientes filtrados por país
    /// </summary>
    /// <param name="pais">Nombre del país</param>
    /// <response code="200">Lista de clientes del país</response>
    [HttpGet("por-pais/{pais}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientesPorPais(string pais)
    {
        try
        {
            var clientes = await _clienteService.GetClientesPorPaisAsync(pais);
            var clientesList = clientes.ToList();

            if (!clientesList.Any())
            {
                return Ok(new
                {
                    message = $"No se encontraron clientes en el país '{pais}'",
                    data = clientesList,
                    count = 0
                });
            }

            return Ok(new
            {
                message = $"Se encontraron {clientesList.Count} cliente(s) en {pais}",
                data = clientesList,
                count = clientesList.Count
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener clientes por país", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE CREACIÓN (POST)
    // ========================================

    /// <summary>
    /// Crear un nuevo cliente
    /// </summary>
    /// <param name="clienteCreateDto">Datos del cliente a crear</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/clientes
    ///     {
    ///       "idUsuario": 5,
    ///       "idCategoria": 2,
    ///       "nombre": "Juan",
    ///       "apellido": "Pérez García",
    ///       "documentoIdentidad": "1234567890",
    ///       "tipoDocumento": "CC",
    ///       "fechaNacimiento": "1990-05-15",
    ///       "correoElectronico": "juan.perez@example.com",
    ///       "telefono": "+57 300 123 4567",
    ///       "direccion": "Calle 123 #45-67",
    ///       "ciudad": "Medellín",
    ///       "pais": "Colombia",
    ///       "estado": true
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Cliente creado exitosamente</response>
    /// <response code="400">Datos inválidos o error de validación</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteResponseDto>> CreateCliente([FromBody] ClienteCreateDto clienteCreateDto)
    {
        try
        {
            var clienteCreado = await _clienteService.CreateClienteAsync(clienteCreateDto);
            return CreatedAtAction(
                nameof(GetClienteById),
                new { id = clienteCreado.IdCliente },
                clienteCreado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
            return StatusCode(500, new { message = "Error al crear el cliente", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE ACTUALIZACIÓN (PUT/PATCH)
    // ========================================

    /// <summary>
    /// Actualizar un cliente existente
    /// </summary>
    /// <param name="id">ID del cliente a actualizar</param>
    /// <param name="clienteUpdateDto">Datos actualizados del cliente</param>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     PUT /api/clientes/1
    ///     {
    ///       "idCliente": 1,
    ///       "idCategoria": 3,
    ///       "nombre": "Juan Carlos",
    ///       "apellido": "Pérez García",
    ///       "documentoIdentidad": "1234567890",
    ///       "tipoDocumento": "CC",
    ///       "fechaNacimiento": "1990-05-15",
    ///       "correoElectronico": "juan.perez@example.com",
    ///       "telefono": "+57 300 123 4567",
    ///       "direccion": "Calle 123 #45-67",
    ///       "ciudad": "Medellín",
    ///       "pais": "Colombia",
    ///       "estado": true
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Cliente actualizado exitosamente</response>
    /// <response code="400">Datos inválidos o ID no coincide</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponseDto>> UpdateCliente(int id, [FromBody] ClienteUpdateDto clienteUpdateDto)
    {
        try
        {
            // Validar que el ID del parámetro coincida con el DTO
            if (id != clienteUpdateDto.IdCliente)
            {
                return BadRequest(new { message = "El ID del parámetro no coincide con el ID del cliente" });
            }

            var clienteActualizado = await _clienteService.UpdateClienteAsync(id, clienteUpdateDto);
            return Ok(clienteActualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
            return StatusCode(500, new { message = "Error al actualizar el cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Cambiar el estado de un cliente (activar/desactivar)
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <param name="nuevoEstado">Nuevo estado (true = activo, false = inactivo)</param>
    /// <response code="200">Estado actualizado exitosamente</response>
    /// <response code="404">Cliente no encontrado</response>
    [HttpPatch("{id}/estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CambiarEstadoCliente(int id, [FromBody] bool nuevoEstado)
    {
        try
        {
            var resultado = await _clienteService.CambiarEstadoClienteAsync(id, nuevoEstado);

            if (!resultado)
            {
                return NotFound(new { message = $"No se encontró el cliente con ID {id}" });
            }

            return Ok(new { message = $"El estado del cliente se actualizó a {(nuevoEstado ? "activo" : "inactivo")}" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al cambiar el estado del cliente", error = ex.Message });
        }
    }

    /// <summary>
    /// Asignar o cambiar la categoría de un cliente
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <param name="idCategoria">ID de la nueva categoría</param>
    /// <response code="200">Categoría asignada exitosamente</response>
    /// <response code="404">Cliente o categoría no encontrada</response>
    /// <response code="400">Error de validación</response>
    [HttpPatch("{id}/asignar-categoria/{idCategoria}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AsignarCategoria(int id, int idCategoria)
    {
        try
        {
            var resultado = await _clienteService.AsignarCategoriaAsync(id, idCategoria);

            if (!resultado)
            {
                return BadRequest(new { message = "No se pudo asignar la categoría" });
            }

            return Ok(new { message = "La categoría se asignó exitosamente al cliente" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al asignar la categoría", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE ELIMINACIÓN (DELETE)
    // ========================================

    /// <summary>
    /// Eliminar un cliente (solo si no tiene reservas asociadas)
    /// </summary>
    /// <param name="id">ID del cliente a eliminar</param>
    /// <response code="200">Cliente eliminado exitosamente</response>
    /// <response code="404">Cliente no encontrado</response>
    /// <response code="400">No se puede eliminar (tiene reservas)</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteCliente(int id)
    {
        try
        {
            var resultado = await _clienteService.DeleteClienteAsync(id);

            if (!resultado)
            {
                return BadRequest(new { message = "No se pudo eliminar el cliente" });
            }

            return Ok(new { message = $"El cliente con ID {id} se eliminó exitosamente" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar el cliente", error = ex.Message });
        }
    }

    // ========================================
    // ENDPOINTS DE VALIDACIÓN
    // ========================================

    /// <summary>
    /// Verificar si un documento de identidad ya existe
    /// </summary>
    /// <param name="documento">Documento a verificar</param>
    /// <response code="200">Resultado de la verificación</response>
    [HttpGet("validar-documento/{documento}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidarDocumento(string documento)
    {
        try
        {
            var existe = await _clienteService.DocumentoExisteAsync(documento);
            return Ok(new { existe, mensaje = existe ? "El documento ya está registrado" : "El documento está disponible" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al validar el documento", error = ex.Message });
        }
    }

    /// <summary>
    /// Verificar si un usuario ya tiene un cliente asociado
    /// </summary>
    /// <param name="idUsuario">ID del usuario a verificar</param>
    /// <response code="200">Resultado de la verificación</response>
    [HttpGet("validar-usuario/{idUsuario}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidarUsuarioTieneCliente(int idUsuario)
    {
        try
        {
            var tieneCliente = await _clienteService.UsuarioTieneClienteAsync(idUsuario);
            return Ok(new { tieneCliente, mensaje = tieneCliente ? "El usuario ya tiene un cliente asociado" : "El usuario no tiene cliente asociado" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al validar el usuario", error = ex.Message });
        }
    }
}
