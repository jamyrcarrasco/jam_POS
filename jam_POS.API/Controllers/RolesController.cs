using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // GET: api/roles
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetRoles([FromQuery] bool includePermissions = false)
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync(includePermissions);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando roles");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los roles" });
            }
        }

        // GET: api/roles/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleResponse>>> GetActiveRoles()
        {
            try
            {
                var roles = await _roleService.GetActiveRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando roles activos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los roles activos" });
            }
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponse>> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                
                if (role == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Rol con ID {id} no encontrado" });
                }

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando rol con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el rol" });
            }
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponse>> PutRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "El ID no coincide" });
                }

                var role = await _roleService.UpdateRoleAsync(id, request);
                return Ok(role);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando rol con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar el rol" });
            }
        }

        // POST: api/roles
        [HttpPost]
        [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> PostRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(request);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando rol");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear el rol" });
            }
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Rol con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando rol con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar el rol" });
            }
        }
    }
}

