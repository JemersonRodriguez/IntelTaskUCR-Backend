namespace Api.DTO
{
    public class EstadoActualizarDTO
    {
        public byte CN_Id_estado { get; set; }
        public string? MotivoEspera { get; set; }
        public string? JustificacionIncumplimiento { get; set; }
        public string? DescripcionRechazo { get; set; } // Nuevo campo para el motivo de rechazo
    }
}