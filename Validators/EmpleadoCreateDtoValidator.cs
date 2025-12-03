using FluentValidation;
using G2rismBeta.API.DTOs.Empleado;

namespace G2rismBeta.API.Validators
{
    /// <summary>
    /// Validador para la creación de empleados
    /// Implementa SOLO validaciones síncronas (formato, rango, etc.)
    /// Las validaciones asíncronas (BD) se realizan en el Service Layer
    /// </summary>
    public class EmpleadoCreateDtoValidator : AbstractValidator<EmpleadoCreateDto>
    {
        public EmpleadoCreateDtoValidator()
        {
            // ========================================
            // VALIDACIONES DE ID_USUARIO
            // ========================================

            RuleFor(x => x.IdUsuario)
                .NotEmpty()
                .WithMessage("El ID de usuario es obligatorio")
                .GreaterThan(0)
                .WithMessage("El ID de usuario debe ser mayor a 0");

            // ========================================
            // VALIDACIONES DE ID_JEFE (OPCIONAL)
            // ========================================

            RuleFor(x => x.IdJefe)
                .GreaterThan(0)
                .WithMessage("El ID de jefe debe ser mayor a 0")
                .When(x => x.IdJefe.HasValue);

            // ========================================
            // VALIDACIONES DE NOMBRE
            // ========================================

            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio")
                .MinimumLength(2)
                .WithMessage("El nombre debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El nombre no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El nombre solo puede contener letras y espacios");

            // ========================================
            // VALIDACIONES DE APELLIDO
            // ========================================

            RuleFor(x => x.Apellido)
                .NotEmpty()
                .WithMessage("El apellido es obligatorio")
                .MinimumLength(2)
                .WithMessage("El apellido debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El apellido no puede exceder 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El apellido solo puede contener letras y espacios");

            // ========================================
            // VALIDACIONES DE DOCUMENTO DE IDENTIDAD
            // ========================================

            RuleFor(x => x.DocumentoIdentidad)
                .NotEmpty()
                .WithMessage("El documento de identidad es obligatorio")
                .MinimumLength(6)
                .WithMessage("El documento debe tener al menos 6 caracteres")
                .MaximumLength(20)
                .WithMessage("El documento no puede exceder 20 caracteres")
                .Matches(@"^[a-zA-Z0-9-]+$")
                .WithMessage("El documento solo puede contener letras, números y guiones");

            // ========================================
            // VALIDACIONES DE TIPO DE DOCUMENTO
            // ========================================

            RuleFor(x => x.TipoDocumento)
                .NotEmpty()
                .WithMessage("El tipo de documento es obligatorio")
                .Must(TipoDocumentoValido)
                .WithMessage("Tipo de documento inválido. Valores permitidos: CC, TI, CE, PASAPORTE, NIT");

            // ========================================
            // VALIDACIONES DE FECHA DE NACIMIENTO
            // ========================================

            RuleFor(x => x.FechaNacimiento)
                .NotEmpty()
                .WithMessage("La fecha de nacimiento es obligatoria")
                .LessThan(DateTime.Now)
                .WithMessage("La fecha de nacimiento no puede ser una fecha futura")
                .Must(EdadMinima18Años)
                .WithMessage("El empleado debe ser mayor de 18 años")
                .Must(EdadMaxima80Años)
                .WithMessage("La fecha de nacimiento indica una edad mayor a 80 años, por favor verifique");

            // ========================================
            // VALIDACIONES DE CORREO ELECTRÓNICO
            // ========================================

            RuleFor(x => x.CorreoElectronico)
                .NotEmpty()
                .WithMessage("El correo electrónico es obligatorio")
                .EmailAddress()
                .WithMessage("El formato del correo electrónico no es válido")
                .MaximumLength(100)
                .WithMessage("El correo electrónico no puede exceder 100 caracteres");

            // ========================================
            // VALIDACIONES DE TELÉFONO
            // ========================================

            RuleFor(x => x.Telefono)
                .NotEmpty()
                .WithMessage("El teléfono es obligatorio")
                .MinimumLength(7)
                .WithMessage("El teléfono debe tener al menos 7 dígitos")
                .MaximumLength(20)
                .WithMessage("El teléfono no puede exceder 20 caracteres")
                .Matches(@"^[\d\s\-\+\(\)]+$")
                .WithMessage("El teléfono solo puede contener números, espacios, guiones, + y paréntesis");

            // ========================================
            // VALIDACIONES DE CARGO
            // ========================================

            RuleFor(x => x.Cargo)
                .NotEmpty()
                .WithMessage("El cargo es obligatorio")
                .MinimumLength(2)
                .WithMessage("El cargo debe tener al menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("El cargo no puede exceder 100 caracteres");

            // ========================================
            // VALIDACIONES DE FECHA DE INGRESO
            // ========================================

            RuleFor(x => x.FechaIngreso)
                .NotEmpty()
                .WithMessage("La fecha de ingreso es obligatoria")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha de ingreso no puede ser una fecha futura")
                .Must((dto, fechaIngreso) => FechaIngresoCoherenteConEdad(dto.FechaNacimiento, fechaIngreso))
                .WithMessage("La fecha de ingreso debe ser posterior a los 15 años de edad");

            // ========================================
            // VALIDACIONES DE SALARIO
            // ========================================

            RuleFor(x => x.Salario)
                .NotEmpty()
                .WithMessage("El salario es obligatorio")
                .GreaterThan(0)
                .WithMessage("El salario debe ser mayor a cero")
                .GreaterThanOrEqualTo(1300000)
                .WithMessage("El salario debe ser al menos el salario mínimo legal vigente (COP $1,300,000)")
                .LessThanOrEqualTo(100000000)
                .WithMessage("El salario no puede exceder COP $100,000,000");

            // ========================================
            // VALIDACIONES DE ESTADO
            // ========================================

            RuleFor(x => x.Estado)
                .NotEmpty()
                .WithMessage("El estado es obligatorio")
                .Must(EstadoValido)
                .WithMessage("Estado inválido. Valores permitidos: Activo, Inactivo, Vacaciones, Licencia");
        }

        // ========================================
        // MÉTODOS DE VALIDACIÓN SÍNCRONOS
        // ========================================

        /// <summary>
        /// Validar que el tipo de documento sea uno de los valores permitidos
        /// </summary>
        private bool TipoDocumentoValido(string tipoDocumento)
        {
            var tiposValidos = new[] { "CC", "TI", "CE", "PASAPORTE", "NIT" };
            return tiposValidos.Contains(tipoDocumento.ToUpper());
        }

        /// <summary>
        /// Validar que el empleado tenga al menos 18 años de edad
        /// </summary>
        private bool EdadMinima18Años(DateTime fechaNacimiento)
        {
            var edad = DateTime.Now.Year - fechaNacimiento.Year;
            if (fechaNacimiento > DateTime.Now.AddYears(-edad))
                edad--;

            return edad >= 18;
        }

        /// <summary>
        /// Validar que la fecha de nacimiento no indique una edad mayor a 80 años
        /// (Prevención de errores de digitación)
        /// </summary>
        private bool EdadMaxima80Años(DateTime fechaNacimiento)
        {
            var edad = DateTime.Now.Year - fechaNacimiento.Year;
            if (fechaNacimiento > DateTime.Now.AddYears(-edad))
                edad--;

            return edad <= 80;
        }

        /// <summary>
        /// Validar que la fecha de ingreso sea coherente con la edad
        /// (No puede ingresar antes de los 15 años)
        /// </summary>
        private bool FechaIngresoCoherenteConEdad(DateTime fechaNacimiento, DateTime fechaIngreso)
        {
            var edadAlIngreso = fechaIngreso.Year - fechaNacimiento.Year;
            if (fechaNacimiento > fechaIngreso.AddYears(-edadAlIngreso))
                edadAlIngreso--;

            return edadAlIngreso >= 15; // Edad mínima legal para trabajar
        }

        /// <summary>
        /// Validar que el estado sea uno de los valores permitidos
        /// Acepta mayúsculas, minúsculas o capitalizado
        /// </summary>
        private bool EstadoValido(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return false;

            var estadosValidos = new[] { "activo", "inactivo", "vacaciones", "licencia" };
            return estadosValidos.Contains(estado.ToLower());
        }
    }
}