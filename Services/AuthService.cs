using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;
using G2rismBeta.API.Helpers;

namespace G2rismBeta.API.Services;

/// <summary>
/// Implementación del servicio de Autenticación
/// Incluye Register, Login, Logout, Recuperación de contraseña y JWT
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;
    private readonly ITokenRecuperacionRepository _tokenRepository;
    private readonly ICodigoRecuperacionRepository _codigoRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    // Configuración de seguridad
    private const int MAX_INTENTOS_FALLIDOS = 5;
    private const int HORAS_EXPIRACION_TOKEN = 1;
    private const int MAX_INTENTOS_CODIGO = 5;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository,
        ITokenRecuperacionRepository tokenRepository,
        ICodigoRecuperacionRepository codigoRepository,
        IRolRepository rolRepository,
        IRefreshTokenRepository refreshTokenRepository,
        JwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _usuarioRepository = usuarioRepository;
        _usuarioRolRepository = usuarioRolRepository;
        _tokenRepository = tokenRepository;
        _codigoRepository = codigoRepository;
        _rolRepository = rolRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
        _emailService = emailService;
    }

    // ========================================
    // REGISTRO
    // ========================================

    /// <summary>
    /// Registrar un nuevo usuario
    /// </summary>
    public async Task<Usuario> RegisterAsync(
        string username,
        string email,
        string password,
        string tipoUsuario = "cliente",
        string? nombre = null,
        string? apellido = null)
    {
        // 1. Validar que el username no exista
        if (await _usuarioRepository.ExistsByUsernameAsync(username))
        {
            throw new InvalidOperationException($"El username '{username}' ya está en uso");
        }

        // 2. Validar que el email no exista
        if (await _usuarioRepository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException($"El email '{email}' ya está registrado");
        }

        // 3. Validar fortaleza de la contraseña
        var (esValidaPassword, errorPassword) = PasswordHasher.ValidatePasswordStrength(password);
        if (!esValidaPassword)
        {
            throw new ArgumentException(errorPassword ?? "La contraseña no cumple los requisitos de seguridad");
        }

        // 4. Crear el nuevo usuario
        var nuevoUsuario = new Usuario
        {
            Username = username.Trim(),
            Email = email.Trim().ToLower(),
            PasswordHash = PasswordHasher.HashPassword(password),
            TipoUsuario = tipoUsuario.ToLower(),
            Estado = true,
            Bloqueado = false,
            IntentosFallidos = 0,
            FechaCreacion = DateTime.Now
        };

        // 5. Guardar el usuario en la base de datos
        await _usuarioRepository.AddAsync(nuevoUsuario);
        await _usuarioRepository.SaveChangesAsync();

        // 6. Asignar el rol por defecto según el tipo de usuario
        string nombreRol = tipoUsuario.ToLower() == "cliente" ? "Cliente" : "Empleado";
        var rol = await _rolRepository.GetByNombreAsync(nombreRol);

        if (rol != null)
        {
            var usuarioRol = new UsuarioRol
            {
                IdUsuario = nuevoUsuario.IdUsuario,
                IdRol = rol.IdRol,
                FechaAsignacion = DateTime.Now
            };

            await _usuarioRolRepository.AsignarRolAsync(usuarioRol);
            await _usuarioRolRepository.SaveChangesAsync();
        }

        // 7. Enviar email de bienvenida
        var nombreCompleto = $"{nombre} {apellido}";
        await _emailService.SendWelcomeEmailAsync(nuevoUsuario.Email, nuevoUsuario.Username, nombreCompleto);

        // 8. Retornar el usuario con sus roles
        return await _usuarioRepository.GetByIdWithRolesAsync(nuevoUsuario.IdUsuario)
            ?? throw new InvalidOperationException("Error al recuperar el usuario creado");
    }

    // ========================================
    // AUTENTICACIÓN
    // ========================================

    /// <summary>
    /// Iniciar sesión con username o email
    /// </summary>
    public async Task<Usuario?> LoginAsync(string usernameOrEmail, string password)
    {
        // 1. Buscar el usuario por username o email
        var usuario = await _usuarioRepository.GetByUsernameOrEmailAsync(usernameOrEmail);

        if (usuario == null)
        {
            // Usuario no encontrado
            return null;
        }

        // 2. Verificar si la cuenta está bloqueada
        if (usuario.Bloqueado)
        {
            throw new InvalidOperationException("La cuenta está bloqueada. Contacte al administrador.");
        }

        // 3. Verificar si la cuenta está inactiva
        if (!usuario.Estado)
        {
            throw new InvalidOperationException("La cuenta está inactiva.");
        }

        // 4. Verificar la contraseña
        bool passwordValida = PasswordHasher.VerifyPassword(password, usuario.PasswordHash);

        if (!passwordValida)
        {
            // Contraseña incorrecta: incrementar intentos fallidos
            await _usuarioRepository.IncrementarIntentosFallidosAsync(usuario.IdUsuario);

            // Recargar el usuario para obtener los intentos actualizados
            usuario = await _usuarioRepository.GetByIdAsync(usuario.IdUsuario);

            // Si excede el máximo, bloquear la cuenta
            if (usuario != null && usuario.IntentosFallidos >= MAX_INTENTOS_FALLIDOS)
            {
                await _usuarioRepository.BloquearUsuarioAsync(usuario.IdUsuario);
                throw new InvalidOperationException(
                    $"Cuenta bloqueada por exceder {MAX_INTENTOS_FALLIDOS} intentos fallidos. " +
                    "Contacte al administrador."
                );
            }

            return null;
        }

        // 5. Login exitoso
        await _usuarioRepository.ReiniciarIntentosFallidosAsync(usuario.IdUsuario);
        await _usuarioRepository.UpdateUltimoAccesoAsync(usuario.IdUsuario);

        // 6. Obtener el usuario con sus roles
        return await _usuarioRepository.GetByIdWithRolesAsync(usuario.IdUsuario);
    }

    /// <summary>
    /// Generar tokens JWT (access token y refresh token) para un usuario
    /// </summary>
    /// <param name="usuario">Usuario autenticado</param>
    /// <param name="ipAddress">Dirección IP del cliente</param>
    /// <param name="userAgent">User Agent del navegador</param>
    /// <returns>Tupla con access token, refresh token y expiración</returns>
    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> GenerarTokensAsync(
        Usuario usuario,
        string? ipAddress = null,
        string? userAgent = null)
    {
        // 1. Obtener roles y permisos del usuario
        var usuarioConRoles = await _usuarioRepository.GetByIdWithRolesAsync(usuario.IdUsuario);
        if (usuarioConRoles == null)
        {
            throw new InvalidOperationException("No se pudo obtener la información del usuario");
        }

        var roles = usuarioConRoles.UsuariosRoles
            .Select(ur => ur.Rol?.Nombre ?? "")
            .Where(r => !string.IsNullOrEmpty(r))
            .ToList();

        var permisos = usuarioConRoles.UsuariosRoles
            .SelectMany(ur => ur.Rol?.RolesPermisos ?? new List<RolPermiso>())
            .Select(rp => rp.Permiso?.NombrePermiso ?? "")
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .ToList();

        // 2. Generar Access Token (JWT)
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            usuario.IdUsuario,
            usuario.Username,
            usuario.Email,
            usuario.TipoUsuario,
            roles,
            permisos
        );

        // 3. Generar Refresh Token
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // 4. Calcular fecha de expiración
        var expirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
        var expiration = DateTime.UtcNow.AddDays(expirationDays);

        // 5. Guardar Refresh Token en la base de datos
        var refreshTokenEntity = new RefreshToken
        {
            IdUsuario = usuario.IdUsuario,
            Token = refreshToken,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = expiration,
            Revocado = false,
            IpCreacion = ipAddress,
            UserAgent = userAgent
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        // 6. Calcular expiración del access token
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60")
        );

        return (accessToken, refreshToken, accessTokenExpiration);
    }

    /// <summary>
    /// Renovar access token usando refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token válido</param>
    /// <param name="ipAddress">IP del cliente</param>
    /// <param name="userAgent">User Agent del navegador</param>
    /// <returns>Tupla con nuevo access token, nuevo refresh token y expiración</returns>
    public async Task<(string AccessToken, string RefreshToken, DateTime Expiration)> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress = null,
        string? userAgent = null)
    {
        // 1. Validar el refresh token
        var tokenEntity = await _refreshTokenRepository.GetActiveTokenAsync(refreshToken);

        if (tokenEntity == null)
        {
            throw new UnauthorizedAccessException("Refresh token inválido o expirado");
        }

        // 2. Obtener el usuario
        var usuario = await _usuarioRepository.GetByIdAsync(tokenEntity.IdUsuario);

        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado");
        }

        // Verificar que el usuario esté activo
        if (!usuario.Estado || usuario.Bloqueado)
        {
            throw new UnauthorizedAccessException("Usuario inactivo o bloqueado");
        }

        // 3. Revocar el refresh token anterior (rotación de tokens)
        tokenEntity.Revocado = true;
        tokenEntity.FechaRevocacion = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(tokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        // 4. Generar nuevos tokens
        var (newAccessToken, newRefreshToken, expiration) = await GenerarTokensAsync(
            usuario,
            ipAddress,
            userAgent
        );

        // 5. Registrar el token que reemplazó al anterior
        tokenEntity.ReemplazadoPor = newRefreshToken;
        await _refreshTokenRepository.UpdateAsync(tokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return (newAccessToken, newRefreshToken, expiration);
    }

    /// <summary>
    /// Cerrar sesión y revocar refresh tokens
    /// </summary>
    public async Task LogoutAsync(int idUsuario, string? refreshToken = null)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(idUsuario);

        if (usuario != null)
        {
            // Si se proporciona un refresh token específico, revocarlo
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
            }
            else
            {
                // Si no, revocar todos los tokens del usuario
                await _refreshTokenRepository.RevokeAllUserTokensAsync(idUsuario);
            }

            Console.WriteLine($"🔓 Usuario {usuario.Username} cerró sesión");
        }

        await Task.CompletedTask;
    }

    // ========================================
    // RECUPERACIÓN DE CONTRASEÑA
    // ========================================

    /// <summary>
    /// Solicitar recuperación de contraseña (con código de 6 dígitos)
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <param name="frontendUrl">URL del frontend para construir el link de recuperación</param>
    /// <param name="ipSolicitud">IP desde donde se hace la solicitud (opcional)</param>
    public async Task<string> SolicitarRecuperacionPasswordAsync(string email, string frontendUrl, string? ipSolicitud = null)
    {
        // 1. Buscar el usuario por email
        var usuario = await _usuarioRepository.GetByEmailAsync(email);

        if (usuario == null)
        {
            // Por seguridad, no revelar si el email existe o no
            // Devolver OK pero no hacer nada
            throw new KeyNotFoundException("Si el email existe, se enviará un correo de recuperación");
        }

        // 2. Invalidar códigos activos anteriores
        await _codigoRepository.InvalidarCodigosActivosAsync(usuario.IdUsuario);

        // 3. Generar un nuevo código de 6 dígitos
        var codigo = TokenGenerator.GenerateNumericCode(6);
        var codigoRecuperacion = new CodigoRecuperacion
        {
            IdUsuario = usuario.IdUsuario,
            Codigo = codigo,
            TipoCodigo = "recuperacion_password",
            FechaGeneracion = DateTime.Now,
            FechaExpiracion = DateTime.Now.AddHours(HORAS_EXPIRACION_TOKEN),
            Usado = false,
            Bloqueado = false,
            IntentosValidacion = 0,
            IpSolicitud = ipSolicitud
        };

        await _codigoRepository.CrearCodigoAsync(codigoRecuperacion);

        // 4. Enviar email con el código de recuperación
        await _emailService.SendPasswordResetEmailAsync(usuario.Email, usuario.Username, codigo, frontendUrl);

        return codigo;
    }

    /// <summary>
    /// Validar un código de recuperación
    /// </summary>
    public async Task<bool> ValidarTokenRecuperacionAsync(string codigo)
    {
        return await _codigoRepository.ValidarCodigoAsync(codigo);
    }

    /// <summary>
    /// Restablecer contraseña con código de 6 dígitos
    /// Incluye validación de intentos y bloqueo automático
    /// </summary>
    /// <param name="codigo">Código de recuperación de 6 dígitos</param>
    /// <param name="nuevaPassword">Nueva contraseña</param>
    /// <param name="ipAddress">IP desde donde se realiza el cambio (opcional, para auditoría)</param>
    public async Task<bool> RestablecerPasswordAsync(string codigo, string nuevaPassword, string? ipAddress = null)
    {
        // 1. Validar formato del código (debe ser exactamente 6 dígitos)
        if (codigo.Length != 6 || !codigo.All(char.IsDigit))
        {
            throw new ArgumentException("El código debe tener exactamente 6 dígitos numéricos");
        }

        // 2. Obtener el código de recuperación
        var codigoObj = await _codigoRepository.GetByCodigoAsync(codigo);
        if (codigoObj == null)
        {
            throw new KeyNotFoundException("Código no encontrado");
        }

        // 3. Verificar si está bloqueado
        if (codigoObj.Bloqueado)
        {
            throw new InvalidOperationException($"El código ha sido bloqueado por exceso de intentos fallidos. Solicita un nuevo código.");
        }

        // 4. Verificar si está expirado
        if (codigoObj.HaExpirado)
        {
            throw new InvalidOperationException("El código ha expirado. Solicita un nuevo código.");
        }

        // 5. Verificar si ya fue usado
        if (codigoObj.Usado)
        {
            throw new InvalidOperationException("El código ya ha sido utilizado. Solicita un nuevo código.");
        }

        // 6. Validar fortaleza de la nueva contraseña
        var (esValidaPassword, errorPassword) = PasswordHasher.ValidatePasswordStrength(nuevaPassword);
        if (!esValidaPassword)
        {
            // Incrementar intentos incluso si la contraseña no es válida
            await _codigoRepository.IncrementarIntentosAsync(codigo);
            throw new ArgumentException(errorPassword ?? "La contraseña no cumple los requisitos");
        }

        // 7. Obtener el usuario
        var usuario = await _usuarioRepository.GetByIdAsync(codigoObj.IdUsuario);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado");
        }

        // 8. Actualizar la contraseña
        usuario.PasswordHash = PasswordHasher.HashPassword(nuevaPassword);
        usuario.FechaModificacion = DateTime.Now;

        // Desbloquear y resetear intentos de login
        usuario.Bloqueado = false;
        usuario.IntentosFallidos = 0;

        // 📊 AUDITORÍA: Registrar la IP para trazabilidad
        Console.WriteLine($"🔐 Contraseña restablecida con código | Usuario: {usuario.Username} (ID: {usuario.IdUsuario}) | IP: {ipAddress ?? "No registrada"}");

        await _usuarioRepository.UpdateAsync(usuario);
        await _usuarioRepository.SaveChangesAsync();

        // 9. Marcar el código como usado
        await _codigoRepository.MarcarComoUsadoAsync(codigo);

        // 10. Invalidar otros códigos activos del usuario
        await _codigoRepository.InvalidarCodigosActivosAsync(usuario.IdUsuario);

        return true;
    }

    // ========================================
    // UTILIDADES
    // ========================================

    /// <summary>
    /// Validar credenciales sin registrar el login
    /// </summary>
    public async Task<bool> ValidarCredencialesAsync(string usernameOrEmail, string password)
    {
        var usuario = await _usuarioRepository.GetByUsernameOrEmailAsync(usernameOrEmail);

        if (usuario == null)
            return false;

        return PasswordHasher.VerifyPassword(password, usuario.PasswordHash);
    }
}