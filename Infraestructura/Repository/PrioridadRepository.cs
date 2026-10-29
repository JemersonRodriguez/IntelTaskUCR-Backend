using Dominio.Entidades;
using Dominio.Interfaces;
using Infraestructura.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class PrioridadRepository : IPrioridadRepository
    {
        private readonly IntelTaskUCRDbContext _context;

        public PrioridadRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EPrioridad>> ObtenerTodasAsync()
        {
            return await _context.T_Prioridades.ToListAsync();
        }

        public async Task<EPrioridad?> ObtenerPorIdAsync(byte id)
        {
            return await _context.T_Prioridades.FindAsync(id);
        }
    }
}
