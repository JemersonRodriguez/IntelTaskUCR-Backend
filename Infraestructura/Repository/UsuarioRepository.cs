using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class UsuarioRepository : IUsuariosRepository
    {
        private readonly IntelTaskUCRDbContext _dbContext;

        public UsuarioRepository(IntelTaskUCRDbContext _context) { 
            _dbContext = _context;
        }

        public async Task<IEnumerable<EUsuario>> ListarUsuarios()
        {
            return await _dbContext.T_Usuarios
                                 .Include(u => u.Rol)
                                 .ToListAsync();
        }
        public async Task ActualizarUsuario(EUsuario usuario)
        {
            _dbContext.T_Usuarios.Update(usuario);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CrearUsuario(EUsuario usuario)
        {
            await _dbContext.T_Usuarios.AddAsync(usuario);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EUsuario?> BuscarUsuario(int id)
        {
            return await _dbContext.T_Usuarios
                                 .Include(u => u.Rol)
                                 .FirstOrDefaultAsync(u => u.CN_Id_usuario == id);
        }

        public async Task EliminarUsuario(int id)
        {
            var entidad = await _dbContext.T_Usuarios.FindAsync(id);
            if (entidad != null) {
                _dbContext.T_Usuarios.Remove(entidad);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<EUsuario?> BuscarUsuarioConRol(int id)
        {
            return await _dbContext.T_Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CN_Id_usuario == id);
        }

    }
}
