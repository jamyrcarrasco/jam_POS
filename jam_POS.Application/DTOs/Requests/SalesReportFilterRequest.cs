namespace jam_POS.Application.DTOs.Requests
{
    public class SalesReportFilterRequest
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
    }
}
