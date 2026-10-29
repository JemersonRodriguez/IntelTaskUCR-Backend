// Dominio/Interfaces/ITareaSeguimientoRepository.cs
using Dominio.Entidades;
using System.Collections.Generic; // Agrega esta línea
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface ITareaSeguimientoRepository
    {
        Task<ETareaSeguimiento> ObtenerSeguimientoPorIdAsync(int id);
        Task CrearSeguimientoAsync(ETareaSeguimiento seguimiento);
        // NUEVO: Método para obtener seguimientos por ID de tarea
        Task<IEnumerable<ETareaSeguimiento>> ObtenerSeguimientosPorTareaIdAsync(int idTarea);
    }
}