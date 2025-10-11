namespace jam_POS.Application.DTOs.Responses
{
    public class ImpuestoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Porcentaje { get; set; }
        public decimal? MontoFijo { get; set; }
        public bool AplicaPorDefecto { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

