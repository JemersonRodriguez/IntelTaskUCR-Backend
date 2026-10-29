// Api/DTO/TareaSeguimientoCreacionDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
    public class TareaSeguimientoCreacionDTO
    {
        [Required(ErrorMessage = "El comentario es requerido.")]
        [StringLength(4000, ErrorMessage = "El comentario no puede exceder los 4000 caracteres.")] // varchar(max)
        public string CT_Comentario { get; set; }
    }
}