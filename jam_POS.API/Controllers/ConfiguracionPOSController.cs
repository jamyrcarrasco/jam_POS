using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionPOSController : ControllerBase
    {
        private readonly IConfiguracionPOSService _configuracionPOSService;
        private readonly ILogger<ConfiguracionPOSController> _logger;

        public ConfiguracionPOSController(IConfiguracionPOSService configuracionPOSService, ILogger<ConfiguracionPOSController> logger)
        {
            _configuracionPOSService = configuracionPOSService;
            _logger = logger;
        }

        // GET: api/configuracionpos
        [HttpGet]
        [ProducesResponseType(typeof(ConfiguracionPOSResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfiguracionPOSResponse>> GetConfiguracion()
        {
            try
            {
                var configuracion = await _configuracionPOSService.GetConfiguracionAsync();
                
                if (configuracion == null)
                {
                    return NotFound(new ErrorResponse { Message = "No se encontró configuración POS" });
                }

                return Ok(configuracion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando configuración POS");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar la configuración POS" });
            }
        }

        // PUT: api/configuracionpos/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ConfiguracionPOSResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ConfiguracionPOSResponse>> PutConfiguracion(int id, [FromBody] UpdateConfiguracionPOSRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "El ID no coincide" });
                }

                var configuracion = await _configuracionPOSService.UpdateConfiguracionAsync(id, request);
                return Ok(configuracion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando configuración POS con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar la configuración POS" });
            }
        }

        // POST: api/configuracionpos
        [HttpPost]
        [ProducesResponseType(typeof(ConfiguracionPOSResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ConfiguracionPOSResponse>> PostConfiguracion([FromBody] CreateConfiguracionPOSRequest request)
        {
            try
            {
                var configuracion = await _configuracionPOSService.CreateConfiguracionAsync(request);
                return CreatedAtAction(nameof(GetConfiguracion), new { id = configuracion.Id }, configuracion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando configuración POS");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear la configuración POS" });
            }
        }

        // DELETE: api/configuracionpos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            try
            {
                var result = await _configuracionPOSService.DeleteConfiguracionAsync(id);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Configuración POS con ID {id} no encontrada" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando configuración POS con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar la configuración POS" });
            }
        }

        // GET: api/configuracionpos/siguiente-recibo
        [HttpGet("siguiente-recibo")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetSiguienteNumeroRecibo()
        {
            try
            {
                var numero = await _configuracionPOSService.GetSiguienteNumeroReciboAsync();
                return Ok(numero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando siguiente número de recibo");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al generar el siguiente número de recibo" });
            }
        }

        // GET: api/configuracionpos/siguiente-factura
        [HttpGet("siguiente-factura")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetSiguienteNumeroFactura()
        {
            try
            {
                var numero = await _configuracionPOSService.GetSiguienteNumeroFacturaAsync();
                return Ok(numero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando siguiente número de factura");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al generar el siguiente número de factura" });
            }
        }
    }
}

