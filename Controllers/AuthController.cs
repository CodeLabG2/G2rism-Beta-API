using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.DTOs.Auth;
using G2rismBeta.API.Models;
using G2rismBeta.API.Helpers;

namespace G2rismBeta.API.Controllers;

/// <summary>
/// Controlador de Autenticación
/// Gestiona registro, login, logout y recuperación de contraseña
/// Algunos endpoints son públicos ([AllowAnonymous]), otros requieren autenticación.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUsuarioRepository usuarioRepository,
        IMapper mapper,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _usuarioRepository = usuarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    // ========================================
    // ENDPOINT 1: REGISTER (REGISTRO)
    // ========================================

    /// <summary>
    /// Registrar un nuevo usuario en el sistema
    /// Endpoint público - no requiere autenticación
    /// </summary>
    /// <param name="dto">Datos del nuevo usuario</param>
    /// <returns>Usuario registrado exitosamente</returns>
    /// <response code="201">Usuario registrado exitosamente</response>
    /// <response code="400">Datos inválidos o usuario ya existe</response>
    /// <response code="429">Límite de solicitudes excedido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register(
        [FromBody] RegisterRequestDto dto)
    {
        _logger.LogInformation("📝 Iniciando registro de usuario: {Username}", dto.Username);

        try
        {
            // Registrar el usuario
            var usuario = await _authService.RegisterAsync(
                username: dto.Username,
                email: dto.Email,
                password: dto.Password,
                tipoUsuario: dto.TipoUsuario,
                nombre: dto.Nombre,
                apellido: dto.Apellido
            );

            // Mapear a DTO de respuesta
            var responseDto = new RegisterResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                Username = usuario.Username,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                FechaRegistro = usuario.FechaCreacion,
                Roles = usuario.UsuariosRoles?.Select(ur => ur.Rol?.Nombre ?? "").ToList() ?? new List<string>(),
                Mensaje = "Usuario registrado exitosamente. Por favor inicia sesión."
            };

            var response = new ApiResponse<RegisterResponseDto>
            {
                Success = true,
                Message = "¡Bienvenido! Tu cuenta ha sido creada exitosamente",
                Data = responseDto
            };

            _logger.LogInformation("✅ Usuario registrado exitosamente: ID={IdUsuario}, Username={Username}",
                usuario.IdUsuario, usuario.Username);

            return CreatedAtAction(
                nameof(GetProfile),
                new { id = usuario.IdUsuario },
                response
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Error en registro: {Message}", ex.Message);
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400,
                ErrorCode = "REGISTRO_ERROR"
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validación de contraseña fallida: {Message}", ex.Message);
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400,
                ErrorCode = "PASSWORD_INVALIDA"
            });
        }
    }

    // ========================================
    // ENDPOINT 2: LOGIN (INICIAR SESIÓN)
    // ========================================

    /// <summary>
    /// Iniciar sesión en el sistema
    /// Endpoint público - no requiere autenticación
    /// </summary>
    /// <param name="dto">Credenciales de acceso</param>
    /// <returns>Datos del usuario autenticado</returns>
    /// <response code="200">Login exitoso</response>
    /// <response code="401">Credenciales inválidas</response>
    /// <response code="403">Cuenta bloqueada o inactiva</response>
    /// <response code="429">Límite de solicitudes excedido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto dto)
    {
        _logger.LogInformation("🔐 Intento de login: {UsernameOrEmail}", dto.UsernameOrEmail);

        try
        {
            var usuario = await _authService.LoginAsync(dto.UsernameOrEmail, dto.Password);

            if (usuario == null)
            {
                _logger.LogWarning("⚠️ Login fallido: credenciales incorrectas para {UsernameOrEmail}",
                    dto.UsernameOrEmail);

                return Unauthorized(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Credenciales incorrectas. Verifica tu usuario/email y contraseña.",
                    StatusCode = 401,
                    ErrorCode = "CREDENCIALES_INVALIDAS"
                });
            }

            // Generar tokens JWT
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var (accessToken, refreshToken, expiration) = await _authService.GenerarTokensAsync(
                usuario,
                ipAddress,
                userAgent
            );

            // Mapear a DTO de respuesta
            var responseDto = _mapper.Map<LoginResponseDto>(usuario);

            // Agregar tokens JWT al response
            responseDto.Token = accessToken;
            responseDto.TokenExpiration = expiration;
            responseDto.RefreshToken = refreshToken;

            var response = new ApiResponse<LoginResponseDto>
            {
                Success = true,
                Message = $"¡Bienvenido de vuelta, {usuario.Username}!",
                Data = responseDto
            };

            _logger.LogInformation("✅ Login exitoso: Usuario={Username}, ID={IdUsuario}, IP={IpAddress}",
                usuario.Username, usuario.IdUsuario, ipAddress);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Login bloqueado: {Message}", ex.Message);
            return StatusCode(403, new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 403,
                ErrorCode = "CUENTA_BLOQUEADA"
            });
        }
    }

    // ========================================
    // ENDPOINT 3: LOGOUT (CERRAR SESIÓN)
    // ========================================

    /// <summary>
    /// Cerrar sesión del sistema
    /// Requiere autenticación - usuario debe estar logueado
    /// </summary>
    /// <param name="idUsuario">ID del usuario que cierra sesión</param>
    /// <returns>Confirmación de cierre de sesión</returns>
    /// <response code="200">Logout exitoso</response>
    /// <response code="401">No autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] int idUsuario)
    {
        _logger.LogInformation("👋 Usuario cerrando sesión: ID={IdUsuario}", idUsuario);

        await _authService.LogoutAsync(idUsuario);

        var response = new ApiResponse<object>
        {
            Success = true,
            Message = "Sesión cerrada exitosamente",
            Data = new { IdUsuario = idUsuario, LogoutTime = DateTime.Now }
        };

        _logger.LogInformation("✅ Logout exitoso: ID={IdUsuario}", idUsuario);

        return Ok(response);
    }

    // ========================================
    // ENDPOINT 4: REFRESH TOKEN (RENOVAR ACCESS TOKEN)
    // ========================================

    /// <summary>
    /// Renovar access token usando refresh token
    /// Endpoint público - no requiere autenticación (usa refresh token)
    /// </summary>
    /// <param name="dto">Refresh token válido</param>
    /// <returns>Nuevo access token y refresh token</returns>
    /// <response code="200">Tokens renovados exitosamente</response>
    /// <response code="401">Refresh token inválido o expirado</response>
    /// <response code="429">Límite de solicitudes excedido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto dto)
    {
        _logger.LogInformation("🔄 Solicitud de renovación de token");

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var (newAccessToken, newRefreshToken, expiration) = await _authService.RefreshTokenAsync(
                dto.RefreshToken,
                ipAddress,
                userAgent
            );

            // Obtener información del usuario desde el nuevo access token
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(newAccessToken);

            var userId = int.Parse(jwtToken.Claims.First(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var username = jwtToken.Claims.First(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value;

            var responseDto = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = expiration,
                IdUsuario = userId,
                Username = username
            };

            var response = new ApiResponse<RefreshTokenResponseDto>
            {
                Success = true,
                Message = "Tokens renovados exitosamente",
                Data = responseDto
            };

            _logger.LogInformation("✅ Tokens renovados: Usuario={Username}, IP={IpAddress}", username, ipAddress);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("⚠️ Refresh token inválido: {Message}", ex.Message);
            return Unauthorized(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 401,
                ErrorCode = "REFRESH_TOKEN_INVALIDO"
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("⚠️ Usuario no encontrado: {Message}", ex.Message);
            return Unauthorized(new ApiErrorResponse
            {
                Success = false,
                Message = "Usuario no encontrado",
                StatusCode = 401,
                ErrorCode = "USUARIO_NO_ENCONTRADO"
            });
        }
    }

    // ========================================
    // ENDPOINT 5: RECUPERAR PASSWORD
    // ========================================

    /// <summary>
    /// Solicitar recuperación de contraseña
    /// Se enviará un token al email registrado
    /// Endpoint público - no requiere autenticación
    /// </summary>
    /// <param name="dto">Email del usuario</param>
    /// <returns>Confirmación de envío de token</returns>
    /// <response code="200">Token enviado al email</response>
    /// <response code="404">Email no encontrado</response>
    /// <response code="429">Límite de solicitudes excedido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("recuperar-password")]
    [AllowAnonymous]
    [EnableRateLimiting("password-recovery")]
    [ProducesResponseType(typeof(ApiResponse<RecuperarPasswordResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RecuperarPasswordResponseDto>>> RecuperarPassword(
        [FromBody] RecuperarPasswordRequestDto dto)
    {
        _logger.LogInformation("🔑 Solicitud de recuperación de contraseña: {Email}", dto.Email);

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var token = await _authService.SolicitarRecuperacionPasswordAsync(dto.Email, dto.FrontendUrl, ipAddress);

            var responseDto = new RecuperarPasswordResponseDto
            {
                Success = true,
                Message = "Si el email existe, recibirás un correo con instrucciones para recuperar tu contraseña",
                EmailEnviado = true,
                FechaExpiracion = DateTime.Now.AddHours(1)
                // ✅ SEGURIDAD: El token NO se expone en la respuesta, solo se envía por email
            };

            var response = new ApiResponse<RecuperarPasswordResponseDto>
            {
                Success = true,
                Message = "Solicitud procesada exitosamente",
                Data = responseDto
            };

            _logger.LogInformation("✅ Token de recuperación generado para: {Email}", dto.Email);

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("⚠️ Email no encontrado: {Email}", dto.Email);

            // Por seguridad, devolver la misma respuesta aunque el email no exista
            var responseDto = new RecuperarPasswordResponseDto
            {
                Success = true,
                Message = "Si el email existe, recibirás un correo con instrucciones para recuperar tu contraseña",
                EmailEnviado = false,
                FechaExpiracion = DateTime.Now.AddHours(1)
            };

            return Ok(new ApiResponse<RecuperarPasswordResponseDto>
            {
                Success = true,
                Message = "Solicitud procesada exitosamente",
                Data = responseDto
            });
        }
    }

    // ========================================
    // ENDPOINT 5: RESET PASSWORD (CON CÓDIGO DE 6 DÍGITOS)
    // ========================================

    /// <summary>
    /// Restablecer contraseña usando código de recuperación de 6 dígitos
    /// Endpoint público - no requiere autenticación (usa código de recuperación)
    /// </summary>
    /// <param name="dto">Código de 6 dígitos y nueva contraseña</param>
    /// <returns>Confirmación de cambio de contraseña</returns>
    /// <response code="200">Contraseña cambiada exitosamente</response>
    /// <response code="400">Código inválido o contraseña débil</response>
    /// <response code="429">Límite de solicitudes excedido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("password-recovery")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
        [FromBody] ResetPasswordDto dto)
    {
        _logger.LogInformation("🔐 Restableciendo contraseña con código: {Codigo}", dto.Codigo);

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var resultado = await _authService.RestablecerPasswordAsync(dto.Codigo, dto.NewPassword, ipAddress);

            if (!resultado)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "No se pudo restablecer la contraseña",
                    StatusCode = 400,
                    ErrorCode = "RESET_FALLIDO"
                });
            }

            var response = new ApiResponse<object>
            {
                Success = true,
                Message = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión con tu nueva contraseña.",
                Data = new { PasswordChanged = true, Timestamp = DateTime.Now }
            };

            _logger.LogInformation("✅ Contraseña restablecida exitosamente");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Token inválido o expirado");
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400,
                ErrorCode = "TOKEN_INVALIDO"
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Contraseña débil: {Message}", ex.Message);
            return BadRequest(new ApiErrorResponse
            {
                Success = false,
                Message = ex.Message,
                StatusCode = 400,
                ErrorCode = "PASSWORD_DEBIL"
            });
        }
    }

    // ========================================
    // ENDPOINT 6: CAMBIAR PASSWORD (AUTENTICADO)
    // ========================================

    /// <summary>
    /// Cambiar contraseña estando autenticado
    /// Requiere autenticación - usuario debe estar logueado
    /// Requiere la contraseña actual para validación
    /// </summary>
    /// <param name="dto">Contraseña actual y nueva contraseña</param>
    /// <returns>Confirmación de cambio de contraseña</returns>
    /// <response code="200">Contraseña cambiada exitosamente</response>
    /// <response code="400">Contraseña actual incorrecta</response>
    /// <response code="401">No autenticado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("cambiar-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> CambiarPassword(
        [FromBody] CambiarPasswordDto dto)
    {
        _logger.LogInformation("🔐 Usuario cambiando contraseña: ID={IdUsuario}", dto.IdUsuario);

        try
        {
            // ========================================
            // PASO 1: Buscar el usuario por ID
            // ========================================
            var usuario = await _usuarioRepository.GetByIdAsync(dto.IdUsuario);

            if (usuario == null)
            {
                _logger.LogWarning("⚠️ Usuario no encontrado: ID={IdUsuario}", dto.IdUsuario);
                return NotFound(new ApiErrorResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    StatusCode = 404,
                    ErrorCode = "USUARIO_NO_ENCONTRADO"
                });
            }

            // ========================================
            // PASO 2: Verificar la contraseña actual con BCrypt
            // ========================================
            bool passwordValida = PasswordHasher.VerifyPassword(dto.CurrentPassword, usuario.PasswordHash);

            if (!passwordValida)
            {
                _logger.LogWarning("⚠️ Contraseña actual incorrecta para usuario ID={IdUsuario}", dto.IdUsuario);
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = "La contraseña actual es incorrecta",
                    StatusCode = 400,
                    ErrorCode = "PASSWORD_ACTUAL_INCORRECTA"
                });
            }

            // ========================================
            // PASO 3: Validar fortaleza de la nueva contraseña
            // (FluentValidation ya lo hace, pero validamos por si acaso)
            // ========================================
            var (esValida, errorPassword) = PasswordHasher.ValidatePasswordStrength(dto.NewPassword);
            if (!esValida)
            {
                _logger.LogWarning("⚠️ Nueva contraseña débil para usuario ID={IdUsuario}", dto.IdUsuario);
                return BadRequest(new ApiErrorResponse
                {
                    Success = false,
                    Message = errorPassword ?? "La nueva contraseña no cumple los requisitos de seguridad",
                    StatusCode = 400,
                    ErrorCode = "PASSWORD_DEBIL"
                });
            }

            // ========================================
            // PASO 4: Actualizar la contraseña directamente
            // ========================================
            usuario.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
            usuario.FechaModificacion = DateTime.Now;

            // 📊 AUDITORÍA: Registrar la IP para trazabilidad
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation(
                "🔐 Contraseña cambiada (autenticado) | Usuario: {Username} (ID: {UserId}) | IP: {IpAddress}",
                usuario.Username,
                usuario.IdUsuario,
                ipAddress ?? "No registrada"
            );

            await _usuarioRepository.UpdateAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            // ========================================
            // PASO 5: Retornar respuesta exitosa
            // ========================================
            var response = new ApiResponse<object>
            {
                Success = true,
                Message = "Contraseña cambiada exitosamente",
                Data = new { PasswordChanged = true, UserId = dto.IdUsuario, Timestamp = DateTime.Now }
            };

            _logger.LogInformation("✅ Contraseña cambiada exitosamente: Usuario ID={IdUsuario}", dto.IdUsuario);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al cambiar contraseña para usuario ID={IdUsuario}", dto.IdUsuario);
            return StatusCode(500, new ApiErrorResponse
            {
                Success = false,
                Message = "Error interno al cambiar la contraseña",
                StatusCode = 500,
                ErrorCode = "ERROR_INTERNO"
            });
        }
    }

    // ========================================
    // ENDPOINT AUXILIAR: GET PROFILE
    // ========================================

    /// <summary>
    /// Obtener perfil del usuario (usado por CreatedAtAction en Register)
    /// </summary>
    [HttpGet("profile/{id}")]
    [ApiExplorerSettings(IgnoreApi = true)] // Ocultar de Swagger
    public Task<ActionResult> GetProfile(int id)
    {
        // Este endpoint es solo para completar el CreatedAtAction
        // En una implementación real, aquí iría la lógica para obtener el perfil
        return Task.FromResult<ActionResult>(Ok(new { Message = "Ver perfil en /api/usuarios/{id}" }));
    }
}