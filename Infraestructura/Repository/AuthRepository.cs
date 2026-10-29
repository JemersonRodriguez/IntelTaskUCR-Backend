using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;
using Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository

{
    public class AuthRepository : IAuthRepository
    {
        private readonly IntelTaskUCRDbContext _dbContext;

        public AuthRepository(IntelTaskUCRDbContext _dbContext) { 
            this._dbContext = _dbContext;
        }

        public async Task<EUsuario?> ValidarCredencialesAsync(string correo, string contrasenna)
        {
            var usuario = await _dbContext.T_Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CT_Correo_usuario == correo);

            if (usuario == null)
                return null;

            // Usar util de BCrypt para comparar
            bool contrasennaValida = PasswordUtils.VerifyPassword(contrasenna, usuario.CT_Contrasenna);
            if (!contrasennaValida)
                return null;

            return usuario;
        }

    }
}
