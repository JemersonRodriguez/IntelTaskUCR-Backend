using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IRolesRepository
    {
        Task<ERol?> BuscarRol(int id);
        Task<ERol?> ObtenerRolPorUsuarioIdAsync(int usuarioId);
        Task<ERol?> BuscarRolPorNombre(string nombreRol);
    }
}
