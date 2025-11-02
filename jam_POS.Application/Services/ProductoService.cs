using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using jam_POS.Application.DTOs.Common;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Core.Entities;
using jam_POS.Infrastructure.Data;

namespace jam_POS.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(ApplicationDbContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<ProductoResponse>> GetAllProductosAsync(ProductFilterRequest filter)
        {
            _logger.LogInformation("Retrieving productos with filters: {@Filter}", filter);

            var query = _context.Productos.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(searchTerm) || 
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(searchTerm)) ||
                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Categoria))
            {
                query = query.Where(p => p.CategoriaId.HasValue && p.Categoria!.Nombre == filter.Categoria);
            }

            if (filter.PrecioMin.HasValue)
            {
                query = query.Where(p => p.Precio >= filter.PrecioMin.Value);
            }

            if (filter.PrecioMax.HasValue)
            {
                query = query.Where(p => p.Precio <= filter.PrecioMax.Value);
            }

            if (filter.StockBajo.HasValue && filter.StockBajo.Value)
            {
                query = query.Where(p => p.StockMinimo.HasValue && p.Stock <= p.StockMinimo.Value);
            }

            if (filter.Activo.HasValue)
            {
                query = query.Where(p => p.Activo == filter.Activo.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply ordering
            query = ApplyOrdering(query, filter.OrderBy, filter.OrderDescending);

            // Apply pagination
            var items = await query
                .Include(p => p.Categoria)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var mappedItems = items.Select(MapToResponse).ToList();

            return new PagedResult<ProductoResponse>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductoResponse?> GetProductoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving producto with ID: {Id}", id);

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found with ID: {Id}", id);
                return null;
            }

            return MapToResponse(producto);
        }

        public async Task<ProductoResponse> CreateProductoAsync(CreateProductoRequest request)
        {
            _logger.LogInformation("Creating new producto: {Nombre}", request.Nombre);

            var producto = new Producto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                CodigoBarras = request.CodigoBarras,
                ImagenUrl = request.ImagenUrl,
                PrecioCompra = request.PrecioCompra,
                MargenGanancia = request.MargenGanancia,
                StockMinimo = request.StockMinimo,
                Activo = request.Activo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto created successfully with ID: {Id}", producto.Id);

            return MapToResponse(producto);
        }

        public async Task<ProductoResponse> UpdateProductoAsync(int id, UpdateProductoRequest request)
        {
            _logger.LogInformation("Updating producto with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found for update with ID: {Id}", id);
                throw new KeyNotFoundException($"Producto with ID {id} not found");
            }

            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.CategoriaId = request.CategoriaId;
            producto.CodigoBarras = request.CodigoBarras;
            producto.ImagenUrl = request.ImagenUrl;
            producto.PrecioCompra = request.PrecioCompra;
            producto.MargenGanancia = request.MargenGanancia;
            producto.StockMinimo = request.StockMinimo;
            producto.Activo = request.Activo;
            producto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto updated successfully with ID: {Id}", id);

            return MapToResponse(producto);
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            _logger.LogInformation("Deleting producto with ID: {Id}", id);

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                _logger.LogWarning("Producto not found for deletion with ID: {Id}", id);
                return false;
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto deleted successfully with ID: {Id}", id);

            return true;
        }

        public async Task<bool> ProductoExistsAsync(int id)
        {
            return await _context.Productos.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<string>> GetCategoriasAsync()
        {
            var categorias = await _context.Set<Categoria>()
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .Select(c => c.Nombre)
                .ToListAsync();

            return categorias;
        }

        private static IQueryable<Producto> ApplyOrdering(IQueryable<Producto> query, string? orderBy, bool descending)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return descending ? query.OrderByDescending(p => p.Nombre) : query.OrderBy(p => p.Nombre);
            }

            Expression<Func<Producto, object>> keySelector = orderBy.ToLower() switch
            {
                "nombre" => p => p.Nombre,
                "precio" => p => p.Precio,
                "stock" => p => p.Stock,
                "categoria" => p => p.Categoria != null ? p.Categoria.Nombre : string.Empty,
                "createdat" => p => p.CreatedAt,
                _ => p => p.Nombre
            };

            return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }

        private static ProductoResponse MapToResponse(Producto producto)
        {
            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = producto.Categoria?.Nombre,
                CodigoBarras = producto.CodigoBarras,
                ImagenUrl = producto.ImagenUrl,
                PrecioCompra = producto.PrecioCompra,
                MargenGanancia = producto.MargenGanancia,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                CreatedAt = producto.CreatedAt,
                UpdatedAt = producto.UpdatedAt
            };
        }

        public async Task<byte[]> ExportProductosTemplateAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating products import template");

            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nombre)
                .ToListAsync(cancellationToken);

            var categorias = await _context.Set<Categoria>()
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .Select(c => c.Nombre)
                .ToListAsync(cancellationToken);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Productos");

            var headers = new[]
            {
                "Nombre",
                "Descripcion",
                "Precio",
                "Stock",
                "Categoria",
                "CodigoBarras",
                "PrecioCompra",
                "MargenGanancia",
                "StockMinimo",
                "Activo"
            };

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#217346");
                cell.Style.Font.FontColor = XLColor.White;
            }

            worksheet.SheetView.FreezeRows(1);

            var currentRow = 2;
            foreach (var producto in productos)
            {
                worksheet.Cell(currentRow, 1).Value = producto.Nombre;
                worksheet.Cell(currentRow, 2).Value = producto.Descripcion ?? string.Empty;
                worksheet.Cell(currentRow, 3).Value = producto.Precio;
                worksheet.Cell(currentRow, 4).Value = producto.Stock;
                worksheet.Cell(currentRow, 5).Value = producto.Categoria?.Nombre ?? string.Empty;
                worksheet.Cell(currentRow, 6).Value = producto.CodigoBarras ?? string.Empty;

                if (producto.PrecioCompra.HasValue)
                {
                    worksheet.Cell(currentRow, 7).Value = producto.PrecioCompra.Value;
                }
                else
                {
                    worksheet.Cell(currentRow, 7).Clear();
                }

                if (producto.MargenGanancia.HasValue)
                {
                    worksheet.Cell(currentRow, 8).Value = producto.MargenGanancia.Value;
                }
                else
                {
                    worksheet.Cell(currentRow, 8).Clear();
                }

                if (producto.StockMinimo.HasValue)
                {
                    worksheet.Cell(currentRow, 9).Value = producto.StockMinimo.Value;
                }
                else
                {
                    worksheet.Cell(currentRow, 9).Clear();
                }

                worksheet.Cell(currentRow, 10).Value = producto.Activo ? "Sí" : "No";

                currentRow++;
            }

            if (currentRow == 2)
            {
                worksheet.Cell(currentRow, 1).Value = "Ejemplo Producto";
                worksheet.Cell(currentRow, 2).Value = "Descripción opcional";
                worksheet.Cell(currentRow, 3).Value = 0;
                worksheet.Cell(currentRow, 4).Value = 0;
                worksheet.Cell(currentRow, 5).Value = categorias.FirstOrDefault() ?? string.Empty;
                worksheet.Cell(currentRow, 10).Value = "Sí";
            }

            worksheet.Column(3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Column(4).Style.NumberFormat.Format = "0";
            worksheet.Column(7).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Column(8).Style.NumberFormat.Format = "0.00";
            worksheet.Column(9).Style.NumberFormat.Format = "0";

            worksheet.Columns().AdjustToContents();

            var categoriesSheet = workbook.Worksheets.Add("Categorias");
            categoriesSheet.Cell(1, 1).Value = "Categorías disponibles";
            categoriesSheet.Cell(1, 1).Style.Font.Bold = true;

            var categoryRow = 2;
            foreach (var categoria in categorias)
            {
                categoriesSheet.Cell(categoryRow, 1).Value = categoria;
                categoryRow++;
            }

            categoriesSheet.Columns().AdjustToContents();

            if (categorias.Any())
            {
                var lastRow = Math.Max(productos.Count + 50, 200);
                var validationRange = worksheet.Range(2, 5, lastRow, 5);
                var listRange = categoriesSheet.Range(2, 1, categorias.Count + 1, 1);
                var validation = validationRange.SetDataValidation();
                validation.List(listRange);
                validation.IgnoreBlanks = true;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<ProductoImportResult> ImportProductosAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Invalid file name", nameof(fileName));
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls" && extension != ".csv")
            {
                throw new InvalidOperationException("Formato de archivo no soportado. Utilice un archivo .xlsx o .csv.");
            }

            if (fileStream.CanSeek)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }

            var rows = extension == ".csv"
                ? ParseCsv(fileStream)
                : ParseExcel(fileStream);

            var result = new ProductoImportResult
            {
                TotalRows = rows.Count
            };

            if (!rows.Any())
            {
                return result;
            }

            var categorias = await _context.Set<Categoria>()
                .Where(c => c.Activo)
                .ToDictionaryAsync(c => c.Nombre.Trim().ToLowerInvariant(), c => c.Id, cancellationToken);

            var productos = await _context.Productos
                .ToListAsync(cancellationToken);

            var productsByName = productos
                .Where(p => !string.IsNullOrWhiteSpace(p.Nombre))
                .ToDictionary(p => p.Nombre.Trim().ToLowerInvariant(), p => p);

            var productsByCode = productos
                .Where(p => !string.IsNullOrWhiteSpace(p.CodigoBarras))
                .ToDictionary(p => p.CodigoBarras!.Trim().ToLowerInvariant(), p => p);

            var now = DateTime.UtcNow;

            foreach (var row in rows)
            {
                var rowErrors = new List<string>();

                var nombre = row.GetValue("Nombre");
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    rowErrors.Add("El nombre es obligatorio.");
                }

                var descripcion = row.GetValue("Descripcion");

                var precioText = row.GetValue("Precio");
                if (!TryParseDecimal(precioText, out var precio) || precio <= 0)
                {
                    rowErrors.Add("El precio es obligatorio y debe ser mayor a 0.");
                }

                var stockText = row.GetValue("Stock");
                if (!TryParseInt(stockText, out var stock) || stock < 0)
                {
                    rowErrors.Add("El stock es obligatorio y no puede ser negativo.");
                }

                var categoriaNombre = row.GetValue("Categoria");
                int? categoriaId = null;
                if (!string.IsNullOrWhiteSpace(categoriaNombre))
                {
                    var normalized = categoriaNombre.Trim().ToLowerInvariant();
                    if (categorias.TryGetValue(normalized, out var id))
                    {
                        categoriaId = id;
                    }
                    else
                    {
                        rowErrors.Add($"La categoría '{categoriaNombre}' no existe o está inactiva.");
                    }
                }

                var codigoBarras = row.GetValue("CodigoBarras");

                decimal? precioCompra = null;
                var precioCompraText = row.GetValue("PrecioCompra");
                if (!string.IsNullOrWhiteSpace(precioCompraText))
                {
                    if (TryParseDecimal(precioCompraText, out var parsedPrecioCompra))
                    {
                        if (parsedPrecioCompra < 0)
                        {
                            rowErrors.Add("El precio de compra no puede ser negativo.");
                        }
                        else
                        {
                            precioCompra = parsedPrecioCompra;
                        }
                    }
                    else
                    {
                        rowErrors.Add("El precio de compra debe ser un número válido.");
                    }
                }

                decimal? margenGanancia = null;
                var margenText = row.GetValue("MargenGanancia");
                if (!string.IsNullOrWhiteSpace(margenText))
                {
                    if (TryParseDecimal(margenText, out var parsedMargen))
                    {
                        if (parsedMargen < 0 || parsedMargen > 100)
                        {
                            rowErrors.Add("El margen de ganancia debe estar entre 0 y 100.");
                        }
                        else
                        {
                            margenGanancia = parsedMargen;
                        }
                    }
                    else
                    {
                        rowErrors.Add("El margen de ganancia debe ser un número válido.");
                    }
                }

                int? stockMinimo = null;
                var stockMinimoText = row.GetValue("StockMinimo");
                if (!string.IsNullOrWhiteSpace(stockMinimoText))
                {
                    if (TryParseInt(stockMinimoText, out var parsedStockMinimo))
                    {
                        if (parsedStockMinimo < 0)
                        {
                            rowErrors.Add("El stock mínimo no puede ser negativo.");
                        }
                        else
                        {
                            stockMinimo = parsedStockMinimo;
                        }
                    }
                    else
                    {
                        rowErrors.Add("El stock mínimo debe ser un número válido.");
                    }
                }

                bool? activo = null;
                var activoText = row.GetValue("Activo");
                if (!string.IsNullOrWhiteSpace(activoText))
                {
                    if (TryParseBool(activoText, out var parsedActivo))
                    {
                        activo = parsedActivo;
                    }
                    else
                    {
                        rowErrors.Add("El estado activo debe ser verdadero o falso (por ejemplo: Sí, No, true, false).");
                    }
                }

                if (rowErrors.Any())
                {
                    result.FailedCount++;
                    result.Errors.Add($"Fila {row.RowNumber}: {string.Join(' ', rowErrors)}");
                    continue;
                }

                Producto? producto = null;
                string? normalizedCode = null;
                if (!string.IsNullOrWhiteSpace(codigoBarras))
                {
                    normalizedCode = codigoBarras.Trim().ToLowerInvariant();
                    productsByCode.TryGetValue(normalizedCode, out producto);
                }

                string? normalizedName = null;
                if (producto == null && !string.IsNullOrWhiteSpace(nombre))
                {
                    normalizedName = nombre.Trim().ToLowerInvariant();
                    productsByName.TryGetValue(normalizedName, out producto);
                }

                if (producto == null)
                {
                    producto = new Producto
                    {
                        Nombre = nombre!.Trim(),
                        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                        Precio = precio,
                        Stock = stock,
                        CategoriaId = categoriaId,
                        CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? null : codigoBarras.Trim(),
                        PrecioCompra = precioCompra,
                        MargenGanancia = margenGanancia,
                        StockMinimo = stockMinimo,
                        Activo = activo ?? true,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    _context.Productos.Add(producto);
                    result.CreatedCount++;

                    normalizedName ??= producto.Nombre.Trim().ToLowerInvariant();
                    if (!productsByName.ContainsKey(normalizedName))
                    {
                        productsByName.Add(normalizedName, producto);
                    }

                    normalizedCode = producto.CodigoBarras != null ? producto.CodigoBarras.Trim().ToLowerInvariant() : null;
                    if (normalizedCode != null && !productsByCode.ContainsKey(normalizedCode))
                    {
                        productsByCode.Add(normalizedCode, producto);
                    }
                }
                else
                {
                    var oldNameKey = producto.Nombre.Trim().ToLowerInvariant();
                    var oldCodeKey = !string.IsNullOrWhiteSpace(producto.CodigoBarras) ? producto.CodigoBarras.Trim().ToLowerInvariant() : null;

                    producto.Nombre = nombre!.Trim();
                    producto.Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
                    producto.Precio = precio;
                    producto.Stock = stock;
                    producto.CategoriaId = categoriaId;
                    producto.CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? null : codigoBarras.Trim();
                    producto.PrecioCompra = precioCompra;
                    producto.MargenGanancia = margenGanancia;
                    producto.StockMinimo = stockMinimo;
                    if (activo.HasValue)
                    {
                        producto.Activo = activo.Value;
                    }
                    producto.UpdatedAt = now;

                    result.UpdatedCount++;

                    normalizedName = producto.Nombre.Trim().ToLowerInvariant();
                    if (oldNameKey != normalizedName)
                    {
                        productsByName.Remove(oldNameKey);
                    }
                    productsByName[normalizedName] = producto;

                    normalizedCode = producto.CodigoBarras != null ? producto.CodigoBarras.Trim().ToLowerInvariant() : null;
                    if (oldCodeKey != null && oldCodeKey != normalizedCode)
                    {
                        productsByCode.Remove(oldCodeKey);
                    }
                    if (normalizedCode != null)
                    {
                        productsByCode[normalizedCode] = producto;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Products import completed. Created: {Created}, Updated: {Updated}, Failed: {Failed}",
                result.CreatedCount, result.UpdatedCount, result.FailedCount);

            return result;
        }

        private static List<ProductImportRow> ParseExcel(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw new InvalidOperationException("El archivo no contiene hojas válidas.");

            var headerRow = worksheet.FirstRowUsed()
                ?? throw new InvalidOperationException("El archivo no contiene encabezados.");

            var headers = headerRow.CellsUsed()
                .ToDictionary(cell => cell.GetString().Trim(), cell => cell.Address.ColumnNumber, StringComparer.OrdinalIgnoreCase);

            if (!headers.Any())
            {
                throw new InvalidOperationException("No se encontraron encabezados en el archivo.");
            }

            var lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? headerRow.RowNumber();
            var rows = new List<ProductImportRow>();

            for (var rowNumber = headerRow.RowNumber() + 1; rowNumber <= lastRowNumber; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);
                if (row.IsEmpty())
                {
                    continue;
                }

                var importRow = new ProductImportRow(rowNumber);

                foreach (var header in headers)
                {
                    var cell = row.Cell(header.Value);
                    string? value;
                    if (cell.Value.Type == XLDataType.Number)
                    {
                        value = cell.Value.GetNumber().ToString(CultureInfo.InvariantCulture);
                    }
                    else if (cell.Value.Type == XLDataType.Boolean)
                    {
                        value = cell.Value.GetBoolean() ? "true" : "false";
                    }
                    else
                    {
                        value = cell.GetString();
                    }

                    importRow.Values[header.Key] = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
                }

                if (importRow.Values.Values.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                rows.Add(importRow);
            }

            return rows;
        }

        private static List<ProductImportRow> ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using var parser = new TextFieldParser(reader)
            {
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true
            };
            parser.SetDelimiters(",", ";");

            if (parser.EndOfData)
            {
                return new List<ProductImportRow>();
            }

            var rawHeaders = parser.ReadFields();
            if (rawHeaders == null)
            {
                throw new InvalidOperationException("No se pudieron leer los encabezados del archivo CSV.");
            }

            var headers = rawHeaders
                .Select((header, index) => new { header = (header ?? string.Empty).Trim(), index })
                .Where(item => !string.IsNullOrWhiteSpace(item.header))
                .ToList();

            if (!headers.Any())
            {
                throw new InvalidOperationException("El archivo CSV no contiene encabezados válidos.");
            }

            var rows = new List<ProductImportRow>();
            var rowNumber = 1;

            while (!parser.EndOfData)
            {
                rowNumber++;
                var fields = parser.ReadFields();
                if (fields == null)
                {
                    continue;
                }

                var importRow = new ProductImportRow(rowNumber);

                foreach (var header in headers)
                {
                    var value = header.index < fields.Length ? fields[header.index] : string.Empty;
                    importRow.Values[header.header] = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
                }

                if (importRow.Values.Values.All(string.IsNullOrWhiteSpace))
                {
                    continue;
                }

                rows.Add(importRow);
            }

            return rows;
        }

        private static bool TryParseDecimal(string? input, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Replace(" ", string.Empty);
            if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            var esCulture = new CultureInfo("es-ES");
            return decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, esCulture, out value);
        }

        private static bool TryParseInt(string? input, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Replace(" ", string.Empty);
            if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
            {
                value = (int)Math.Round(decimalValue, MidpointRounding.AwayFromZero);
                return true;
            }

            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out decimalValue))
            {
                value = (int)Math.Round(decimalValue, MidpointRounding.AwayFromZero);
                return true;
            }

            return false;
        }

        private static bool TryParseBool(string? input, out bool value)
        {
            value = true;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Trim().ToLowerInvariant();
            normalized = normalized
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");

            switch (normalized)
            {
                case "true":
                case "1":
                case "si":
                case "yes":
                case "activo":
                    value = true;
                    return true;
                case "false":
                case "0":
                case "no":
                case "inactivo":
                    value = false;
                    return true;
            }

            return bool.TryParse(input, out value);
        }

        private sealed class ProductImportRow
        {
            public ProductImportRow(int rowNumber)
            {
                RowNumber = rowNumber;
                Values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            }

            public int RowNumber { get; }

            public Dictionary<string, string?> Values { get; }

            public string? GetValue(string key)
            {
                return Values.TryGetValue(key, out var value) ? value : null;
            }
        }
    }
}
