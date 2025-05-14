using Microsoft.AspNetCore.Mvc;
using Csharpapigenerica.Services;
using Csharpapigenerica.Models;

namespace Csharpapigenerica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndicadorController : ControllerBase
    {
        private readonly IndicadorService _service;

        public IndicadorController(IndicadorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndicadorDto>>> Get()
        {
            var indicadores = await _service.ObtenerTodosAsync();
            return Ok(indicadores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IndicadorDto>> GetById(int id)
        {
            var indicador = await _service.ObtenerPorIdAsync(id);
            return indicador == null ? NotFound() : Ok(indicador);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] IndicadorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.CrearAsync(dto);
            return Ok(new { mensaje = "Indicador creado correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] IndicadorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.Id = id;
            await _service.ActualizarAsync(dto);
            return Ok(new { mensaje = "Indicador actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.EliminarAsync(id);
            return Ok(new { mensaje = "Indicador eliminado correctamente" });
        }
    }
}
