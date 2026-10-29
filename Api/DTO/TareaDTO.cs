namespace Api.DTO
{
    public class TareaDTO
    {
        public int CN_Id_tarea { get; set; }
        public int? CN_Tarea_origen { get; set; }                     // nullable en entidad
        public string CT_Titulo_tarea { get; set; }                   // requerido
        public string CT_Descripcion_tarea { get; set; }             // requerido
        public string? CT_Descripcion_espera { get; set; }           // nullable en entidad
        public string? CN_Numero_GIS { get; set; }                   // nullable en entidad
        public DateTime CF_Fecha_asignacion { get; set; }           // requerido
        public DateTime CF_Fecha_limite { get; set; }               // requerido
        public DateTime? CF_Fecha_finalizacion { get; set; }        // nullable en entidad

        public EstadoDTO? Estado { get; set; }                       // relación nullable en entidad
        public ComplejidadDTO? Complejidad { get; set; }            // relación nullable en entidad
        public PrioridadDTO? Prioridad { get; set; }                // relación nullable en entidad
        public UsuarioDTO? UsuarioCreador { get; set; }             // relación nullable en entidad
        public UsuarioDTO? UsuarioAsignado { get; set; }            // nullable en entidad
    }
}
