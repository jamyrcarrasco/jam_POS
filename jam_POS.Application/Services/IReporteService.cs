using jam_POS.Application.DTOs.Requests;
using jam_POS.Application.DTOs.Responses;

namespace jam_POS.Application.Services
{
    public interface IReporteService
    {
        Task<SalesReportResponse> GetSalesReportAsync(SalesReportFilterRequest filter);
    }
}
