using Api.DTO;
using Api.Helpers;
using Dominio.Entidades;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilidades;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuariosRepository _usuariosRepositorio;
        private readonly IRolesRepository _rolesRepositorio;

        public UsuarioController(IUsuariosRepository repositorio, IRolesRepository rolesRepositorio)
        {
            _usuariosRepositorio = repositorio;
            _rolesRepositorio = rolesRepositorio;
        }

        // Listar todos los usuarios
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarios = await _usuariosRepositorio.ListarUsuarios();

            var resultado = usuarios.Select(u => new UsuarioDTO
            {
                CN_Id_usuario = u.CN_Id_usuario,
                CT_Nombre_usuario = u.CT_Nombre_usuario,
                CT_Correo_usuario = u.CT_Correo_usuario,
                Rol = new RolDTO
                {
                    CN_Id_rol = u.Rol.CN_Id_rol,
                    CT_Nombre_rol = u.Rol.CT_Nombre_rol,
                    CN_Jerarquia = u.Rol.CN_Jerarquia
                }
            });

            return Ok(resultado);
        }

        // Buscar usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var usuario = await _usuariosRepositorio.BuscarUsuario(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var dto = new UsuarioDTO
            {
                CN_Id_usuario = usuario.CN_Id_usuario,
                CT_Nombre_usuario = usuario.CT_Nombre_usuario,
                CT_Correo_usuario = usuario.CT_Correo_usuario,
                Rol = new RolDTO
                {
                    CN_Id_rol = usuario.Rol.CN_Id_rol,
                    CT_Nombre_rol = usuario.Rol.CT_Nombre_rol,
                    CN_Jerarquia = usuario.Rol.CN_Jerarquia
                }
            };

            return Ok(dto);
        }

        // Crear un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioCrearDTO dto)
        {
            if (dto == null)
                return BadRequest("El usuario no puede ser nulo.");

            var rol = await _rolesRepositorio.BuscarRol(dto.CN_Id_rol);
            if (rol == null)
                return BadRequest($"El rol con ID {dto.CN_Id_rol} no existe.");

            var nuevoUsuario = new EUsuario
            {
                CN_Id_usuario = dto.CN_Id_Usuario,
                CT_Nombre_usuario = dto.CT_Nombre_usuario,
                CT_Correo_usuario = dto.CT_Correo_usuario,
                CF_Fecha_nacimiento = dto.CF_Fecha_nacimiento,

                // Aquí se hashea la contraseña usando la utilidad
                CT_Contrasenna = PasswordUtils.HashPassword(dto.CT_Contrasenna),

                CB_Estado_usuario = dto.CB_Estado_usuario,
                CN_Id_rol = dto.CN_Id_rol,
                CF_Fecha_creacion_usuario = DateTime.UtcNow,
                CF_Fecha_modificacion_usuario = DateTime.UtcNow
            };

            await _usuariosRepositorio.CrearUsuario(nuevoUsuario);

            var resultado = new UsuarioDTO
            {
                CN_Id_usuario = nuevoUsuario.CN_Id_usuario,
                CT_Nombre_usuario = nuevoUsuario.CT_Nombre_usuario,
                CT_Correo_usuario = nuevoUsuario.CT_Correo_usuario,
                Rol = new RolDTO
                {
                    CN_Id_rol = rol.CN_Id_rol,
                    CT_Nombre_rol = rol.CT_Nombre_rol,
                    CN_Jerarquia = rol.CN_Jerarquia
                }
            };

            return CreatedAtAction(nameof(Buscar), new { id = resultado.CN_Id_usuario }, resultado);
        }


        // Actualizar usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioCrearDTO dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var existente = await _usuariosRepositorio.BuscarUsuario(id);
            if (existente == null)
                return NotFound("El usuario no existe.");

            var rol = await _rolesRepositorio.BuscarRol(dto.CN_Id_rol);
            if (rol == null)
                return BadRequest($"El rol con ID {dto.CN_Id_rol} no existe.");

            existente.CT_Nombre_usuario = dto.CT_Nombre_usuario;
            existente.CT_Correo_usuario = dto.CT_Correo_usuario;
            existente.CN_Id_rol = dto.CN_Id_rol;

            // Aquí actualizamos solo la fecha de modificación
            existente.CF_Fecha_modificacion_usuario = DateTime.UtcNow;

            await _usuariosRepositorio.ActualizarUsuario(existente);
            return NoContent();
        }

        [HttpGet("subordinados")]
        public async Task<IActionResult> ListarSubordinados()
        {
            // Obtener el ID y rol del usuario autenticado
            var idUsuario = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);
            var nombreRol = JwtClaimsUtils.ObtenerRol(HttpContext);

            if (idUsuario == null)
                return Unauthorized("Token inválido o usuario no autenticado.");

            if (string.IsNullOrEmpty(nombreRol))
                return Unauthorized("No se pudo obtener el rol desde el token.");

            // Buscar el rol por nombre para obtener la jerarquía
            var rolSolicitante = await _rolesRepositorio.BuscarRolPorNombre(nombreRol);
            if (rolSolicitante == null)
                return BadRequest("Rol no encontrado.");

            int jerarquiaSolicitante = rolSolicitante.CN_Jerarquia;

            // Obtener todos los usuarios
            var usuarios = await _usuariosRepositorio.ListarUsuarios();

            // Filtrar usuarios con igual o menor jerarquía y excluir al solicitante
            var filtrados = usuarios
                .Where(u =>
                    u.Rol != null &&
                    u.Rol.CN_Jerarquia >= jerarquiaSolicitante &&
                    u.CN_Id_usuario != idUsuario.Value)
                .Select(u => new UsuarioDTO
                {
                    CN_Id_usuario = u.CN_Id_usuario,
                    CT_Nombre_usuario = u.CT_Nombre_usuario,
                    CT_Correo_usuario = u.CT_Correo_usuario,
                    Rol = new RolDTO
                    {
                        CN_Id_rol = u.Rol.CN_Id_rol,
                        CT_Nombre_rol = u.Rol.CT_Nombre_rol,
                        CN_Jerarquia = u.Rol.CN_Jerarquia
                    }
                });

            return Ok(filtrados);
        }

        [HttpPut("{id}/cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena(int id, [FromBody] CambiarContrasenaDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NuevaContrasena))
                return BadRequest("La nueva contraseña es obligatoria.");

            var usuario = await _usuariosRepositorio.BuscarUsuario(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Hashear la nueva contraseña
            usuario.CT_Contrasenna = PasswordUtils.HashPassword(dto.NuevaContrasena);
            usuario.CF_Fecha_modificacion_usuario = DateTime.UtcNow;

            await _usuariosRepositorio.ActualizarUsuario(usuario);

            return NoContent();
        }
    }
}
