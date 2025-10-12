using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly ILogger<VentasController> _logger;

        public VentasController(IVentaService ventaService, ILogger<VentasController> logger)
        {
            _ventaService = ventaService;
            _logger = logger;
        }

        // GET: api/ventas
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VentaSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VentaSummaryResponse>>> GetVentas(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var ventas = await _ventaService.GetVentasAsync(page, pageSize);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando ventas");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las ventas" });
            }
        }

        // GET: api/ventas/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VentaResponse>> GetVenta(int id)
        {
            try
            {
                var venta = await _ventaService.GetVentaByIdAsync(id);
                
                if (venta == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Venta con ID {id} no encontrada" });
                }

                return Ok(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando venta con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar la venta" });
            }
        }

        // POST: api/ventas
        [HttpPost]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VentaResponse>> PostVenta([FromBody] CreateVentaRequest request)
        {
            try
            {
                var venta = await _ventaService.CreateVentaAsync(request);
                return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, venta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando venta");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear la venta" });
            }
        }

        // PUT: api/ventas/5/cancelar
        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarVenta(int id, [FromBody] CancelarVentaRequest request)
        {
            try
            {
                var result = await _ventaService.CancelarVentaAsync(id, request.Motivo);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Venta con ID {id} no encontrada" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelando venta con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al cancelar la venta" });
            }
        }

        // GET: api/ventas/usuario/5
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<VentaSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VentaSummaryResponse>>> GetVentasByUsuario(
            int usuarioId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByUsuarioAsync(usuarioId, page, pageSize);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando ventas del usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las ventas del usuario" });
            }
        }

        // GET: api/ventas/fecha
        [HttpGet("fecha")]
        [ProducesResponseType(typeof(IEnumerable<VentaSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VentaSummaryResponse>>> GetVentasByFecha(
            [FromQuery] DateTime fechaInicio, 
            [FromQuery] DateTime fechaFin, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var ventas = await _ventaService.GetVentasByFechaAsync(fechaInicio, fechaFin, page, pageSize);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando ventas por fecha");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las ventas por fecha" });
            }
        }

        // GET: api/ventas/resumen/hoy
        [HttpGet("resumen/hoy")]
        [ProducesResponseType(typeof(VentasHoyResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<VentasHoyResponse>> GetVentasHoy()
        {
            try
            {
                var total = await _ventaService.GetTotalVentasHoyAsync();
                var cantidad = await _ventaService.GetCantidadVentasHoyAsync();

                var resumen = new VentasHoyResponse
                {
                    TotalVentas = total,
                    CantidadVentas = cantidad,
                    Fecha = DateTime.Today
                };

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando resumen de ventas de hoy");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el resumen de ventas" });
            }
        }
    }

    public class CancelarVentaRequest
    {
        [Required(ErrorMessage = "El motivo de cancelación es requerido")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }

    public class VentasHoyResponse
    {
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public DateTime Fecha { get; set; }
    }
}
