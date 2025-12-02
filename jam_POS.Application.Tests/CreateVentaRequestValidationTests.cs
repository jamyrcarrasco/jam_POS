using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using jam_POS.Application.DTOs.Requests;
using Xunit;

namespace jam_POS.Application.Tests;

public class CreateVentaRequestValidationTests
{
    [Fact]
    public void Credito_DebeTenerClienteOReferencia()
    {
        var request = new CreateVentaRequest
        {
            ClienteId = null,
            Items = new List<CreateVentaItemRequest>
            {
                new CreateVentaItemRequest
                {
                    ProductoId = 1,
                    Cantidad = 1,
                    PrecioUnitario = 10
                }
            },
            Pagos = new List<CreatePagoRequest>
            {
                new CreatePagoRequest
                {
                    MetodoPago = "CREDITO",
                    Monto = 10,
                    Referencia = null
                }
            }
        };

        var results = Validate(request);

        Assert.Contains(results, r => r.ErrorMessage == "Para pagos a crédito se requiere especificar el cliente o una referencia de crédito.");
    }

    private static IList<ValidationResult> Validate(object instance)
    {
        var context = new ValidationContext(instance);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, context, results, validateAllProperties: true);
        return results;
    }
}
