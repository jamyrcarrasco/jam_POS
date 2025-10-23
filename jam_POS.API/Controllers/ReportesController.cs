using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteService reporteService, ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        [HttpGet("ventas")]
        [ProducesResponseType(typeof(SalesReportResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SalesReportResponse>> GetReporteVentas([FromQuery] SalesReportFilterRequest filter)
        {
            try
            {
                var reporte = await _reporteService.GetSalesReportAsync(filter);
                return Ok(reporte);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando el reporte de ventas");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al generar el reporte de ventas" });
            }
        }
    }
}
