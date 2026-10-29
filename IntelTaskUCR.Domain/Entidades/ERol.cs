using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class ERol
    {
        public int CN_Id_rol { get; set; }
        public string CT_Nombre_rol { get; set; } = string.Empty;
        public int CN_Jerarquia { get; set; }

        // Relación inversa opcional: lista de usuarios que tienen este rol
        public ICollection<EUsuario> Usuarios { get; set; } = new List<EUsuario>();
    }
}
