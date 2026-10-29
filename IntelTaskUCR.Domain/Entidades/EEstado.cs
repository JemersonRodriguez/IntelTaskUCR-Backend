using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class EEstado
    {
        public byte CN_Id_estado { get; set; }
        public string CT_Estado { get; set; } = string.Empty;
        public string? CT_Descripcion { get; set; }

        // Relación uno a muchos con ETarea
        public ICollection<ETarea>? Tareas { get; set; }
    }
}
