using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("T_Tareas_Justificacion_Rechazo")]
    public class ETareaJustificacionRechazo
    {
        [Key]
        [Column("CN_Id_tarea_rechazo")]
        public int CN_Id_tarea_rechazo { get; set; } // Clave Primaria

        [Column("CN_Id_tarea")]
        [ForeignKey("Tarea")]
        public int CN_Id_tarea { get; set; } // Clave Foránea a ETarea

        [Column("CF_Fecha_hora_rechazo")]
        public DateTime CF_Fecha_hora_rechazo { get; set; }

        [Required]
        [StringLength(400)] // Según el varchar(400) de tu esquema
        [Column("CT_Descripcion_rechazo")]
        public string CT_Descripcion_rechazo { get; set; } = string.Empty;

        // Propiedad de navegación
        public ETarea Tarea { get; set; } = null!; // Se asume que siempre estará asociada a una tarea
    }
}