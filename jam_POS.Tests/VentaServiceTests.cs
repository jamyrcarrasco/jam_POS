using System;
using System.Security.Claims;
using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;
using jam_POS.Application.Services;
using jam_POS.Core.Entities;
using jam_POS.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace jam_POS.Tests
{
    public class VentaServiceTests
    {
        [Fact]
        public async Task CreateVentaAsync_AppliesNetCashContribution()
        {
            var service = CreateService(nameof(CreateVentaAsync_AppliesNetCashContribution), out var context);

            var request = new CreateVentaRequest
            {
                Items =
                [
                    new CreateVentaItemRequest
                    {
                        ProductoId = 1,
                        Cantidad = 1,
                        PrecioUnitario = 100m
                    }
                ],
                Pagos =
                [
                    new CreatePagoRequest
                    {
                        MetodoPago = "EFECTIVO",
                        Monto = 70m,
                        MontoRecibido = 70m,
                        CambioDevolver = 10m
                    },
                    new CreatePagoRequest
                    {
                        MetodoPago = "TARJETA",
                        Monto = 40m
                    }
                ]
            };

            var response = await service.CreateVentaAsync(request);

            Assert.Equal(100m, response.Total);
            Assert.Equal(2, context.Pagos.Count());
        }

        [Fact]
        public async Task CreateVentaAsync_ThrowsWhenNetPaymentsAreInsufficient()
        {
            var service = CreateService(nameof(CreateVentaAsync_ThrowsWhenNetPaymentsAreInsufficient), out _);

            var request = new CreateVentaRequest
            {
                Items =
                [
                    new CreateVentaItemRequest
                    {
                        ProductoId = 1,
                        Cantidad = 1,
                        PrecioUnitario = 50m
                    }
                ],
                Pagos =
                [
                    new CreatePagoRequest
                    {
                        MetodoPago = "EFECTIVO",
                        Monto = 50m,
                        MontoRecibido = 50m,
                        CambioDevolver = 20m
                    },
                    new CreatePagoRequest
                    {
                        MetodoPago = "TARJETA",
                        Monto = 10m
                    }
                ]
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateVentaAsync(request));
        }

        private static VentaService CreateService(string databaseName, out ApplicationDbContext context)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            context = new ApplicationDbContext(options);
            SeedData(context);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    ], "Test"))
            };

            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = httpContext
            };

            return new VentaService(
                context,
                NullLogger<VentaService>.Instance,
                new FakeConfiguracionPOSService(),
                httpContextAccessor);
        }

        private static void SeedData(ApplicationDbContext context)
        {
            var role = new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrador",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Roles.Add(role);

            context.Users.Add(new User
            {
                Id = 1,
                Username = "usuario",
                Email = "usuario@example.com",
                PasswordHash = "hash",
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            context.Productos.Add(new Producto
            {
                Id = 1,
                Nombre = "Producto de prueba",
                Precio = 100m,
                Stock = 10,
                CodigoBarras = "P001",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            context.SaveChanges();
        }
    }

    internal class FakeConfiguracionPOSService : IConfiguracionPOSService
    {
        private int _contador = 1;

        public Task<ConfiguracionPOSResponse?> GetConfiguracionAsync()
        {
            return Task.FromResult<ConfiguracionPOSResponse?>(new ConfiguracionPOSResponse
            {
                PrefijoRecibo = "VTA",
                PrefijoFactura = "FAC",
                SiguienteNumeroRecibo = _contador,
                SiguienteNumeroFactura = _contador,
                PermitirDescuentos = true,
                PermitirDevoluciones = true,
                TiempoLimiteAnulacionMinutos = 60,
                DescuentoMaximoPorcentaje = 100,
                EfectivoHabilitado = true,
                TarjetaHabilitado = true,
                TransferenciaHabilitado = true,
                CreditoHabilitado = true,
                MonedaPorDefecto = "DOP",
                SimboloMoneda = "RD$",
                DecimalesMoneda = 2
            });
        }

        public Task<ConfiguracionPOSResponse> CreateConfiguracionAsync(CreateConfiguracionPOSRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ConfiguracionPOSResponse> UpdateConfiguracionAsync(int id, UpdateConfiguracionPOSRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteConfiguracionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSiguienteNumeroReciboAsync()
        {
            return Task.FromResult($"VTA-{_contador++:D6}");
        }

        public Task<string> GetSiguienteNumeroFacturaAsync()
        {
            return Task.FromResult($"FAC-{_contador++:D6}");
        }
    }
}
