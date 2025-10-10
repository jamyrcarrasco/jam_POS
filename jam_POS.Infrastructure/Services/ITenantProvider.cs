namespace jam_POS.Infrastructure.Services
{
    /// <summary>
    /// Proveedor del Tenant ID actual desde el contexto HTTP
    /// </summary>
    public interface ITenantProvider
    {
        int? GetTenantId();
        void SetTenantId(int? tenantId);
    }
}

