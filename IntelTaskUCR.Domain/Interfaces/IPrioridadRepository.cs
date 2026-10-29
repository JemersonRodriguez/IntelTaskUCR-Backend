using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IPrioridadRepository
    {
        Task<IEnumerable<EPrioridad>> ObtenerTodasAsync();
        Task<EPrioridad?> ObtenerPorIdAsync(byte id);
    }
}
