using System.ComponentModel.DataAnnotations;

namespace jam_POS.API.DTOs
{
    public class SalesReportFilterRequest
    {
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12")]
        public int? Month { get; set; }

        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
        public int? Year { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UsuarioId { get; set; }
        public string? TipoReporte { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
    }
}
