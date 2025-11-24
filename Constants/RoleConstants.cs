namespace G2rismBeta.API.Constants;

/// <summary>
/// Constantes para los roles del sistema
/// Define los IDs y nombres de roles predefinidos
/// </summary>
public static class RoleConstants
{
    // ========================================
    // IDS DE ROLES (según seeding en DbInitializer)
    // ========================================

    /// <summary>
    /// ID del rol Súper Administrador
    /// </summary>
    public const int SUPER_ADMINISTRADOR_ID = 1;

    /// <summary>
    /// ID del rol Administrador
    /// </summary>
    public const int ADMINISTRADOR_ID = 2;

    /// <summary>
    /// ID del rol Empleado
    /// </summary>
    public const int EMPLEADO_ID = 3;

    /// <summary>
    /// ID del rol Cliente
    /// </summary>
    public const int CLIENTE_ID = 4;

    // ========================================
    // NOMBRES DE ROLES
    // ========================================

    /// <summary>
    /// Nombre del rol Súper Administrador
    /// </summary>
    public const string SUPER_ADMINISTRADOR = "Super Administrador";

    /// <summary>
    /// Nombre del rol Administrador
    /// </summary>
    public const string ADMINISTRADOR = "Administrador";

    /// <summary>
    /// Nombre del rol Empleado
    /// </summary>
    public const string EMPLEADO = "Empleado";

    /// <summary>
    /// Nombre del rol Cliente
    /// </summary>
    public const string CLIENTE = "Cliente";

    // ========================================
    // TIPOS DE USUARIO
    // ========================================

    /// <summary>
    /// Tipo de usuario empleado
    /// </summary>
    public const string TIPO_EMPLEADO = "empleado";

    /// <summary>
    /// Tipo de usuario cliente
    /// </summary>
    public const string TIPO_CLIENTE = "cliente";

    // ========================================
    // NIVELES DE ACCESO
    // ========================================

    /// <summary>
    /// Nivel de acceso del Súper Administrador
    /// </summary>
    public const int NIVEL_SUPER_ADMIN = 1;

    /// <summary>
    /// Nivel de acceso del Administrador
    /// </summary>
    public const int NIVEL_ADMIN = 2;

    /// <summary>
    /// Nivel de acceso del Empleado
    /// </summary>
    public const int NIVEL_EMPLEADO = 10;

    /// <summary>
    /// Nivel de acceso del Cliente
    /// </summary>
    public const int NIVEL_CLIENTE = 50;

    // ========================================
    // CONJUNTOS DE ROLES
    // ========================================

    /// <summary>
    /// Roles administrativos (Súper Admin y Admin)
    /// </summary>
    public static readonly int[] ROLES_ADMINISTRATIVOS =
    {
        SUPER_ADMINISTRADOR_ID,
        ADMINISTRADOR_ID
    };

    /// <summary>
    /// Roles permitidos para usuarios tipo "empleado"
    /// </summary>
    public static readonly int[] ROLES_EMPLEADO =
    {
        SUPER_ADMINISTRADOR_ID,
        ADMINISTRADOR_ID,
        EMPLEADO_ID
    };

    /// <summary>
    /// Roles permitidos para usuarios tipo "cliente"
    /// </summary>
    public static readonly int[] ROLES_CLIENTE =
    {
        CLIENTE_ID
    };

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    /// <summary>
    /// Verifica si un rol es administrativo
    /// </summary>
    public static bool EsRolAdministrativo(int idRol)
    {
        return idRol == SUPER_ADMINISTRADOR_ID || idRol == ADMINISTRADOR_ID;
    }

    /// <summary>
    /// Verifica si un rol es el Súper Administrador
    /// </summary>
    public static bool EsSuperAdministrador(int idRol)
    {
        return idRol == SUPER_ADMINISTRADOR_ID;
    }

    /// <summary>
    /// Obtiene los roles permitidos según el tipo de usuario
    /// </summary>
    public static int[] GetRolesPermitidos(string tipoUsuario)
    {
        return tipoUsuario.ToLower() switch
        {
            TIPO_EMPLEADO => ROLES_EMPLEADO,
            TIPO_CLIENTE => ROLES_CLIENTE,
            _ => Array.Empty<int>()
        };
    }

    /// <summary>
    /// Verifica si un rol es válido para un tipo de usuario
    /// </summary>
    public static bool EsRolValidoParaTipoUsuario(int idRol, string tipoUsuario)
    {
        var rolesPermitidos = GetRolesPermitidos(tipoUsuario);
        return rolesPermitidos.Contains(idRol);
    }
}
