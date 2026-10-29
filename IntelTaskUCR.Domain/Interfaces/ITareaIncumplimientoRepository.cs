using Dominio.Entidades;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface ITareaIncumplimientoRepository
    {
        Task CrearIncumplimientoAsync(ETareaIncumplimiento incumplimiento);
        Task<ETareaIncumplimiento?> ObtenerPorTareaIdAsync(int idTarea);
        Task EliminarIncumplimientoAsync(ETareaIncumplimiento incumplimiento);
    }
}