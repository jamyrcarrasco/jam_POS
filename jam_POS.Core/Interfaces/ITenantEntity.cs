namespace jam_POS.Core.Interfaces
{
    /// <summary>
    /// Interface para entidades que deben estar aisladas por empresa (tenant)
    /// </summary>
    public interface ITenantEntity
    {
        int? EmpresaId { get; set; }
    }
}

