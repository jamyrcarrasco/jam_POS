using Microsoft.AspNetCore.Mvc;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        // GET: api/permissions
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PermissionResponse>>> GetPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando permisos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los permisos" });
            }
        }

        // GET: api/permissions/by-module
        [HttpGet("by-module")]
        [ProducesResponseType(typeof(Dictionary<string, List<PermissionResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, List<PermissionResponse>>>> GetPermissionsByModule()
        {
            try
            {
                var permissions = await _permissionService.GetPermissionsByModuleAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando permisos por módulo");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los permisos" });
            }
        }

        // GET: api/permissions/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionResponse>> GetPermission(int id)
        {
            try
            {
                var permission = await _permissionService.GetPermissionByIdAsync(id);
                
                if (permission == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Permiso con ID {id} no encontrado" });
                }

                return Ok(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando permiso con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el permiso" });
            }
        }
    }
}

