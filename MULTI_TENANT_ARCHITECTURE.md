# 🏢 Arquitectura Multi-Tenant - jamPOS

## Descripción General

El sistema jamPOS implementa un modelo **multi-tenant** donde cada empresa tiene sus propios datos completamente aislados. Los datos de una empresa **NUNCA** se mezclan con los de otra.

## 🔐 Componentes del Sistema Multi-Tenant

### 1. Entidades con Tenant

Todas las entidades que deben estar aisladas por empresa implementan `ITenantEntity`:

```csharp
- Producto      ✅ EmpresaId
- Categoria     ✅ EmpresaId
- User          ✅ EmpresaId
- Role          ✅ EmpresaId (excepto IsSystem=true)
```

### 2. Flujo de Aislamiento de Datos

```
┌──────────────────────────────────────────────────────┐
│ 1. Usuario inicia sesión                             │
│    → AuthService valida credenciales                 │
│    → JwtService genera token con TenantId            │
│    → Token: { userId, role, TenantId: 5 }           │
└──────────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────┐
│ 2. Cliente hace request con token                    │
│    → GET /api/productos                              │
│    → Headers: { Authorization: "Bearer token..." }   │
└──────────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────┐
│ 3. TenantMiddleware extrae TenantId del token        │
│    → Lee claim "TenantId" = 5                        │
│    → TenantProvider.SetTenantId(5)                   │
│    → Todos los requests subsecuentes usan TenantId=5│
└──────────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────┐
│ 4. Global Query Filter filtra automáticamente        │
│    → Query: SELECT * FROM Productos                  │
│    → Auto-filtrado: WHERE EmpresaId = 5              │
│    → Solo devuelve productos de la Empresa 5         │
└──────────────────────────────────────────────────────┘
                       ↓
┌──────────────────────────────────────────────────────┐
│ 5. Al crear nuevos registros                         │
│    → ProductoService.CreateProducto(...)             │
│    → SaveChanges detecta nueva entidad               │
│    → Auto-asigna: producto.EmpresaId = 5            │
│    → Guarda con TenantId automáticamente             │
└──────────────────────────────────────────────────────┘
```

## 🛡️ Niveles de Protección

### Nivel 1: JWT Token
- ✅ TenantId encriptado en el token
- ✅ No puede ser modificado por el cliente
- ✅ Validado en cada request

### Nivel 2: Middleware
- ✅ Extrae TenantId del token validado
- ✅ Establece contexto para el request completo
- ✅ Thread-safe usando AsyncLocal

### Nivel 3: Global Query Filters
- ✅ EF Core filtra automáticamente TODAS las consultas
- ✅ Imposible acceder a datos de otro tenant en queries
- ✅ Aplicado a: Productos, Categorías, Usuarios

### Nivel 4: Auto-asignación
- ✅ SaveChanges asigna TenantId automáticamente
- ✅ No requiere código en cada servicio
- ✅ Imposible olvidar asignar el tenant

## 📋 Ejemplo de Operaciones

### Crear Producto (Empresa 5)
```csharp
// El desarrollador solo hace esto:
var producto = new Producto { Nombre = "Coca Cola", Precio = 2.50 };
context.Productos.Add(producto);
await context.SaveChangesAsync();

// El sistema automáticamente hace esto:
// producto.EmpresaId = 5; (del TenantProvider)
// INSERT INTO Productos (Nombre, Precio, EmpresaId) VALUES ('Coca Cola', 2.50, 5)
```

### Consultar Productos (Empresa 5)
```csharp
// El desarrollador solo hace esto:
var productos = await context.Productos.ToListAsync();

// El sistema automáticamente ejecuta esto:
// SELECT * FROM Productos WHERE EmpresaId = 5
// Solo devuelve productos de la Empresa 5
```

### Actualizar Producto (Empresa 5)
```csharp
// El usuario de Empresa 5 intenta:
var producto = await context.Productos.FindAsync(100);
// Si producto.EmpresaId = 7 → producto será NULL (filtro global)
// Si producto.EmpresaId = 5 → producto se devuelve normalmente
```

## 🚫 Escenarios de Seguridad

### ❌ Intentar acceder a datos de otro tenant
```
Empresa A (TenantId=5) intenta:
GET /api/productos/999 (donde producto.EmpresaId=7)

Resultado: 404 Not Found
Razón: Global Query Filter filtra el producto
```

### ❌ Intentar crear con otro TenantId
```csharp
var producto = new Producto 
{ 
    Nombre = "Producto",
    EmpresaId = 7  // Usuario de Empresa 5 intenta crear en Empresa 7
};

// SaveChanges SOBRESCRIBE con el TenantId correcto
// producto.EmpresaId = 5 (del contexto actual)
```

## 🔧 Configuración Técnica

### ApplicationDbContext.cs
```csharp
// Constructor inyecta ITenantProvider
public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantProvider tenantProvider) : base(options)
{
    _tenantProvider = tenantProvider;
}

// Global Query Filters
modelBuilder.Entity<Producto>().HasQueryFilter(
    e => e.EmpresaId == null || 
         e.EmpresaId == _tenantProvider!.GetTenantId()
);

// Auto-asignar TenantId en SaveChanges
private void ApplyTenantId()
{
    var tenantId = _tenantProvider.GetTenantId();
    var entries = ChangeTracker.Entries<ITenantEntity>()
        .Where(e => e.State == EntityState.Added);
    
    foreach (var entry in entries)
    {
        entry.Entity.EmpresaId = tenantId.Value;
    }
}
```

### Program.cs
```csharp
// Registrar TenantProvider
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Registrar Middleware (después de UseAuthentication)
app.UseAuthentication();
app.UseTenantMiddleware();  // ← Aquí
app.UseAuthorization();
```

## ✅ Garantías del Sistema

1. ✅ **Aislamiento Total**: Cada empresa solo ve sus datos
2. ✅ **Automático**: No requiere código manual en cada servicio
3. ✅ **Seguro**: Imposible acceder a datos de otro tenant
4. ✅ **Transparente**: Los servicios no necesitan saber del tenant
5. ✅ **Thread-Safe**: Usa AsyncLocal para concurrencia
6. ✅ **Validado en JWT**: El tenant viene del token validado

## 🎯 Excepciones (Datos Globales)

Algunas entidades son globales (compartidas entre todas las empresas):

- ✅ **Permission**: Los permisos son globales (catálogo del sistema)
- ✅ **Role (IsSystem=true)**: Roles de sistema (SuperAdmin, Seller) son globales
- ✅ **Role (IsSystem=false)**: Roles personalizados están separados por empresa
- ✅ **RolePermission**: Se filtran según el rol (sistema o del tenant)
- ❌ **Empresa**: Obviamente no tiene tenant
- ❌ **User sin empresa**: Usuarios del sistema (admin@jampos.com - solo para mantenimiento)

## 📊 Testing

### Probar Aislamiento
```bash
# Empresa 1
POST /auth/login { username: "user1", password: "..." }
→ Token con TenantId=1

GET /api/productos
→ Solo productos con EmpresaId=1

# Empresa 2
POST /auth/login { username: "user2", password: "..." }
→ Token con TenantId=2

GET /api/productos
→ Solo productos con EmpresaId=2
```

## 🔍 Debugging

Para verificar que el tenant está configurado correctamente:

```csharp
// En cualquier servicio:
var currentTenantId = _tenantProvider.GetTenantId();
_logger.LogInformation("TenantId actual: {TenantId}", currentTenantId);
```

## 🚀 Ventajas de esta Implementación

1. **Código Limpio**: Los servicios no tienen lógica de tenant
2. **DRY**: No repetir filtros WHERE EmpresaId en cada query
3. **Seguro**: Imposible olvidar filtrar por tenant
4. **Mantenible**: Cambios centralizados en DbContext
5. **Escalable**: Agregar nuevas entidades es trivial
6. **Performance**: Los índices en EmpresaId optimizan queries

---

**⚠️ IMPORTANTE**: Nunca usar `.IgnoreQueryFilters()` a menos que sea absolutamente necesario y con permisos de SuperAdmin del sistema (no de empresa).

