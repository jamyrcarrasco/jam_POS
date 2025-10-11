namespace jam_POS.Application.DTOs.Responses
{
    public class EmpresaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string? RNC { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string? CodigoPostal { get; set; }
        public string Plan { get; set; } = string.Empty;
        public DateTime? FechaVencimientoPlan { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsersCount { get; set; }
        public int ProductsCount { get; set; }
    }
}

