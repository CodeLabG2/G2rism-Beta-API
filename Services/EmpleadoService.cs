using AutoMapper;
using G2rismBeta.API.DTOs.Empleado;
using G2rismBeta.API.Interfaces;
using G2rismBeta.API.Models;

namespace G2rismBeta.API.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Empleados
    /// Implementa validaciones complejas, gestión de jerarquía organizacional y operaciones especiales
    /// </summary>
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public EmpleadoService(
            IEmpleadoRepository empleadoRepository,
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
        {
            _empleadoRepository = empleadoRepository;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        // ========================================
        // OPERACIONES CRUD BÁSICAS
        // ========================================

        /// <summary>
        /// Obtener todos los empleados
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetAllEmpleadosAsync()
        {
            var empleados = await _empleadoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Obtener solo empleados activos
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosActivosAsync()
        {
            var empleados = await _empleadoRepository.GetEmpleadosPorEstadoAsync("activo");
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Obtener un empleado por su ID
        /// </summary>
        public async Task<EmpleadoResponseDto?> GetEmpleadoByIdAsync(int idEmpleado)
        {
            var empleado = await _empleadoRepository.GetEmpleadoConRelacionesAsync(idEmpleado);

            if (empleado == null)
                return null;

            return _mapper.Map<EmpleadoResponseDto>(empleado);
        }

        /// <summary>
        /// Obtener empleado con información completa (incluye jefe y subordinados)
        /// </summary>
        public async Task<EmpleadoResponseDto?> GetEmpleadoCompletoAsync(int idEmpleado)
        {
            var empleado = await _empleadoRepository.GetEmpleadoCompletoAsync(idEmpleado);

            if (empleado == null)
                return null;

            return _mapper.Map<EmpleadoResponseDto>(empleado);
        }

        /// <summary>
        /// Crear un nuevo empleado
        /// </summary>
        public async Task<EmpleadoResponseDto> CreateEmpleadoAsync(EmpleadoCreateDto empleadoCreateDto)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Validar que el usuario existe
            var usuario = await _usuarioRepository.GetByIdAsync(empleadoCreateDto.IdUsuario);
            if (usuario == null)
            {
                throw new InvalidOperationException($"No existe un usuario con ID {empleadoCreateDto.IdUsuario}");
            }

            // 2. Validar que el usuario no esté ya asociado a otro empleado
            var usuarioTieneEmpleado = await _empleadoRepository.UsuarioTieneEmpleadoAsync(empleadoCreateDto.IdUsuario);
            if (usuarioTieneEmpleado)
            {
                throw new InvalidOperationException("Este usuario ya está asociado a otro empleado");
            }

            // 3. Validar que el documento de identidad sea único
            var documentoExiste = await _empleadoRepository.ExisteDocumentoAsync(empleadoCreateDto.DocumentoIdentidad);
            if (documentoExiste)
            {
                throw new InvalidOperationException($"Ya existe un empleado con el documento de identidad {empleadoCreateDto.DocumentoIdentidad}");
            }

            // 4. Validar que el jefe existe (si se proporciona)
            if (empleadoCreateDto.IdJefe.HasValue)
            {
                var jefe = await _empleadoRepository.GetByIdAsync(empleadoCreateDto.IdJefe.Value);
                if (jefe == null)
                {
                    throw new InvalidOperationException($"No existe un empleado con ID {empleadoCreateDto.IdJefe.Value} para asignar como jefe");
                }
            }

            // 5. Validar que la fecha de nacimiento sea válida (mayor de 18 años)
            var edad = DateTime.Now.Year - empleadoCreateDto.FechaNacimiento.Year;
            if (empleadoCreateDto.FechaNacimiento > DateTime.Now.AddYears(-edad)) edad--;

            if (edad < 18)
            {
                throw new InvalidOperationException("El empleado debe ser mayor de 18 años");
            }

            // 6. Validar que la fecha de ingreso no sea futura
            if (empleadoCreateDto.FechaIngreso > DateTime.Now)
            {
                throw new InvalidOperationException("La fecha de ingreso no puede ser una fecha futura");
            }

            // ====================================
            // CREAR EMPLEADO
            // ====================================

            var empleado = _mapper.Map<Empleado>(empleadoCreateDto);
            await _empleadoRepository.AddAsync(empleado);
            await _empleadoRepository.SaveChangesAsync();

            // Recargar el empleado con sus relaciones para retornar datos completos
            var empleadoCreado = await _empleadoRepository.GetEmpleadoConRelacionesAsync(empleado.IdEmpleado);

            return _mapper.Map<EmpleadoResponseDto>(empleadoCreado);
        }

        /// <summary>
        /// Actualizar un empleado existente
        /// </summary>
        public async Task<EmpleadoResponseDto> UpdateEmpleadoAsync(int idEmpleado, EmpleadoUpdateDto empleadoUpdateDto)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleadoExistente = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleadoExistente == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Si se cambia el usuario, validar que existe y no esté asociado a otro empleado
            if (empleadoUpdateDto.IdUsuario.HasValue)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(empleadoUpdateDto.IdUsuario.Value);
                if (usuario == null)
                {
                    throw new InvalidOperationException($"No existe un usuario con ID {empleadoUpdateDto.IdUsuario.Value}");
                }

                var usuarioTieneEmpleado = await _empleadoRepository.UsuarioTieneEmpleadoAsync(
                    empleadoUpdateDto.IdUsuario.Value,
                    idEmpleado);

                if (usuarioTieneEmpleado)
                {
                    throw new InvalidOperationException("Este usuario ya está asociado a otro empleado");
                }
            }

            // 3. Si se cambia el documento, validar que sea único
            if (!string.IsNullOrEmpty(empleadoUpdateDto.DocumentoIdentidad))
            {
                var documentoExiste = await _empleadoRepository.ExisteDocumentoAsync(
                    empleadoUpdateDto.DocumentoIdentidad,
                    idEmpleado);

                if (documentoExiste)
                {
                    throw new InvalidOperationException($"Ya existe otro empleado con el documento de identidad {empleadoUpdateDto.DocumentoIdentidad}");
                }
            }

            // 4. Si se cambia el jefe, validar que existe y no crea ciclos
            if (empleadoUpdateDto.IdJefe.HasValue)
            {
                var jefe = await _empleadoRepository.GetByIdAsync(empleadoUpdateDto.IdJefe.Value);
                if (jefe == null)
                {
                    throw new InvalidOperationException($"No existe un empleado con ID {empleadoUpdateDto.IdJefe.Value} para asignar como jefe");
                }

                // Validar que no se creen ciclos en la jerarquía
                var puedeSerJefe = await _empleadoRepository.PuedeSerJefeDeAsync(empleadoUpdateDto.IdJefe.Value, idEmpleado);
                if (!puedeSerJefe)
                {
                    throw new InvalidOperationException("No se puede asignar este jefe porque crearía un ciclo en la jerarquía organizacional");
                }
            }

            // 5. Validar fecha de nacimiento si se actualiza
            if (empleadoUpdateDto.FechaNacimiento.HasValue)
            {
                var edad = DateTime.Now.Year - empleadoUpdateDto.FechaNacimiento.Value.Year;
                if (empleadoUpdateDto.FechaNacimiento.Value > DateTime.Now.AddYears(-edad)) edad--;

                if (edad < 18)
                {
                    throw new InvalidOperationException("El empleado debe ser mayor de 18 años");
                }
            }

            // 6. Validar fecha de ingreso si se actualiza
            if (empleadoUpdateDto.FechaIngreso.HasValue && empleadoUpdateDto.FechaIngreso.Value > DateTime.Now)
            {
                throw new InvalidOperationException("La fecha de ingreso no puede ser una fecha futura");
            }

            // ====================================
            // ACTUALIZAR EMPLEADO
            // ====================================

            // Mapear solo las propiedades que no son null en el DTO
            if (empleadoUpdateDto.IdUsuario.HasValue)
                empleadoExistente.IdUsuario = empleadoUpdateDto.IdUsuario.Value;

            if (empleadoUpdateDto.IdJefe.HasValue)
                empleadoExistente.IdJefe = empleadoUpdateDto.IdJefe;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.Nombre))
                empleadoExistente.Nombre = empleadoUpdateDto.Nombre;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.Apellido))
                empleadoExistente.Apellido = empleadoUpdateDto.Apellido;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.DocumentoIdentidad))
                empleadoExistente.DocumentoIdentidad = empleadoUpdateDto.DocumentoIdentidad;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.TipoDocumento))
                empleadoExistente.TipoDocumento = empleadoUpdateDto.TipoDocumento;

            if (empleadoUpdateDto.FechaNacimiento.HasValue)
                empleadoExistente.FechaNacimiento = empleadoUpdateDto.FechaNacimiento.Value;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.CorreoElectronico))
                empleadoExistente.CorreoElectronico = empleadoUpdateDto.CorreoElectronico;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.Telefono))
                empleadoExistente.Telefono = empleadoUpdateDto.Telefono;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.Cargo))
                empleadoExistente.Cargo = empleadoUpdateDto.Cargo;

            if (empleadoUpdateDto.FechaIngreso.HasValue)
                empleadoExistente.FechaIngreso = empleadoUpdateDto.FechaIngreso.Value;

            if (empleadoUpdateDto.Salario.HasValue)
                empleadoExistente.Salario = empleadoUpdateDto.Salario.Value;

            if (!string.IsNullOrEmpty(empleadoUpdateDto.Estado))
                empleadoExistente.Estado = empleadoUpdateDto.Estado;

            await _empleadoRepository.UpdateAsync(empleadoExistente);
            await _empleadoRepository.SaveChangesAsync();

            // Recargar el empleado con sus relaciones para retornar datos completos
            var empleadoActualizado = await _empleadoRepository.GetEmpleadoConRelacionesAsync(idEmpleado);

            return _mapper.Map<EmpleadoResponseDto>(empleadoActualizado);
        }

        /// <summary>
        /// Eliminar un empleado (solo si no tiene subordinados)
        /// Si tiene subordinados, primero deben ser reasignados
        /// </summary>
        public async Task<bool> DeleteEmpleadoAsync(int idEmpleado)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleado = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Verificar que no tenga subordinados
            var tieneSubordinados = await _empleadoRepository.TieneSubordinadosAsync(idEmpleado);
            if (tieneSubordinados)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el empleado porque tiene subordinados. " +
                    "Primero debe reasignar los subordinados a otro jefe.");
            }

            // ====================================
            // ELIMINAR EMPLEADO
            // ====================================

            await _empleadoRepository.DeleteAsync(idEmpleado);
            await _empleadoRepository.SaveChangesAsync();

            return true;
        }

        // ========================================
        // OPERACIONES DE BÚSQUEDA
        // ========================================

        /// <summary>
        /// Buscar empleado por documento de identidad
        /// </summary>
        public async Task<EmpleadoResponseDto?> GetEmpleadoByDocumentoAsync(string documentoIdentidad)
        {
            var empleado = await _empleadoRepository.GetByDocumentoAsync(documentoIdentidad);

            if (empleado == null)
                return null;

            return _mapper.Map<EmpleadoResponseDto>(empleado);
        }

        /// <summary>
        /// Buscar empleado por ID de usuario
        /// </summary>
        public async Task<EmpleadoResponseDto?> GetEmpleadoByUsuarioIdAsync(int idUsuario)
        {
            var empleado = await _empleadoRepository.GetByUsuarioIdAsync(idUsuario);

            if (empleado == null)
                return null;

            return _mapper.Map<EmpleadoResponseDto>(empleado);
        }

        /// <summary>
        /// Buscar empleados por nombre o apellido
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> BuscarEmpleadosPorNombreAsync(string termino)
        {
            var empleados = await _empleadoRepository.BuscarPorNombreAsync(termino);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Obtener empleados por cargo
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosPorCargoAsync(string cargo)
        {
            var empleados = await _empleadoRepository.GetEmpleadosPorCargoAsync(cargo);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Obtener empleados por estado (Activo, Inactivo, Vacaciones, Licencia)
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosPorEstadoAsync(string estado)
        {
            var empleados = await _empleadoRepository.GetEmpleadosPorEstadoAsync(estado);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        // ========================================
        // OPERACIONES DE JERARQUÍA
        // ========================================

        /// <summary>
        /// Obtener todos los subordinados directos de un empleado
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetSubordinadosDirectosAsync(int idJefe)
        {
            var subordinados = await _empleadoRepository.GetSubordinadosDirectosAsync(idJefe);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(subordinados);
        }

        /// <summary>
        /// Obtener todos los subordinados de un empleado (todos los niveles)
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetTodosLosSubordinadosAsync(int idJefe)
        {
            var subordinados = await _empleadoRepository.GetTodosLosSubordinadosAsync(idJefe);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(subordinados);
        }

        /// <summary>
        /// Obtener el jefe directo de un empleado
        /// </summary>
        public async Task<EmpleadoResponseDto?> GetJefeDirectoAsync(int idEmpleado)
        {
            var jefe = await _empleadoRepository.GetJefeDirectoAsync(idEmpleado);

            if (jefe == null)
                return null;

            return _mapper.Map<EmpleadoResponseDto>(jefe);
        }

        /// <summary>
        /// Obtener la cadena de jefes hasta el nivel más alto
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetCadenaDeJefesAsync(int idEmpleado)
        {
            var cadenaJefes = await _empleadoRepository.GetCadenaDeJefesAsync(idEmpleado);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(cadenaJefes);
        }

        /// <summary>
        /// Obtener empleados sin jefe (nivel más alto)
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosSinJefeAsync()
        {
            var empleados = await _empleadoRepository.GetEmpleadosSinJefeAsync();
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Obtener empleados que son jefes (tienen subordinados)
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosQuesonJefesAsync()
        {
            var jefes = await _empleadoRepository.GetEmpleadosQuesonJefesAsync();
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(jefes);
        }

        /// <summary>
        /// Obtener el organigrama completo de la empresa
        /// Estructura jerárquica completa
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetOrganigramaCompletoAsync()
        {
            var organigrama = await _empleadoRepository.GetOrganigramaCompletoAsync();
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(organigrama);
        }

        // ========================================
        // OPERACIONES DE GESTIÓN DE JERARQUÍA
        // ========================================

        /// <summary>
        /// Cambiar el jefe de un empleado
        /// Valida que no se creen ciclos en la jerarquía
        /// </summary>
        public async Task<bool> CambiarJefeAsync(int idEmpleado, int? idNuevoJefe)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleado = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Si se asigna un nuevo jefe, validar que existe
            if (idNuevoJefe.HasValue)
            {
                var nuevoJefe = await _empleadoRepository.GetByIdAsync(idNuevoJefe.Value);
                if (nuevoJefe == null)
                {
                    throw new InvalidOperationException($"No existe un empleado con ID {idNuevoJefe.Value}");
                }

                // 3. Validar que no se creen ciclos en la jerarquía
                var puedeSerJefe = await _empleadoRepository.PuedeSerJefeDeAsync(idNuevoJefe.Value, idEmpleado);
                if (!puedeSerJefe)
                {
                    throw new InvalidOperationException(
                        "No se puede asignar este jefe porque crearía un ciclo en la jerarquía organizacional. " +
                        "Un empleado no puede ser jefe de alguien que está en su línea de reporte.");
                }
            }

            // ====================================
            // CAMBIAR JEFE
            // ====================================

            return await _empleadoRepository.CambiarJefeAsync(idEmpleado, idNuevoJefe);
        }

        /// <summary>
        /// Reasignar subordinados de un jefe a otro
        /// Útil antes de eliminar o cambiar el cargo de un jefe
        /// </summary>
        public async Task<bool> ReasignarSubordinadosAsync(int idJefeActual, int? idNuevoJefe)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el jefe actual existe
            var jefeActual = await _empleadoRepository.GetByIdAsync(idJefeActual);
            if (jefeActual == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idJefeActual}");
            }

            // 2. Verificar que el jefe actual tenga subordinados
            var tieneSubordinados = await _empleadoRepository.TieneSubordinadosAsync(idJefeActual);
            if (!tieneSubordinados)
            {
                throw new InvalidOperationException($"El empleado con ID {idJefeActual} no tiene subordinados para reasignar");
            }

            // 3. Si se asigna un nuevo jefe, validar que existe
            if (idNuevoJefe.HasValue)
            {
                var nuevoJefe = await _empleadoRepository.GetByIdAsync(idNuevoJefe.Value);
                if (nuevoJefe == null)
                {
                    throw new InvalidOperationException($"No existe un empleado con ID {idNuevoJefe.Value} para asignar como nuevo jefe");
                }
            }

            // ====================================
            // REASIGNAR SUBORDINADOS
            // ====================================

            return await _empleadoRepository.ReasignarSubordinadosAsync(idJefeActual, idNuevoJefe);
        }

        /// <summary>
        /// Promover un empleado (cambiar cargo y posiblemente jefe)
        /// </summary>
        public async Task<bool> PromoverEmpleadoAsync(int idEmpleado, string nuevoCargo, int? idNuevoJefe = null, decimal? nuevoSalario = null)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleado = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Validar que el cargo no esté vacío
            if (string.IsNullOrWhiteSpace(nuevoCargo))
            {
                throw new InvalidOperationException("El nuevo cargo no puede estar vacío");
            }

            // 3. Si se asigna un nuevo jefe, validar que existe y no crea ciclos
            if (idNuevoJefe.HasValue)
            {
                var nuevoJefe = await _empleadoRepository.GetByIdAsync(idNuevoJefe.Value);
                if (nuevoJefe == null)
                {
                    throw new InvalidOperationException($"No existe un empleado con ID {idNuevoJefe.Value}");
                }

                var puedeSerJefe = await _empleadoRepository.PuedeSerJefeDeAsync(idNuevoJefe.Value, idEmpleado);
                if (!puedeSerJefe)
                {
                    throw new InvalidOperationException("No se puede asignar este jefe porque crearía un ciclo en la jerarquía");
                }
            }

            // 4. Validar que el nuevo salario sea positivo (si se proporciona)
            if (nuevoSalario.HasValue && nuevoSalario.Value <= 0)
            {
                throw new InvalidOperationException("El nuevo salario debe ser mayor a cero");
            }

            // ====================================
            // PROMOVER EMPLEADO
            // ====================================

            empleado.Cargo = nuevoCargo;

            if (idNuevoJefe.HasValue)
            {
                empleado.IdJefe = idNuevoJefe;
            }

            if (nuevoSalario.HasValue)
            {
                empleado.Salario = nuevoSalario.Value;
            }

            await _empleadoRepository.UpdateAsync(empleado);
            await _empleadoRepository.SaveChangesAsync();

            return true;
        }

        // ========================================
        // OPERACIONES ESPECIALES
        // ========================================

        /// <summary>
        /// Cambiar el estado de un empleado (Activo/Inactivo/Vacaciones/Licencia)
        /// </summary>
        public async Task<bool> CambiarEstadoEmpleadoAsync(int idEmpleado, string nuevoEstado)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleado = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Validar que el estado sea válido
            var estadosValidos = new[] { "activo", "inactivo", "vacaciones", "licencia" };
            if (!estadosValidos.Contains(nuevoEstado.ToLower()))
            {
                throw new InvalidOperationException(
                    $"Estado inválido. Los estados válidos son: {string.Join(", ", estadosValidos)}");
            }

            // ====================================
            // CAMBIAR ESTADO
            // ====================================

            return await _empleadoRepository.CambiarEstadoAsync(idEmpleado, nuevoEstado);
        }

        /// <summary>
        /// Actualizar el salario de un empleado
        /// (Operación separada por ser información sensible)
        /// </summary>
        public async Task<bool> ActualizarSalarioAsync(int idEmpleado, decimal nuevoSalario)
        {
            // ====================================
            // VALIDACIONES DE NEGOCIO
            // ====================================

            // 1. Verificar que el empleado existe
            var empleado = await _empleadoRepository.GetByIdAsync(idEmpleado);
            if (empleado == null)
            {
                throw new InvalidOperationException($"No existe un empleado con ID {idEmpleado}");
            }

            // 2. Validar que el salario sea positivo
            if (nuevoSalario <= 0)
            {
                throw new InvalidOperationException("El salario debe ser mayor a cero");
            }

            // 3. Validar que el salario no sea menor al salario mínimo (opcional, ajustar según país)
            const decimal SALARIO_MINIMO = 1300000; // Ejemplo: Colombia 2024
            if (nuevoSalario < SALARIO_MINIMO)
            {
                throw new InvalidOperationException($"El salario no puede ser menor al salario mínimo legal ({SALARIO_MINIMO:C})");
            }

            // ====================================
            // ACTUALIZAR SALARIO
            // ====================================

            empleado.Salario = nuevoSalario;
            await _empleadoRepository.UpdateAsync(empleado);
            await _empleadoRepository.SaveChangesAsync();

            return true;
        }

        // ========================================
        // VALIDACIONES
        // ========================================

        /// <summary>
        /// Validar si un documento de identidad ya existe
        /// </summary>
        public async Task<bool> DocumentoExisteAsync(string documentoIdentidad, int? idEmpleadoExcluir = null)
        {
            return await _empleadoRepository.ExisteDocumentoAsync(documentoIdentidad, idEmpleadoExcluir);
        }

        /// <summary>
        /// Validar si un usuario ya tiene un empleado asociado
        /// </summary>
        public async Task<bool> UsuarioTieneEmpleadoAsync(int idUsuario, int? idEmpleadoExcluir = null)
        {
            return await _empleadoRepository.UsuarioTieneEmpleadoAsync(idUsuario, idEmpleadoExcluir);
        }

        /// <summary>
        /// Validar si un empleado puede ser jefe de otro
        /// (Previene ciclos: A no puede ser jefe de B si B ya es jefe de A)
        /// </summary>
        public async Task<bool> PuedeSerJefeDeAsync(int idNuevoJefe, int idEmpleado)
        {
            return await _empleadoRepository.PuedeSerJefeDeAsync(idNuevoJefe, idEmpleado);
        }

        /// <summary>
        /// Validar si un empleado tiene subordinados
        /// </summary>
        public async Task<bool> TieneSubordinadosAsync(int idEmpleado)
        {
            return await _empleadoRepository.TieneSubordinadosAsync(idEmpleado);
        }

        /// <summary>
        /// Validar si un empleado es jefe de otro (directo o indirecto)
        /// </summary>
        public async Task<bool> EsJefeDeAsync(int idPosibleJefe, int idEmpleado)
        {
            return await _empleadoRepository.EsJefeDeAsync(idPosibleJefe, idEmpleado);
        }

        // ========================================
        // ESTADÍSTICAS Y REPORTES
        // ========================================

        /// <summary>
        /// Obtener estadísticas de empleados por cargo
        /// </summary>
        public async Task<Dictionary<string, int>> GetEstadisticasPorCargoAsync()
        {
            return await _empleadoRepository.GetEstadisticasPorCargoAsync();
        }

        /// <summary>
        /// Obtener estadísticas de empleados por estado
        /// </summary>
        public async Task<Dictionary<string, int>> GetEstadisticasPorEstadoAsync()
        {
            return await _empleadoRepository.GetEstadisticasPorEstadoAsync();
        }

        /// <summary>
        /// Contar total de empleados activos
        /// </summary>
        public async Task<int> ContarEmpleadosActivosAsync()
        {
            var empleadosActivos = await _empleadoRepository.GetEmpleadosPorEstadoAsync("activo");
            return empleadosActivos.Count();
        }

        /// <summary>
        /// Obtener promedio de antigüedad de empleados
        /// </summary>
        public async Task<double> GetPromedioAntiguedadAsync()
        {
            return await _empleadoRepository.GetPromedioAntiguedadAsync();
        }

        /// <summary>
        /// Obtener empleados con mayor antigüedad
        /// </summary>
        public async Task<IEnumerable<EmpleadoResponseDto>> GetEmpleadosConMayorAntiguedadAsync(int cantidad = 10)
        {
            var empleados = await _empleadoRepository.GetEmpleadosConMayorAntiguedadAsync(cantidad);
            return _mapper.Map<IEnumerable<EmpleadoResponseDto>>(empleados);
        }

        /// <summary>
        /// Contar cuántos subordinados directos tiene un empleado
        /// </summary>
        public async Task<int> ContarSubordinadosDirectosAsync(int idJefe)
        {
            return await _empleadoRepository.ContarSubordinadosDirectosAsync(idJefe);
        }

        /// <summary>
        /// Contar cuántos subordinados totales tiene un empleado (todos los niveles)
        /// </summary>
        public async Task<int> ContarTodosLosSubordinadosAsync(int idJefe)
        {
            return await _empleadoRepository.ContarTodosLosSubordinadosAsync(idJefe);
        }
    }
}