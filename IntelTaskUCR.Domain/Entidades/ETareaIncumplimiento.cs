using System;

namespace Dominio.Entidades
{
    public class ETareaIncumplimiento
    {
        public int CN_Id_tarea_incumplimiento { get; set; } // PK
        public int CN_Id_tarea { get; set; }                // FK
        public string? CT_Justificacion_incumplimiento { get; set; }
        public DateTime CF_Fecha_incumplimiento { get; set; }

        // Relación
        public ETarea? Tarea { get; set; }
    }
}
