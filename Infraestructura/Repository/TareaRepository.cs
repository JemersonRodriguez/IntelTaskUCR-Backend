using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repository
{
    public class TareaRepository : ITareaRepository
    {
        private readonly IntelTaskUCRDbContext _context;

        public TareaRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ETarea>> ObtenerSubTareasPorUsuarioAsync(int idUsuario)
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .Where(t => t.CN_Tarea_origen != null &&
                           (t.CN_Usuario_asignado == idUsuario || t.CN_Usuario_creador == idUsuario))
                .ToListAsync();
        }

        public async Task<IEnumerable<ETarea>> ObtenerTareasPorUsuarioAsync(int idUsuario)
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .Where(t => t.CN_Usuario_asignado == idUsuario || t.CN_Usuario_creador == idUsuario)
                .ToListAsync();
        }
        public async Task<IEnumerable<ETarea>> ObtenerTareasActivasPorUsuarioAsync(int idUsuario)
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .Where(t =>
                    (t.CN_Usuario_asignado == idUsuario || t.CN_Usuario_creador == idUsuario)
                    && t.CN_Id_estado != 7 // Solo tareas que NO están finalizadas
                    && t.CN_Tarea_origen == null // Solo tareas principales, NO subtareas
                                                 // NO mostrar tareas que sean origen de al menos una subtarea:
                    && !_context.T_Tareas.Any(sub => sub.CN_Tarea_origen == t.CN_Id_tarea)
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<ETarea>> ObtenerTareasFinalizadasPorUsuarioAsync(int idUsuario)
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .Where(t =>
                    (t.CN_Usuario_asignado == idUsuario || t.CN_Usuario_creador == idUsuario)
                    && t.CN_Id_estado == 7 // Solo tareas finalizadas
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<ETarea>> ObtenerTareasAsync()
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .ToListAsync();
        }

        public async Task<ETarea?> ObtenerTareaPorIdAsync(int id)
        {
            return await _context.T_Tareas
                .Include(t => t.Estado)
                .Include(t => t.Complejidad)
                .Include(t => t.Prioridad)
                .Include(t => t.UsuarioCreador)
                .Include(t => t.UsuarioAsignado)
                .FirstOrDefaultAsync(t => t.CN_Id_tarea == id);
        }


        public async Task CrearTareaAsync(ETarea tarea)
        {
            _context.T_Tareas.Add(tarea);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarTareaAsync(ETarea tarea)
        {
            _context.T_Tareas.Update(tarea);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarTareaAsync(int id)
        {
            var tarea = await _context.T_Tareas.FindAsync(id);
            if (tarea != null)
            {
                _context.T_Tareas.Remove(tarea);
                await _context.SaveChangesAsync();
            }
        }
    }
}
