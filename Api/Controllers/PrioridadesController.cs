using Api.DTO;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrioridadesController : ControllerBase
    {
        private readonly IPrioridadRepository _repo;

        public PrioridadesController(IPrioridadRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var prioridades = await _repo.ObtenerTodasAsync();

            // Mapea a DTO para no enviar la relación Tareas
            var dtoList = prioridades.Select(p => new PrioridadDTO
            {
                CN_Id_prioridad = p.CN_Id_prioridad,
                CT_Descripcion_prioridad = p.CT_Descripcion_prioridad
            });

            return Ok(dtoList);
        }
    }
}
