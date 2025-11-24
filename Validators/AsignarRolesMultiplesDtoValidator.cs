using FluentValidation;
using G2rismBeta.API.DTOs.UsuarioRol;
using G2rismBeta.API.Constants;

namespace G2rismBeta.API.Validators;

/// <summary>
/// Validador para AsignarRolesMultiplesDto
/// Realiza validaciones síncronas de estructura y formato
/// Las validaciones de negocio complejas (BD) se realizan en el servicio
/// </summary>
public class AsignarRolesMultiplesDtoValidator : AbstractValidator<AsignarRolesMultiplesDto>
{
    public AsignarRolesMultiplesDtoValidator()
    {
        // ========================================
        // VALIDACIÓN 1: Lista de roles no vacía
        // ========================================

        RuleFor(x => x.RolesIds)
            .NotNull()
            .WithMessage("La lista de roles no puede ser nula")
            .NotEmpty()
            .WithMessage("Debe proporcionar al menos un rol");

        // ========================================
        // VALIDACIÓN 2: No roles duplicados
        // ========================================

        RuleFor(x => x.RolesIds)
            .Must(roles => roles == null || roles.Count == roles.Distinct().Count())
            .WithMessage("La lista de roles contiene IDs duplicados");

        // ========================================
        // VALIDACIÓN 3: IDs de roles deben ser válidos (mayores a 0)
        // ========================================

        RuleFor(x => x.RolesIds)
            .Must(roles => roles == null || roles.All(id => id > 0))
            .WithMessage("Todos los IDs de roles deben ser mayores a 0");

        // ========================================
        // VALIDACIÓN 4: No mezclar roles incompatibles
        // ========================================

        RuleFor(x => x.RolesIds)
            .Must(rolesIds =>
            {
                if (rolesIds == null || !rolesIds.Any())
                    return true;

                // Un usuario no puede tener roles de "empleado" y "cliente" al mismo tiempo
                var tieneRolEmpleado = rolesIds.Any(id =>
                    id == RoleConstants.SUPER_ADMINISTRADOR_ID ||
                    id == RoleConstants.ADMINISTRADOR_ID ||
                    id == RoleConstants.EMPLEADO_ID);

                var tieneRolCliente = rolesIds.Contains(RoleConstants.CLIENTE_ID);

                // No pueden estar ambos presentes
                return !(tieneRolEmpleado && tieneRolCliente);
            })
            .WithMessage("No se pueden mezclar roles de empleado (Super Administrador, Administrador, Empleado) con roles de Cliente");

        // ========================================
        // VALIDACIÓN 5: Límite de roles por usuario
        // ========================================

        RuleFor(x => x.RolesIds)
            .Must(roles => roles == null || roles.Count <= 10)
            .WithMessage("No se pueden asignar más de 10 roles a un usuario");

        // ========================================
        // NOTA: Validaciones que requieren BD se hacen en UsuarioService:
        // - Existencia de roles en la base de datos
        // - Estado activo de los roles
        // - Compatibilidad con tipo de usuario
        // - Restricción de Súper Administrador único
        // ========================================
    }
}
