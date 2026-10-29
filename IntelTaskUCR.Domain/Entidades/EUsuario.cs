namespace Dominio.Entidades
{
    public class EUsuario
    {
        public int CN_Id_usuario { get; set; }
        public string CT_Nombre_usuario { get; set; } = string.Empty;
        public string CT_Correo_usuario { get; set; } = string.Empty;
        public DateTime CF_Fecha_nacimiento { get; set; }
        public string CT_Contrasenna { get; set; } = string.Empty;
        public bool CB_Estado_usuario { get; set; }
        public DateTime CF_Fecha_creacion_usuario { get; set; }
        public DateTime CF_Fecha_modificacion_usuario { get; set; }
        // Clave foránea
        public int CN_Id_rol { get; set; }

        // Propiedad de navegación
        public ERol? Rol { get; set; }
        // Relación uno a muchos (inversa) con ETarea como creador
        public ICollection<ETarea>? TareasCreadas { get; set; }
        // Relación uno a muchos (inversa) con ETarea como asignado
        public ICollection<ETarea>? TareasAsignadas { get; set; }
    }
}
