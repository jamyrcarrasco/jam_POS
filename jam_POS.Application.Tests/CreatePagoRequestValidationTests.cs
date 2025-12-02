using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using jam_POS.Application.DTOs.Requests;
using Xunit;

namespace jam_POS.Application.Tests;

public class CreatePagoRequestValidationTests
{
    [Fact]
    public void Tarjeta_DebeRequerirTipoTarjetaYReferencia()
    {
        var pago = new CreatePagoRequest
        {
            MetodoPago = "TARJETA",
            Monto = 100,
            Referencia = null,
            TipoTarjeta = null
        };

        var results = Validate(pago);

        Assert.Contains(results, r => r.ErrorMessage == "El tipo de tarjeta es requerido para pagos con TARJETA.");
        Assert.Contains(results, r => r.ErrorMessage == "La referencia es requerida para pagos con TARJETA.");
    }

    [Fact]
    public void Transferencia_DebeRequerirBancoYReferencia()
    {
        var pago = new CreatePagoRequest
        {
            MetodoPago = "TRANSFERENCIA",
            Monto = 75,
            Banco = null,
            Referencia = null
        };

        var results = Validate(pago);

        Assert.Contains(results, r => r.ErrorMessage == "El banco es requerido para pagos con TRANSFERENCIA.");
        Assert.Contains(results, r => r.ErrorMessage == "La referencia es requerida para pagos con TRANSFERENCIA.");
    }

    private static IList<ValidationResult> Validate(object instance)
    {
        var context = new ValidationContext(instance);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, context, results, validateAllProperties: true);
        return results;
    }
}
