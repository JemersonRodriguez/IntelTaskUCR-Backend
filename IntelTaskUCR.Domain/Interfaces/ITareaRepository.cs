using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface ITareaRepository
    {
        Task<IEnumerable<ETarea>> ObtenerTareasAsync();
        Task<IEnumerable<ETarea>> ObtenerSubTareasPorUsuarioAsync(int id);
        Task<IEnumerable<ETarea>> ObtenerTareasActivasPorUsuarioAsync(int idUsuario);
        Task<IEnumerable<ETarea>> ObtenerTareasFinalizadasPorUsuarioAsync(int idUsuario);
        Task<ETarea?> ObtenerTareaPorIdAsync(int id);
        Task CrearTareaAsync(ETarea tarea);
        Task ActualizarTareaAsync(ETarea tarea);
        Task EliminarTareaAsync(int id);
        Task<IEnumerable<ETarea>> ObtenerTareasPorUsuarioAsync(int idUsuario);
    }
}
