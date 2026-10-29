namespace Api.DTO
{
    public class UsuarioCrearDTO
    {
        public int CN_Id_Usuario { get; set; }
        public string CT_Nombre_usuario { get; set; }
        public string CT_Correo_usuario { get; set; }
        public DateTime CF_Fecha_nacimiento { get; set; }
        public string CT_Contrasenna { get; set; }
        public Boolean CB_Estado_usuario { get; set; }
        public int CN_Id_rol { get; set; }
    }
}
