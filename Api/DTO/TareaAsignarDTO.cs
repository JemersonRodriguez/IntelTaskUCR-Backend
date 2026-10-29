namespace Api.DTO
{
    public class TareaAsignarDTO
    {
        public int IdUsuarioAsignador { get; set; }
        public List<int> IdsUsuariosAsignados { get; set; } = new();
        public DateTime FechaAsignacion { get; set; }
        public DateTime FechaLimite { get; set; }
    }
}
