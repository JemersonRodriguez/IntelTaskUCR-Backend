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
    public class ComplejidadRepository : IComplejidadRepository
    {
        private readonly IntelTaskUCRDbContext _context;

        public ComplejidadRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EComplejidad>> ObtenerTodasAsync()
        {
            return await _context.T_Complejidades.ToListAsync();
        }

        public async Task<EComplejidad?> ObtenerPorIdAsync(byte id)
        {
            return await _context.T_Complejidades.FindAsync(id);
        }
    }
}
