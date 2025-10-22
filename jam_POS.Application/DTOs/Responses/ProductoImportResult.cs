using System.Collections.Generic;

namespace jam_POS.Application.DTOs.Responses
{
    public class ProductoImportResult
    {
        public int TotalRows { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
