// Ruta: Infraestructura/Repository/TareaIncumplimientoRepository.cs
using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq; // Necesario para .MaxAsync()

namespace Infraestructura.Repository
{
    public class TareaIncumplimientoRepository : ITareaIncumplimientoRepository
    {
        private readonly IntelTaskUCRDbContext _context;

        public TareaIncumplimientoRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task CrearIncumplimientoAsync(ETareaIncumplimiento incumplimiento)
        {
            // --- INICIO DE LA LÓGICA DE GENERACIÓN DE ID ---
            // Si la columna CN_Id_tarea_incumplimiento NO es IDENTITY en la DB,
            // debemos generar el ID manualmente.
            // Esto es propenso a condiciones de carrera en alta concurrencia.
            int nextId = 1; // Valor inicial si la tabla está vacía
            if (await _context.T_Tareas_Incumplimientos.AnyAsync())
            {
                // Obtener el ID máximo actual y sumarle 1
                nextId = await _context.T_Tareas_Incumplimientos
                                        .MaxAsync(i => i.CN_Id_tarea_incumplimiento) + 1;
            }
            incumplimiento.CN_Id_tarea_incumplimiento = nextId; // Asignar el ID generado
            // --- FIN DE LA LÓGICA DE GENERACIÓN DE ID ---

            _context.T_Tareas_Incumplimientos.Add(incumplimiento);
            await _context.SaveChangesAsync();
        }

        public async Task<ETareaIncumplimiento?> ObtenerPorTareaIdAsync(int idTarea)
        {
            return await _context.T_Tareas_Incumplimientos
                .FirstOrDefaultAsync(i => i.CN_Id_tarea == idTarea);
        }

        public async Task EliminarIncumplimientoAsync(ETareaIncumplimiento incumplimiento)
        {
            _context.T_Tareas_Incumplimientos.Remove(incumplimiento);
            await _context.SaveChangesAsync();
        }
    }
}