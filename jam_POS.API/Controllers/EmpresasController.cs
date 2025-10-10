using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;
        private readonly ILogger<EmpresasController> _logger;

        public EmpresasController(IEmpresaService empresaService, ILogger<EmpresasController> logger)
        {
            _empresaService = empresaService;
            _logger = logger;
        }

        // POST: api/empresas/register
        [HttpPost("register")]
        [ProducesResponseType(typeof(EmpresaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmpresaResponse>> RegisterEmpresa([FromBody] RegisterEmpresaRequest request)
        {
            try
            {
                var empresa = await _empresaService.RegisterEmpresaAsync(request);
                return CreatedAtAction(nameof(GetEmpresa), new { id = empresa.Id }, empresa);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando empresa");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al registrar la empresa" });
            }
        }

        // GET: api/empresas/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmpresaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmpresaResponse>> GetEmpresa(int id)
        {
            try
            {
                var empresa = await _empresaService.GetEmpresaByIdAsync(id);
                
                if (empresa == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Empresa con ID {id} no encontrada" });
                }

                return Ok(empresa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando empresa con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar la empresa" });
            }
        }
    }
}

