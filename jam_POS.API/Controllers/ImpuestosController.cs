using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpuestosController : ControllerBase
    {
        private readonly IImpuestoService _impuestoService;
        private readonly ILogger<ImpuestosController> _logger;

        public ImpuestosController(IImpuestoService impuestoService, ILogger<ImpuestosController> logger)
        {
            _impuestoService = impuestoService;
            _logger = logger;
        }

        // GET: api/impuestos
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImpuestoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ImpuestoResponse>>> GetImpuestos()
        {
            try
            {
                var impuestos = await _impuestoService.GetAllImpuestosAsync();
                return Ok(impuestos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando impuestos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los impuestos" });
            }
        }

        // GET: api/impuestos/activos
        [HttpGet("activos")]
        [ProducesResponseType(typeof(IEnumerable<ImpuestoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ImpuestoResponse>>> GetImpuestosActivos()
        {
            try
            {
                var impuestos = await _impuestoService.GetImpuestosActivosAsync();
                return Ok(impuestos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando impuestos activos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los impuestos activos" });
            }
        }

        // GET: api/impuestos/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ImpuestoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpuestoResponse>> GetImpuesto(int id)
        {
            try
            {
                var impuesto = await _impuestoService.GetImpuestoByIdAsync(id);
                
                if (impuesto == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Impuesto con ID {id} no encontrado" });
                }

                return Ok(impuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando impuesto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el impuesto" });
            }
        }

        // PUT: api/impuestos/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ImpuestoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ImpuestoResponse>> PutImpuesto(int id, [FromBody] UpdateImpuestoRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "El ID no coincide" });
                }

                var impuesto = await _impuestoService.UpdateImpuestoAsync(id, request);
                return Ok(impuesto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando impuesto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar el impuesto" });
            }
        }

        // POST: api/impuestos
        [HttpPost]
        [ProducesResponseType(typeof(ImpuestoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImpuestoResponse>> PostImpuesto([FromBody] CreateImpuestoRequest request)
        {
            try
            {
                var impuesto = await _impuestoService.CreateImpuestoAsync(request);
                return CreatedAtAction(nameof(GetImpuesto), new { id = impuesto.Id }, impuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando impuesto");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear el impuesto" });
            }
        }

        // DELETE: api/impuestos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImpuesto(int id)
        {
            try
            {
                var result = await _impuestoService.DeleteImpuestoAsync(id);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Impuesto con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando impuesto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar el impuesto" });
            }
        }
    }
}

