// Dominio/Entidades/ETareaSeguimiento.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("T_Tareas_Seguimiento")]
    public class ETareaSeguimiento
    {
        [Key]
        [Column("CN_Id_seguimiento")]
        public int CN_Id_seguimiento { get; set; }

        [Required]
        [Column("CN_Id_tarea")]
        public int CN_Id_tarea { get; set; }

        [Column("CT_Comentario")]
        public string CT_Comentario { get; set; }

        [Required]
        [Column("CF_Fecha_seguimiento")]
        public DateTime CF_Fecha_seguimiento { get; set; }

        // Propiedad de navegación para la relación con Tarea
        [ForeignKey("CN_Id_tarea")]
        public ETarea Tarea { get; set; }
    }
}