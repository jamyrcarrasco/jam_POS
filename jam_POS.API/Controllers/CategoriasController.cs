using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.DTOs.Common;
using jam_POS.Application.Services;

namespace jam_POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        // GET: api/categorias
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CategoriaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CategoriaResponse>>> GetCategorias([FromQuery] CategoriaFilterRequest filter)
        {
            try
            {
                var categorias = await _categoriaService.GetAllCategoriasAsync(filter);
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando categorías");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las categorías" });
            }
        }

        // GET: api/categorias/activas
        [HttpGet("activas")]
        [ProducesResponseType(typeof(IEnumerable<CategoriaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoriaResponse>>> GetCategoriasActivas()
        {
            try
            {
                var categorias = await _categoriaService.GetCategoriasActivasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando categorías activas");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las categorías activas" });
            }
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetCategoriaByIdAsync(id);
                
                if (categoria == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Categoría con ID {id} no encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando categoría con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar la categoría" });
            }
        }

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaResponse>> PutCategoria(int id, [FromBody] UpdateCategoriaRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "El ID no coincide" });
                }

                var categoria = await _categoriaService.UpdateCategoriaAsync(id, request);
                return Ok(categoria);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando categoría con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar la categoría" });
            }
        }

        // POST: api/categorias
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaResponse>> PostCategoria([FromBody] CreateCategoriaRequest request)
        {
            try
            {
                var categoria = await _categoriaService.CreateCategoriaAsync(request);
                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando categoría");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear la categoría" });
            }
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var result = await _categoriaService.DeleteCategoriaAsync(id);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Categoría con ID {id} no encontrada" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando categoría con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar la categoría" });
            }
        }
    }
}

