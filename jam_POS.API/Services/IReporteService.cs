using jam_POS.API.DTOs;

namespace jam_POS.API.Services
{
    public interface IReporteService
    {
        Task<SalesReportApiResponse> GetSalesReportAsync(SalesReportFilterRequest filter);
    }
}
