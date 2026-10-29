using Api.DTO;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplejidadesController : ControllerBase
    {
        private readonly IComplejidadRepository _repo;

        public ComplejidadesController(IComplejidadRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var complejidades = await _repo.ObtenerTodasAsync();

            // Mapea a DTO para no enviar la relación Tareas
            var dtoList = complejidades.Select(p => new ComplejidadDTO
            {
                CN_Id_complejidad = p.CN_Id_complejidad,
                CT_Nombre = p.CT_Nombre
            });

            return Ok(dtoList);
        }
    }
}
