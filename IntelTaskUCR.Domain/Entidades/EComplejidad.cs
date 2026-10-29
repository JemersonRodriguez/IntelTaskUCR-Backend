using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class EComplejidad
    {
        public byte CN_Id_complejidad { get; set; }
        public string CT_Nombre { get; set; } = string.Empty;

        // Relación uno a muchos con ETarea
        public ICollection<ETarea>? Tareas { get; set; }
    }
}
