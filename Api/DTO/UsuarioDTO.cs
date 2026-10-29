namespace Api.DTO
{
    public class UsuarioDTO
    {
        public int CN_Id_usuario { get; set; }
        public string CT_Nombre_usuario { get; set; }
        public string CT_Correo_usuario { get; set; }
        public RolDTO Rol { get; set; } // objeto completo
    }
}
