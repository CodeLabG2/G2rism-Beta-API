using FluentValidation;
using G2rismBeta.API.DTOs.Proveedor;
using G2rismBeta.API.Interfaces;
using System.Text.RegularExpressions;

namespace G2rismBeta.API.Validators
{
    /// <summary>
    /// Validador para la creación de proveedores
    /// Implementa validaciones de formato y estructura (sincrónicas)
    /// Las validaciones de negocio (como unicidad) se realizan en el Service Layer
    /// </summary>
    public class ProveedorCreateDtoValidator : AbstractValidator<ProveedorCreateDto>
    {
        public ProveedorCreateDtoValidator()
        {
            // ========================================
            // VALIDACIONES DE NOMBRE DE EMPRESA
            // ========================================
            RuleFor(x => x.NombreEmpresa)
                .NotEmpty().WithMessage("El nombre de la empresa es obligatorio")
                .Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s\.\,\-&]+$")
                .WithMessage("El nombre contiene caracteres no permitidos");

            // ========================================
            // VALIDACIONES DE NOMBRE DE CONTACTO
            // ========================================
            RuleFor(x => x.NombreContacto)
                .NotEmpty().WithMessage("El nombre del contacto es obligatorio")
                .Length(3, 100).WithMessage("El nombre del contacto debe tener entre 3 y 100 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El nombre del contacto solo debe contener letras y espacios");

            // ========================================
            // VALIDACIONES DE TELÉFONOS
            // ========================================
            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .Length(7, 20).WithMessage("El teléfono debe tener entre 7 y 20 caracteres")
                .Matches(@"^[\d\s\+\-\(\)]+$")
                .WithMessage("El teléfono contiene caracteres no válidos");

            RuleFor(x => x.TelefonoAlternativo)
                .Length(7, 20).WithMessage("El teléfono alternativo debe tener entre 7 y 20 caracteres")
                .Matches(@"^[\d\s\+\-\(\)]+$")
                .WithMessage("El teléfono alternativo contiene caracteres no válidos")
                .When(x => !string.IsNullOrWhiteSpace(x.TelefonoAlternativo));

            // ========================================
            // VALIDACIONES DE CORREOS ELECTRÓNICOS
            // ========================================
            RuleFor(x => x.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio")
                .EmailAddress().WithMessage("El formato del correo electrónico no es válido")
                .MaximumLength(100).WithMessage("El correo no puede exceder 100 caracteres");

            RuleFor(x => x.CorreoAlternativo)
                .EmailAddress().WithMessage("El formato del correo alternativo no es válido")
                .MaximumLength(100).WithMessage("El correo alternativo no puede exceder 100 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.CorreoAlternativo));

            // ========================================
            // VALIDACIONES DE UBICACIÓN
            // ========================================
            RuleFor(x => x.Direccion)
                .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Direccion));

            RuleFor(x => x.Ciudad)
                .NotEmpty().WithMessage("La ciudad es obligatoria")
                .Length(2, 50).WithMessage("La ciudad debe tener entre 2 y 50 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("La ciudad solo debe contener letras y espacios");

            RuleFor(x => x.Pais)
                .NotEmpty().WithMessage("El país es obligatorio")
                .Length(2, 50).WithMessage("El país debe tener entre 2 y 50 caracteres")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
                .WithMessage("El país solo debe contener letras y espacios");

            // ========================================
            // VALIDACIONES DE NIT/RUT
            // ========================================
            // NOTA: La validación de unicidad se realiza en el Service Layer
            RuleFor(x => x.NitRut)
                .NotEmpty().WithMessage("El NIT/RUT es obligatorio")
                .Length(5, 20).WithMessage("El NIT/RUT debe tener entre 5 y 20 caracteres")
                .Matches(@"^[\d\-]+$")
                .WithMessage("El NIT/RUT solo debe contener números y guiones");

            // ========================================
            // VALIDACIONES DE TIPO DE PROVEEDOR
            // ========================================
            RuleFor(x => x.TipoProveedor)
                .NotEmpty().WithMessage("El tipo de proveedor es obligatorio")
                .Must(tipo => new[] { "Hotel", "Aerolinea", "Transporte", "Servicios", "Mixto" }.Contains(tipo))
                .WithMessage("El tipo de proveedor debe ser: Hotel, Aerolinea, Transporte, Servicios o Mixto");

            // ========================================
            // VALIDACIONES DE SITIO WEB
            // ========================================
            RuleFor(x => x.SitioWeb)
                .Must(BeAValidUrl)
                .WithMessage("El formato de la URL no es válido")
                .MaximumLength(200).WithMessage("La URL del sitio web no puede exceder 200 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.SitioWeb));

            // ========================================
            // VALIDACIONES DE CALIFICACIÓN
            // ========================================
            RuleFor(x => x.Calificacion)
                .InclusiveBetween(0.0m, 5.0m)
                .WithMessage("La calificación debe estar entre 0.0 y 5.0");

            // ========================================
            // VALIDACIONES DE ESTADO
            // ========================================
            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("El estado es obligatorio")
                .Must(estado => new[] { "Activo", "Inactivo", "Suspendido" }.Contains(estado))
                .WithMessage("El estado debe ser: Activo, Inactivo o Suspendido");

            // ========================================
            // VALIDACIONES DE OBSERVACIONES
            // ========================================
            RuleFor(x => x.Observaciones)
                .MaximumLength(1000).WithMessage("Las observaciones no pueden exceder 1000 caracteres")
                .When(x => !string.IsNullOrWhiteSpace(x.Observaciones));
        }

        /// <summary>
        /// Validar formato de URL
        /// </summary>
        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}