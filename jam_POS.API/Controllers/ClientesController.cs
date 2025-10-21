using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using jam_POS.Application.DTOs.Common;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ClienteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ClienteResponse>>> GetClientes([FromQuery] ClienteFilterRequest filter)
        {
            try
            {
                var clientes = await _clienteService.GetClientesAsync(filter);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando clientes");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los clientes" });
            }
        }

        [HttpGet("activos")]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClienteResponse>>> GetClientesActivos()
        {
            try
            {
                var clientes = await _clienteService.GetClientesActivosAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando clientes activos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los clientes activos" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteResponse>> GetCliente(int id)
        {
            try
            {
                var cliente = await _clienteService.GetClienteByIdAsync(id);

                if (cliente == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Cliente con ID {id} no encontrado" });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando cliente con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el cliente" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClienteResponse>> PostCliente([FromBody] CreateClienteRequest request)
        {
            try
            {
                var cliente = await _clienteService.CreateClienteAsync(request);
                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando cliente");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear el cliente" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteResponse>> PutCliente(int id, [FromBody] UpdateClienteRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "ID mismatch" });
                }

                var cliente = await _clienteService.UpdateClienteAsync(id, request);
                return Ok(cliente);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando cliente con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar el cliente" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var eliminado = await _clienteService.DeleteClienteAsync(id);

                if (!eliminado)
                {
                    return NotFound(new ErrorResponse { Message = $"Cliente con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando cliente con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar el cliente" });
            }
        }
    }
}
