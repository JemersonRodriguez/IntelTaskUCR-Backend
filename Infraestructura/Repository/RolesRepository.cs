using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly IntelTaskUCRDbContext _dbContext;

        public RolesRepository(IntelTaskUCRDbContext _context) { 
            this._dbContext = _context;
        }

        public async Task<ERol?> BuscarRol(int id)
        {
            return await _dbContext.T_Roles.FindAsync(id);
        }

        public async Task<ERol?> ObtenerRolPorUsuarioIdAsync(int usuarioId)
        {
            var usuario = await _dbContext.T_Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CN_Id_usuario == usuarioId);

            return usuario?.Rol;
        }

        public async Task<ERol?> BuscarRolPorNombre(string nombreRol)
        {
            return await _dbContext.T_Roles
                .FirstOrDefaultAsync(r => r.CT_Nombre_rol == nombreRol);
        }
    }
}
