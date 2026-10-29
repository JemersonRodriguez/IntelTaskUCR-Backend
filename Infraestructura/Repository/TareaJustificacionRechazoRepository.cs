// Infraestructura.Repository/TareaJustificacionRechazoRepository.cs
using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context; // Usas 'Infraestructura.Context' para tu DbContext
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repository
{
    public class TareaJustificacionRechazoRepository : ITareaJustificacionRechazoRepository
    {
        private readonly IntelTaskUCRDbContext _context; // Tu DbContext se llama IntelTaskUCRDbContext

        public TareaJustificacionRechazoRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task CrearRechazoAsync(ETareaJustificacionRechazo rechazo)
        {
            // ***** INICIO DE CAMBIO *****
            // Genera el próximo ID único manualmente.
            // Esta es una solución alternativa cuando la columna PK no es IDENTITY en la DB.
            // Importante: En entornos de alta concurrencia, esta lógica podría llevar a
            // colisiones de ID si múltiples usuarios intentan insertar exactamente al mismo tiempo.
            // Si la base de datos permitiera cambios, la solución ideal sería hacer
            // 'CN_Id_tarea_rechazo' una columna IDENTITY directamente en la DB.

            int nextId = 1; // Valor inicial por si la tabla está vacía o el primer ID es 1

            // Verifica si ya hay registros en la tabla
            if (await _context.T_Tareas_Justificacion_Rechazo.AnyAsync())
            {
                // Obtiene el ID máximo existente y le suma 1
                nextId = await _context.T_Tareas_Justificacion_Rechazo
                                       .MaxAsync(r => r.CN_Id_tarea_rechazo) + 1;
            }

            // Asigna el ID generado a la entidad antes de añadirla al contexto
            rechazo.CN_Id_tarea_rechazo = nextId;
            // ***** FIN DE CAMBIO *****

            await _context.T_Tareas_Justificacion_Rechazo.AddAsync(rechazo);
            await _context.SaveChangesAsync();
        }

        public async Task<ETareaJustificacionRechazo?> ObtenerPorTareaIdAsync(int idTarea)
        {
            return await _context.T_Tareas_Justificacion_Rechazo
                                 .FirstOrDefaultAsync(r => r.CN_Id_tarea == idTarea);
        }

        public async Task EliminarRechazoAsync(ETareaJustificacionRechazo rechazo)
        {
            _context.T_Tareas_Justificacion_Rechazo.Remove(rechazo);
            await _context.SaveChangesAsync();
        }
    }
}