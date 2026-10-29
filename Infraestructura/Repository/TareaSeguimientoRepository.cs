// Infraestructura/Repositorios/TareaSeguimientoRepository.cs
using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context; // Asegúrate de que esta sea la ruta correcta a tu DbContext
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Agrega esta línea
using System.Linq; // Agrega esta línea para OrderByDescending
using System.Threading.Tasks;

namespace Infraestructura.Repositorios
{
    public class TareaSeguimientoRepository : ITareaSeguimientoRepository
    {
        private readonly IntelTaskUCRDbContext _context; // Tu DbContext

        public TareaSeguimientoRepository(IntelTaskUCRDbContext context) // Asegúrate de usar el nombre correcto de tu DbContext
        {
            _context = context;
        }

        public async Task<ETareaSeguimiento> ObtenerSeguimientoPorIdAsync(int id)
        {
            return await _context.T_Tareas_Seguimiento.FirstOrDefaultAsync(s => s.CN_Id_seguimiento == id);
        }

        public async Task CrearSeguimientoAsync(ETareaSeguimiento seguimiento)
        {
            _context.T_Tareas_Seguimiento.Add(seguimiento);
            await _context.SaveChangesAsync();
        }

        // NUEVO: Implementación del método para obtener seguimientos por ID de tarea
        public async Task<IEnumerable<ETareaSeguimiento>> ObtenerSeguimientosPorTareaIdAsync(int idTarea)
        {
            return await _context.T_Tareas_Seguimiento
                                 .Where(s => s.CN_Id_tarea == idTarea)
                                 .OrderByDescending(s => s.CF_Fecha_seguimiento) // Ordenar por fecha, los más recientes primero
                                 .ToListAsync();
        }
    }
}