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
    public class EstadoRepository : IEstadoRepository
    {
        private readonly IntelTaskUCRDbContext _context;

        public EstadoRepository(IntelTaskUCRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EEstado>> ObtenerTodosAsync()
        {
            return await _context.T_Estados.ToListAsync();
        }

        public async Task<EEstado?> ObtenerPorIdAsync(byte id)
        {
            return await _context.T_Estados.FindAsync(id);
        }
    }
}
