using AutoMapper;
using G2rismBeta.API.DTOs.Proveedor;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services
{
    /// <summary>
    /// Servicio de Proveedores
    /// Contiene toda la lógica de negocio para la gestión de proveedores
    /// </summary>
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(
            IProveedorRepository proveedorRepository,
            IMapper mapper,
            ILogger<ProveedorService> logger)
        {
            _proveedorRepository = proveedorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ========================================
        // OPERACIONES CRUD
        // ========================================

        /// <summary>
        /// Obtener todos los proveedores
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetAllAsync()
        {
            var proveedores = await _proveedorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener un proveedor por ID
        /// </summary>
        public async Task<ProveedorResponseDto?> GetByIdAsync(int id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
                return null;

            var dto = _mapper.Map<ProveedorResponseDto>(proveedor);
            
            // Calcular contratos activos
            if (proveedor.Contratos != null)
            {
                var hoy = DateTime.Now;
                dto.ContratosActivos = proveedor.Contratos
                    .Count(c => c.Estado == "Vigente" && c.FechaFin >= hoy);
                dto.TieneContratosVigentes = dto.ContratosActivos > 0;
            }

            return dto;
        }

        /// <summary>
        /// Crear un nuevo proveedor
        /// </summary>
        public async Task<ProveedorResponseDto> CreateAsync(ProveedorCreateDto dto)
        {
            // Validación 1: NIT/RUT único
            if (await _proveedorRepository.ExisteNitRutAsync(dto.NitRut))
            {
                throw new InvalidOperationException($"Ya existe un proveedor con el NIT/RUT {dto.NitRut}");
            }

            // Validación 2: Tipo de proveedor válido
            var tiposValidos = new[] { "Hotel", "Aerolinea", "Transporte", "Servicios", "Mixto" };
            if (!tiposValidos.Contains(dto.TipoProveedor))
            {
                throw new ArgumentException($"Tipo de proveedor '{dto.TipoProveedor}' no es válido. Valores permitidos: {string.Join(", ", tiposValidos)}");
            }

            // Validación 3: Estado válido
            var estadosValidos = new[] { "Activo", "Inactivo", "Suspendido" };
            if (!estadosValidos.Contains(dto.Estado))
            {
                throw new ArgumentException($"Estado '{dto.Estado}' no es válido. Valores permitidos: {string.Join(", ", estadosValidos)}");
            }

            // Validación 4: Calificación en rango válido
            if (dto.Calificacion < 0.0m || dto.Calificacion > 5.0m)
            {
                throw new ArgumentException("La calificación debe estar entre 0.0 y 5.0");
            }

            // Mapear y crear
            var proveedor = _mapper.Map<Proveedor>(dto);
            proveedor.FechaRegistro = DateTime.Now;

            var proveedorCreado = await _proveedorRepository.AddAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            _logger.LogInformation($"Proveedor creado exitosamente: {proveedorCreado.NombreEmpresa} (ID: {proveedorCreado.IdProveedor})");

            return _mapper.Map<ProveedorResponseDto>(proveedorCreado);
        }

        /// <summary>
        /// Actualizar un proveedor existente
        /// </summary>
        public async Task<ProveedorResponseDto> UpdateAsync(int id, ProveedorUpdateDto dto)
        {
            var proveedorExistente = await _proveedorRepository.GetByIdAsync(id);
            if (proveedorExistente == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado");
            }

            // Validación 1: Si se actualiza el NIT/RUT, verificar que sea único
            if (!string.IsNullOrWhiteSpace(dto.NitRut) && dto.NitRut != proveedorExistente.NitRut)
            {
                if (await _proveedorRepository.ExisteNitRutAsync(dto.NitRut, id))
                {
                    throw new InvalidOperationException($"Ya existe otro proveedor con el NIT/RUT {dto.NitRut}");
                }
                proveedorExistente.NitRut = dto.NitRut;
            }

            // Validación 2: Tipo de proveedor válido
            if (!string.IsNullOrWhiteSpace(dto.TipoProveedor))
            {
                var tiposValidos = new[] { "Hotel", "Aerolinea", "Transporte", "Servicios", "Mixto" };
                if (!tiposValidos.Contains(dto.TipoProveedor))
                {
                    throw new ArgumentException($"Tipo de proveedor '{dto.TipoProveedor}' no es válido");
                }
                proveedorExistente.TipoProveedor = dto.TipoProveedor;
            }

            // Validación 3: Estado válido
            if (!string.IsNullOrWhiteSpace(dto.Estado))
            {
                var estadosValidos = new[] { "Activo", "Inactivo", "Suspendido" };
                if (!estadosValidos.Contains(dto.Estado))
                {
                    throw new ArgumentException($"Estado '{dto.Estado}' no es válido");
                }
                proveedorExistente.Estado = dto.Estado;
            }

            // Validación 4: Calificación en rango válido
            if (dto.Calificacion.HasValue)
            {
                if (dto.Calificacion.Value < 0.0m || dto.Calificacion.Value > 5.0m)
                {
                    throw new ArgumentException("La calificación debe estar entre 0.0 y 5.0");
                }
                proveedorExistente.Calificacion = dto.Calificacion.Value;
            }

            // Actualizar solo los campos que vienen en el DTO
            if (!string.IsNullOrWhiteSpace(dto.NombreEmpresa))
                proveedorExistente.NombreEmpresa = dto.NombreEmpresa;

            if (!string.IsNullOrWhiteSpace(dto.NombreContacto))
                proveedorExistente.NombreContacto = dto.NombreContacto;

            if (!string.IsNullOrWhiteSpace(dto.Telefono))
                proveedorExistente.Telefono = dto.Telefono;

            if (dto.TelefonoAlternativo != null)
                proveedorExistente.TelefonoAlternativo = dto.TelefonoAlternativo;

            if (!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
                proveedorExistente.CorreoElectronico = dto.CorreoElectronico;

            if (dto.CorreoAlternativo != null)
                proveedorExistente.CorreoAlternativo = dto.CorreoAlternativo;

            if (dto.Direccion != null)
                proveedorExistente.Direccion = dto.Direccion;

            if (!string.IsNullOrWhiteSpace(dto.Ciudad))
                proveedorExistente.Ciudad = dto.Ciudad;

            if (!string.IsNullOrWhiteSpace(dto.Pais))
                proveedorExistente.Pais = dto.Pais;

            if (!string.IsNullOrWhiteSpace(dto.SitioWeb))
                proveedorExistente.SitioWeb = dto.SitioWeb;

            if (dto.Observaciones != null)
                proveedorExistente.Observaciones = dto.Observaciones;

            await _proveedorRepository.UpdateAsync(proveedorExistente);
            await _proveedorRepository.SaveChangesAsync();

            _logger.LogInformation($"Proveedor actualizado exitosamente: ID {id}");

            return _mapper.Map<ProveedorResponseDto>(proveedorExistente);
        }

        /// <summary>
        /// Eliminar un proveedor
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado");
            }

            // Validación: No eliminar proveedores con contratos vigentes
            if (proveedor.Contratos != null && proveedor.Contratos.Any())
            {
                var hoy = DateTime.Now;
                var tieneContratosVigentes = proveedor.Contratos
                    .Any(c => c.Estado == "Vigente" && c.FechaFin >= hoy);

                if (tieneContratosVigentes)
                {
                    throw new InvalidOperationException(
                        "No se puede eliminar el proveedor porque tiene contratos vigentes. " +
                        "Por favor, cancele o finalice los contratos antes de eliminar el proveedor.");
                }
            }

            await _proveedorRepository.DeleteAsync(id);
            await _proveedorRepository.SaveChangesAsync();

            _logger.LogInformation($"Proveedor eliminado exitosamente: ID {id}");

            return true;
        }

        // ========================================
        // BÚSQUEDAS Y FILTROS
        // ========================================

        /// <summary>
        /// Buscar proveedor por NIT/RUT
        /// </summary>
        public async Task<ProveedorResponseDto?> GetByNitRutAsync(string nitRut)
        {
            var proveedor = await _proveedorRepository.GetByNitRutAsync(nitRut);
            if (proveedor == null)
                return null;

            var dto = _mapper.Map<ProveedorResponseDto>(proveedor);

            // Calcular contratos activos
            if (proveedor.Contratos != null)
            {
                var hoy = DateTime.Now;
                dto.ContratosActivos = proveedor.Contratos
                    .Count(c => c.Estado == "Vigente" && c.FechaFin >= hoy);
                dto.TieneContratosVigentes = dto.ContratosActivos > 0;
            }

            return dto;
        }

        /// <summary>
        /// Buscar proveedores por nombre
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> SearchByNombreAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre de búsqueda no puede estar vacío");
            }

            var proveedores = await _proveedorRepository.SearchByNombreAsync(nombre);
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener proveedores por tipo (case-insensitive)
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetByTipoAsync(string tipo)
        {
            var tiposValidos = new[] { "Hotel", "Aerolinea", "Transporte", "Servicios", "Mixto" };

            // Normalizar el tipo a formato capitalizado (primera letra mayúscula)
            var tipoNormalizado = string.IsNullOrWhiteSpace(tipo)
                ? string.Empty
                : char.ToUpper(tipo[0]) + tipo.Substring(1).ToLower();

            if (!tiposValidos.Contains(tipoNormalizado))
            {
                throw new ArgumentException($"Tipo '{tipo}' no es válido. Valores permitidos: {string.Join(", ", tiposValidos)}");
            }

            var proveedores = await _proveedorRepository.GetByTipoAsync(tipoNormalizado);
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener proveedores activos
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetActivosAsync()
        {
            var proveedores = await _proveedorRepository.GetActivosAsync();
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener proveedores por ciudad
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetByCiudadAsync(string ciudad)
        {
            if (string.IsNullOrWhiteSpace(ciudad))
            {
                throw new ArgumentException("La ciudad no puede estar vacía");
            }

            var proveedores = await _proveedorRepository.GetByCiudadAsync(ciudad);
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener proveedores por calificación mínima
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetByCalificacionMinimaAsync(decimal calificacionMinima)
        {
            if (calificacionMinima < 0.0m || calificacionMinima > 5.0m)
            {
                throw new ArgumentException("La calificación mínima debe estar entre 0.0 y 5.0");
            }

            var proveedores = await _proveedorRepository.GetByCalificacionMinimaAsync(calificacionMinima);
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        // ========================================
        // GESTIÓN DE ESTADO Y CALIFICACIÓN
        // ========================================

        /// <summary>
        /// Cambiar estado de un proveedor
        /// </summary>
        public async Task<ProveedorResponseDto> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado");
            }

            var estadosValidos = new[] { "Activo", "Inactivo", "Suspendido" };
            if (!estadosValidos.Contains(nuevoEstado))
            {
                throw new ArgumentException($"Estado '{nuevoEstado}' no es válido. Valores permitidos: {string.Join(", ", estadosValidos)}");
            }

            // No permitir inactivar/suspender proveedor con contratos vigentes
            if ((nuevoEstado == "Inactivo" || nuevoEstado == "Suspendido") && proveedor.Contratos != null)
            {
                var hoy = DateTime.Now;
                var tieneContratosVigentes = proveedor.Contratos
                    .Any(c => c.Estado == "Vigente" && c.FechaFin >= hoy);

                if (tieneContratosVigentes)
                {
                    throw new InvalidOperationException(
                        "No se puede cambiar el estado del proveedor a Inactivo/Suspendido " +
                        "porque tiene contratos vigentes.");
                }
            }

            proveedor.Estado = nuevoEstado;
            await _proveedorRepository.UpdateAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            _logger.LogInformation($"Estado del proveedor ID {id} cambiado a: {nuevoEstado}");

            return _mapper.Map<ProveedorResponseDto>(proveedor);
        }

        /// <summary>
        /// Actualizar calificación de un proveedor
        /// </summary>
        public async Task<ProveedorResponseDto> ActualizarCalificacionAsync(int id, decimal nuevaCalificacion)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado");
            }

            if (nuevaCalificacion < 0.0m || nuevaCalificacion > 5.0m)
            {
                throw new ArgumentException("La calificación debe estar entre 0.0 y 5.0");
            }

            proveedor.Calificacion = nuevaCalificacion;
            await _proveedorRepository.UpdateAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            _logger.LogInformation($"Calificación del proveedor ID {id} actualizada a: {nuevaCalificacion}");

            return _mapper.Map<ProveedorResponseDto>(proveedor);
        }

        // ========================================
        // ESTADÍSTICAS Y REPORTES
        // ========================================

        /// <summary>
        /// Obtener top proveedores mejor calificados
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetTopProveedoresAsync(int cantidad = 10)
        {
            if (cantidad <= 0 || cantidad > 100)
            {
                throw new ArgumentException("La cantidad debe estar entre 1 y 100");
            }

            var proveedores = await _proveedorRepository.GetTopProveedoresAsync(cantidad);
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }

        /// <summary>
        /// Obtener estadísticas de proveedores por tipo
        /// </summary>
        public async Task<Dictionary<string, int>> GetEstadisticasPorTipoAsync()
        {
            return await _proveedorRepository.CountByTipoAsync();
        }

        /// <summary>
        /// Obtener proveedores con contratos vigentes
        /// </summary>
        public async Task<IEnumerable<ProveedorResponseDto>> GetConContratosVigentesAsync()
        {
            var proveedores = await _proveedorRepository.GetConContratosVigentesAsync();
            return _mapper.Map<IEnumerable<ProveedorResponseDto>>(proveedores);
        }
    }
}