using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IEstadoRepository
    {
        Task<IEnumerable<EEstado>> ObtenerTodosAsync();
        Task<EEstado?> ObtenerPorIdAsync(byte id);
    }
}
