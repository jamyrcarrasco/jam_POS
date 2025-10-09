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
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        // GET: api/productos
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProductoResponse>>> GetProductos([FromQuery] ProductFilterRequest filter)
        {
            try
            {
                var productos = await _productoService.GetAllProductosAsync(filter);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando productos");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar los productos" });
            }
        }

        // GET: api/productos/categorias
        [HttpGet("categorias")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            try
            {
                var categorias = await _productoService.GetCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando categorías");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar las categorías" });
            }
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoResponse>> GetProducto(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                
                if (producto == null)
                {
                    return NotFound(new ErrorResponse { Message = $"Producto con ID {id} no encontrado" });
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recuperando producto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al recuperar el producto" });
            }
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoResponse>> PutProducto(int id, [FromBody] UpdateProductoRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new ErrorResponse { Message = "ID mismatch" });
                }

                var producto = await _productoService.UpdateProductoAsync(id, request);
                return Ok(producto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando producto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al actualizar el producto" });
            }
        }

        // POST: api/productos
        [HttpPost]
        [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoResponse>> PostProducto([FromBody] CreateProductoRequest request)
        {
            try
            {
                var producto = await _productoService.CreateProductoAsync(request);
                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando producto");
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al crear el producto" });
            }
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                var result = await _productoService.DeleteProductoAsync(id);
                
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"Producto con ID {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando producto con ID: {Id}", id);
                return StatusCode(500, new ErrorResponse { Message = "Ocurrió un error al eliminar el producto" });
            }
        }
    }
}