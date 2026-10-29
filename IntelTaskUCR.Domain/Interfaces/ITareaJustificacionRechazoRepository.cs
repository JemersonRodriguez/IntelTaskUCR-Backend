using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface ITareaJustificacionRechazoRepository
    {
        Task CrearRechazoAsync(ETareaJustificacionRechazo rechazo);
        Task<ETareaJustificacionRechazo?> ObtenerPorTareaIdAsync(int idTarea);
        Task EliminarRechazoAsync(ETareaJustificacionRechazo rechazo);
    }
}