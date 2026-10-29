using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class EPrioridad
    {
        public byte CN_Id_prioridad { get; set; }
        public string CT_Descripcion_prioridad { get; set; } = string.Empty;

        // Relación uno a muchos con ETarea
        public ICollection<ETarea>? Tareas { get; set; }
    }
}
