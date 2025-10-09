namespace jam_POS.Application.DTOs.Responses
{
    public class CategoriaResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
        public string? Icono { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ProductosCount { get; set; }
    }
}

