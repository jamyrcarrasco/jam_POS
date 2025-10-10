namespace jam_POS.Infrastructure.Services
{
    /// <summary>
    /// Implementación del proveedor de Tenant usando AsyncLocal para thread-safety
    /// </summary>
    public class TenantProvider : ITenantProvider
    {
        private static readonly AsyncLocal<int?> _tenantId = new AsyncLocal<int?>();

        public int? GetTenantId()
        {
            return _tenantId.Value;
        }

        public void SetTenantId(int? tenantId)
        {
            _tenantId.Value = tenantId;
        }
    }
}

