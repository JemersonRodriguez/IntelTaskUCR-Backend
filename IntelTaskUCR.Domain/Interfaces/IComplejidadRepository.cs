using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IComplejidadRepository
    {
        Task<IEnumerable<EComplejidad>> ObtenerTodasAsync();
        Task<EComplejidad?> ObtenerPorIdAsync(byte id);
    }
}
