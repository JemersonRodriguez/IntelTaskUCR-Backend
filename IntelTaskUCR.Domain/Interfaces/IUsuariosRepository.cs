using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IUsuariosRepository
    {
        Task<IEnumerable<EUsuario>> ListarUsuarios();
        Task EliminarUsuario(int id);
        Task ActualizarUsuario(EUsuario usuario);
        Task CrearUsuario(EUsuario usuario);
        Task<EUsuario?> BuscarUsuario(int id);
        Task<EUsuario?> BuscarUsuarioConRol(int id);
    }
}
