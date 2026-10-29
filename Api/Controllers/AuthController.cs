using Microsoft.AspNetCore.Mvc;
using Dominio.Interfaces;
using Api.DTO;
using Utilidades; // Asegúrate de importar tu utilidad JwtUtils
using Api.Helpers;
using Microsoft.AspNetCore.Authorization; // <- Importante para usar [Authorize]

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authService;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly JwtUtils _jwtUtils;

        public AuthController(IAuthRepository authService, JwtUtils jwtUtils, IUsuariosRepository usuariosRepository, IRolesRepository rolesRepository)
        {
            _authService = authService;
            _jwtUtils = jwtUtils;
            _usuariosRepository = usuariosRepository;
            _rolesRepository = rolesRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Correo) || string.IsNullOrWhiteSpace(loginDto.Contrasenna))
                return BadRequest(new { mensaje = "Correo y contraseña son requeridos" });

            var usuario = await _authService.ValidarCredencialesAsync(loginDto.Correo, loginDto.Contrasenna);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            // Generar el token JWT
            var token = _jwtUtils.GenerarToken(usuario);

            // Crear DTO limpio
            var usuarioDTO = new UsuarioDTO
            {
                CN_Id_usuario = usuario.CN_Id_usuario,
                CT_Nombre_usuario = usuario.CT_Nombre_usuario,
                CT_Correo_usuario = usuario.CT_Correo_usuario,
                Rol = new RolDTO
                {
                    CN_Id_rol = usuario.Rol?.CN_Id_rol ?? 0,
                    CT_Nombre_rol = usuario.Rol?.CT_Nombre_rol ?? "Sin rol",
                    CN_Jerarquia = usuario.Rol?.CN_Jerarquia ?? 0
                }
            };

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso",
                token,
                usuario = usuarioDTO
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> ObtenerUsuarioActual()
        {
            var userId = JwtClaimsUtils.ObtenerUsuarioId(HttpContext);

            if (userId == null)
                return Unauthorized("Token inválido o expirado.");

            // Aquí obtienes el usuario con su rol incluido
            var usuario = await _usuariosRepository.BuscarUsuarioConRol(userId.Value);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var usuarioDto = new
            {
                Id = usuario.CN_Id_usuario,
                Nombre = usuario.CT_Nombre_usuario,
                Correo = usuario.CT_Correo_usuario,
                FechaNacimiento = usuario.CF_Fecha_nacimiento,
                Rol = usuario.Rol != null ? new { usuario.Rol.CN_Id_rol, usuario.Rol.CT_Nombre_rol } : null
            };

            return Ok(usuarioDto);
        }

    }
}
