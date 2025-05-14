using Microsoft.AspNetCore.Mvc;
using Csharpapigenerica.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Csharpapigenerica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariablesPorIndicadorController : ControllerBase
    {
        private readonly IndicadorService _service;

        public VariablesPorIndicadorController(IndicadorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<VariableIndicadorDto>>> Get()
        {
            var resultado = await _service.ObtenerVariablesPorIndicadorAsync();
            return Ok(resultado);
        }
    }

    public class VariableIndicadorDto
    {
        public int Id { get; set; }
        public string Variable { get; set; } = "";
        public string Indicador { get; set; } = "";
        public double Dato { get; set; }
        public DateTime FechaDato { get; set; }
        public string Usuario { get; set; } = "";
    }
}
