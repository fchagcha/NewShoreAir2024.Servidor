using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewShoreAir.Business.Application.Models;

namespace NewShoreAir.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CalcularRutaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CalcularRutaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(Name = "CalcularRuta")]
        [ProducesResponseType(typeof(CalcularRutaResponse), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CalcularRuta([FromBody] CalcularRutaRequest request)
        {
            var respuesta = await _mediator.Send(request);

            return Ok(respuesta);
        }
    }
}
