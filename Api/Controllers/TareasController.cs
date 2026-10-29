using Api.DTO;
using Api.Helpers;
using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR; // Asegúrate de que esto esté si lo usas
using API.Hubs; // Asegúrate de que esto esté si lo usas
using Microsoft.EntityFrameworkCore; // Asegúrate de que esto esté si lo usas
using System.Linq; // Necesario para Select

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly ITareaRepository _tareaRepository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly IComplejidadRepository _complejidadRepository;
        private readonly IPrioridadRepository _prioridadRepository;
        private readonly IUsuariosRepository _usuarioRepository;
        private readonly ITareaIncumplimientoRepository _incumplimientoRepository;
        private readonly ITareaJustificacionRechazoRepository _justificacionRechazoRepository;
        private readonly ITareaSeguimientoRepository _tareaSeguimientoRepository;

        public TareasController(
            ITareaRepository tareaRepository,
            IEstadoRepository estadoRepository,
            IComplejidadRepository complejidadRepository,
            IPrioridadRepository prioridadRepository,
            IUsuariosRepository usuarioRepository,
            ITareaIncumplimientoRepository incumplimientoRepository,
            ITareaJustificacionRechazoRepository justificacionRechazoRepository,
            ITareaSeguimientoRepository tareaSeguimientoRepository)
        {
            _tareaRepository = tareaRepository;
            _estadoRepository = estadoRepository;
            _complejidadRepository = complejidadRepository;
            _prioridadRepository = prioridadRepository;
            _usuarioRepository = usuarioRepository;
            _incumplimientoRepository = incumplimientoRepository;
            _justificacionRechazoRepository = justificacionRechazoRepository;
            _tareaSeguimientoRepository = tareaSeguimientoRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var tarea = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (tarea == null)
                return NotFound($"No se encontró la tarea con ID {id}");

            var dto = MapearTareaADTO(tarea);
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tareas = await _tareaRepository.ObtenerTareasAsync();
            var dtoList = tareas.Select(MapearTareaADTO);
            return Ok(dtoList);
        }

        [HttpGet("activas/usuario")]
        public async Task<IActionResult> ObtenerTareasActivasDelUsuarioAutenticado()
        {
            var idUsuario = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);

            if (idUsuario == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            var tareas = await _tareaRepository.ObtenerTareasActivasPorUsuarioAsync(idUsuario.Value);
            var dtoList = tareas.Select(MapearTareaADTO);

            return Ok(dtoList);
        }

        [HttpGet("finalizadas/usuario")]
        public async Task<IActionResult> ObtenerTareasFinalizadasDelUsuarioAutenticado()
        {
            var idUsuario = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);

            if (idUsuario == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            var tareas = await _tareaRepository.ObtenerTareasFinalizadasPorUsuarioAsync(idUsuario.Value);
            var dtoList = tareas.Select(MapearTareaADTO);

            return Ok(dtoList);
        }

        [HttpGet("usuario")]
        public async Task<IActionResult> ObtenerTareasDelUsuarioAutenticado()
        {
            var idUsuario = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);

            if (idUsuario == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            var tareas = await _tareaRepository.ObtenerTareasPorUsuarioAsync(idUsuario.Value);
            var dtoList = tareas.Select(MapearTareaADTO);

            return Ok(dtoList);
        }

        [HttpGet("subtareas/usuario")]
        public async Task<IActionResult> ObtenerSubTareasDelUsuarioAutenticado()
        {
            var idUsuario = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);

            if (idUsuario == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            var subtareas = await _tareaRepository.ObtenerSubTareasPorUsuarioAsync(idUsuario.Value);

            var dtoList = subtareas.Select(MapearTareaADTO);

            return Ok(dtoList);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ETarea tarea)
        {
            if (tarea == null)
                return BadRequest("Datos inválidos.");

            var idUsuarioCreador = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);
            if (idUsuarioCreador == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            tarea.CN_Usuario_creador = idUsuarioCreador.Value;
            tarea.CF_Fecha_asignacion = DateTime.UtcNow;

            await _tareaRepository.CrearTareaAsync(tarea);

            return CreatedAtAction(nameof(GetPorId), new { id = tarea.CN_Id_tarea }, tarea);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] TareaActualizarDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var tareaExistente = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (tareaExistente == null)
                return NotFound("La tarea no existe.");

            if (dto.CN_Id_estado <= 0 || dto.CN_Id_complejidad <= 0 ||
                dto.CN_Id_prioridad <= 0 || dto.CN_Id_usuario_creador <= 0 ||
                (dto.CN_Id_usuario_asignado.HasValue && dto.CN_Id_usuario_asignado.Value <= 0))
            {
                return BadRequest("Uno o más campos obligatorios tienen valores inválidos.");
            }

            var estado = await _estadoRepository.ObtenerPorIdAsync(dto.CN_Id_estado);
            if (estado == null) return BadRequest($"Estado con ID {dto.CN_Id_estado} no existe.");

            var complejidad = await _complejidadRepository.ObtenerPorIdAsync(dto.CN_Id_complejidad);
            if (complejidad == null) return BadRequest($"Complejidad con ID {dto.CN_Id_complejidad} no existe.");

            var prioridad = await _prioridadRepository.ObtenerPorIdAsync(dto.CN_Id_prioridad);
            if (prioridad == null) return BadRequest($"Prioridad con ID {dto.CN_Id_prioridad} no existe.");

            var usuarioCreador = await _usuarioRepository.BuscarUsuario(dto.CN_Id_usuario_creador);
            if (usuarioCreador == null) return BadRequest($"Usuario creador con ID {dto.CN_Id_usuario_creador} no existe.");

            EUsuario? usuarioAsignado = null;
            if (dto.CN_Id_usuario_asignado.HasValue)
            {
                usuarioAsignado = await _usuarioRepository.BuscarUsuario(dto.CN_Id_usuario_asignado.Value);
                if (usuarioAsignado == null)
                    return BadRequest($"Usuario asignado con ID {dto.CN_Id_usuario_asignado} no existe.");
            }

            tareaExistente.CN_Tarea_origen = dto.CN_Tarea_origen;
            tareaExistente.CT_Titulo_tarea = dto.CT_Titulo_tarea;
            tareaExistente.CT_Descripcion_tarea = dto.CT_Descripcion_tarea;
            tareaExistente.CT_Descripcion_espera = dto.CT_Descripcion_espera;
            tareaExistente.CN_Numero_GIS = dto.CN_Numero_GIS;
            tareaExistente.CF_Fecha_asignacion = dto.CF_Fecha_asignacion;
            tareaExistente.CF_Fecha_limite = dto.CF_Fecha_limite;
            tareaExistente.CF_Fecha_finalizacion = dto.CF_Fecha_finalizacion;

            tareaExistente.Estado = estado;
            tareaExistente.Complejidad = complejidad;
            tareaExistente.Prioridad = prioridad;
            tareaExistente.UsuarioCreador = usuarioCreador;
            tareaExistente.UsuarioAsignado = usuarioAsignado;

            await _tareaRepository.ActualizarTareaAsync(tareaExistente);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existente = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (existente == null)
                return NotFound($"No se encontró la tarea con ID {id}");

            await _tareaRepository.EliminarTareaAsync(id);
            return Ok();
        }

        [HttpPut("asignar/{idTarea}")]
        public async Task<IActionResult> AsignarTarea(int idTarea, [FromQuery] int idAsignado)
        {
            var idAsignador = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);
            if (idAsignador == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            var tarea = await _tareaRepository.ObtenerTareaPorIdAsync(idTarea);
            if (tarea == null)
                return NotFound($"Tarea con ID {idTarea} no existe.");

            if (tarea.CN_Id_estado == 3)
                return BadRequest("No se puede asignar una tarea que ya está en proceso.");

            var usuarioAsignador = await _usuarioRepository.BuscarUsuarioConRol(idAsignador.Value);
            var usuarioAsignado = await _usuarioRepository.BuscarUsuarioConRol(idAsignado);

            if (usuarioAsignador == null || usuarioAsignado == null)
                return BadRequest("El usuario asignador o asignado no existe.");

            var jerarquiaAsignador = usuarioAsignador.Rol?.CN_Jerarquia ?? -1;
            var jerarquiaAsignado = usuarioAsignado.Rol?.CN_Jerarquia ?? -1;

            if (jerarquiaAsignador >= jerarquiaAsignado)
                return BadRequest("El usuario asignador debe tener una jerarquía superior al asignado.");

            tarea.CN_Usuario_asignado = idAsignado;
            tarea.CF_Fecha_asignacion = DateTime.Now;
            tarea.CN_Id_estado = 2;

            await _tareaRepository.ActualizarTareaAsync(tarea);

            return Ok($"Tarea ID {idTarea} asignada al usuario ID {idAsignado}.");
        }

        // En API.Controllers/TareasController.cs
        // Dentro del método CambiarEstado
        [HttpPatch("{id}/estado")]
        [Authorize]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] EstadoActualizarDTO dto)
        {
            var userId = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);
            if (userId == null)
                return Unauthorized("No se pudo identificar al usuario desde el token.");

            var tarea = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (tarea == null)
                return NotFound("Tarea no encontrada.");

            var estadoActual = tarea.CN_Id_estado;
            var nuevoEstado = dto.CN_Id_estado;

            // ===== VALIDACIONES =====
            // Casos donde solo el usuario asignado puede cambiar el estado
            if (nuevoEstado == 3 || nuevoEstado == 4 || nuevoEstado == 5)
            {
                if (tarea.CN_Usuario_asignado != userId)
                    return Forbid("Solo el usuario asignado puede realizar esta acción.");

                if (nuevoEstado == 4) // En espera
                {
                    if (string.IsNullOrWhiteSpace(dto.MotivoEspera))
                        return BadRequest("Debe indicar una descripción de espera para el estado 'En espera'.");
                    tarea.CT_Descripcion_espera = dto.MotivoEspera;
                }
                else
                {
                    // Limpiar la descripción de espera si el estado no es 'En espera'
                    tarea.CT_Descripcion_espera = null;
                }

                tarea.CN_Id_estado = nuevoEstado;
            }
            // Casos donde solo el usuario creador puede cambiar el estado (Finalizada, Rechazada, Incumplida)
            else if (nuevoEstado == 6 || nuevoEstado == 7 || nuevoEstado == 9)
            {
                if (tarea.CN_Usuario_creador != userId)
                    return Forbid("Solo el usuario que creó la tarea puede cambiarla a este estado.");

                // Validación adicional: Solo puede pasar a estos estados desde "En revisión" (estado 5)
                if (estadoActual != 5)
                    return BadRequest("La tarea debe estar en estado 'En revisión' para poder finalizarla, rechazarla o marcarla como incumplida.");

                tarea.CN_Id_estado = nuevoEstado;
                // Si la tarea se finaliza, rechaza o se marca como incumplida, se establece la fecha de finalización
                tarea.CF_Fecha_finalizacion = DateTime.UtcNow;

                // --- INICIO DE CAMBIO: Manejo especial: Estado 6 = RECHAZADA ---
                if (nuevoEstado == 6)
                {
                    if (string.IsNullOrWhiteSpace(dto.DescripcionRechazo))
                        return BadRequest("Debe proporcionar una descripción de rechazo para el estado 'Rechazada'.");

                    var nuevaJustificacionRechazo = new ETareaJustificacionRechazo
                    {
                        CN_Id_tarea = tarea.CN_Id_tarea,
                        CT_Descripcion_rechazo = dto.DescripcionRechazo,
                        CF_Fecha_hora_rechazo = DateTime.UtcNow
                    };
                    await _justificacionRechazoRepository.CrearRechazoAsync(nuevaJustificacionRechazo);
                }
                // --- FIN DE CAMBIO ---
                // Manejo especial: Estado 9 = INCUMPLIDA (este bloque ya existía, pero ahora es un 'else if' del anterior 'if')
                else if (nuevoEstado == 9)
                {
                    if (string.IsNullOrWhiteSpace(dto.JustificacionIncumplimiento))
                        return BadRequest("Debe proporcionar una justificación para marcar la tarea como incumplida.");

                    var nuevoIncumplimiento = new ETareaIncumplimiento
                    {
                        CN_Id_tarea = tarea.CN_Id_tarea,
                        CT_Justificacion_incumplimiento = dto.JustificacionIncumplimiento,
                        CF_Fecha_incumplimiento = DateTime.UtcNow
                    };
                    await _incumplimientoRepository.CrearIncumplimientoAsync(nuevoIncumplimiento);
                }
            }
            // --- INICIO DE CAMBIO: NUEVO CASO ESPECIAL: De Rechazada (6) a Asignada (2) - Reasignación ---
            else if (estadoActual == 6 && nuevoEstado == 2)
            {
                // Solo el creador puede reactivar una tarea rechazada
                if (tarea.CN_Usuario_creador != userId)
                    return Forbid("Solo el usuario que creó la tarea puede reactivar una tarea rechazada.");

                tarea.CN_Id_estado = nuevoEstado;
                tarea.CF_Fecha_finalizacion = null; // Limpiamos la fecha de finalización si se reactiva

                // Eliminar el registro de justificación de rechazo asociado
                var justificacionExistente = await _justificacionRechazoRepository.ObtenerPorTareaIdAsync(tarea.CN_Id_tarea);
                if (justificacionExistente != null)
                {
                    await _justificacionRechazoRepository.EliminarRechazoAsync(justificacionExistente);
                }
            }
            // --- FIN DE CAMBIO ---
            // Caso especial: De Incumplida (9) a Asignada (2) (Este bloque ya existía)
            else if (estadoActual == 9 && nuevoEstado == 2)
            {
                // Solo el creador puede reactivar una tarea incumplida
                if (tarea.CN_Usuario_creador != userId)
                    return Forbid("Solo el usuario que creó la tarea puede reactivar una tarea incumplida.");

                tarea.CN_Id_estado = nuevoEstado;
                tarea.CF_Fecha_finalizacion = null; // Limpiamos la fecha de finalización si se reactiva

                // Eliminar el registro de incumplimiento asociado, ya que la tarea ya no está incumplida
                var incumplimientoExistente = await _incumplimientoRepository.ObtenerPorTareaIdAsync(tarea.CN_Id_tarea);
                if (incumplimientoExistente != null)
                {
                    await _incumplimientoRepository.EliminarIncumplimientoAsync(incumplimientoExistente);
                }
            }
            else
            {
                return BadRequest("El cambio de estado solicitado no está permitido.");
            }

            // Guardar los cambios en la tarea principal
            await _tareaRepository.ActualizarTareaAsync(tarea);
            return NoContent();
        }

        [HttpPost("{id}/seguimiento")]
        [Authorize]
        public async Task<IActionResult> AgregarSeguimiento(int id, [FromBody] TareaSeguimientoCreacionDTO dto)
        {
            var userId = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);
            if (userId == null)
            {
                return Unauthorized("No se pudo identificar al usuario desde el token.");
            }

            var tarea = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada.");
            }

            if (tarea.CN_Usuario_creador != userId.Value)
            {
                return Forbid("Solo el creador de la tarea puede agregar seguimiento.");
            }

            if (tarea.CN_Id_estado != 3 && tarea.CN_Id_estado != 4)
            {
                return BadRequest("Solo se puede agregar seguimiento a tareas en estado 'En proceso' o 'En espera'.");
            }

            var nuevoSeguimiento = new ETareaSeguimiento
            {
                CN_Id_tarea = id,
                CT_Comentario = dto.CT_Comentario,
                CF_Fecha_seguimiento = DateTime.UtcNow
            };

            await _tareaSeguimientoRepository.CrearSeguimientoAsync(nuevoSeguimiento);

            return NoContent();
        }

        // NUEVO ENDPOINT: Obtener seguimientos de una tarea
        [HttpGet("{id}/seguimientos")] // Endpoint para obtener todos los seguimientos de una tarea
        public async Task<IActionResult> GetSeguimientosPorTareaId(int id)
        {
            var tarea = await _tareaRepository.ObtenerTareaPorIdAsync(id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada.");
            }

            var seguimientos = await _tareaSeguimientoRepository.ObtenerSeguimientosPorTareaIdAsync(id);

            // Mapear a DTOs para la respuesta
            var dtoList = seguimientos.Select(s => new TareaSeguimientoDTO
            {
                CN_Id_seguimiento = s.CN_Id_seguimiento,
                CN_Id_tarea = s.CN_Id_tarea,
                CT_Comentario = s.CT_Comentario,
                CF_Fecha_seguimiento = s.CF_Fecha_seguimiento
            }).ToList(); // Convertir a lista para asegurar la enumeración

            return Ok(dtoList);
        }
        private TareaDTO MapearTareaADTO(ETarea tarea)
        {
            return new TareaDTO
            {
                CN_Id_tarea = tarea.CN_Id_tarea,
                CN_Tarea_origen = tarea.CN_Tarea_origen,
                CT_Titulo_tarea = tarea.CT_Titulo_tarea,
                CT_Descripcion_tarea = tarea.CT_Descripcion_tarea,
                CT_Descripcion_espera = tarea.CT_Descripcion_espera,
                CN_Numero_GIS = tarea.CN_Numero_GIS,
                CF_Fecha_asignacion = tarea.CF_Fecha_asignacion,
                CF_Fecha_limite = tarea.CF_Fecha_limite,
                CF_Fecha_finalizacion = tarea.CF_Fecha_finalizacion,
                Estado = tarea.Estado != null ? new EstadoDTO
                {
                    CN_Id_estado = tarea.Estado.CN_Id_estado,
                    CT_Estado = tarea.Estado.CT_Estado,
                    CT_Descripcion = tarea.Estado.CT_Descripcion
                } : null,
                Complejidad = tarea.Complejidad != null ? new ComplejidadDTO
                {
                    CN_Id_complejidad = tarea.Complejidad.CN_Id_complejidad,
                    CT_Nombre = tarea.Complejidad.CT_Nombre
                } : null,
                Prioridad = tarea.Prioridad != null ? new PrioridadDTO
                {
                    CN_Id_prioridad = tarea.Prioridad.CN_Id_prioridad,
                    CT_Descripcion_prioridad = tarea.Prioridad.CT_Descripcion_prioridad
                } : null,
                UsuarioCreador = tarea.UsuarioCreador != null ? new UsuarioDTO
                {
                    CN_Id_usuario = tarea.UsuarioCreador.CN_Id_usuario,
                    CT_Nombre_usuario = tarea.UsuarioCreador.CT_Nombre_usuario
                } : null,
                UsuarioAsignado = tarea.UsuarioAsignado != null ? new UsuarioDTO
                {
                    CN_Id_usuario = tarea.UsuarioAsignado.CN_Id_usuario,
                    CT_Nombre_usuario = tarea.UsuarioAsignado.CT_Nombre_usuario
                } : null
            };
        }
    }
}