# Módulo de Ventas - POS System

## Descripción General

El módulo de ventas es el corazón del sistema POS, permitiendo procesar transacciones de venta de manera eficiente con soporte para múltiples métodos de pago, descuentos, impuestos y integración completa con el inventario y configuraciones del sistema.

## Características Implementadas

### 🎯 Backend

#### 1. Entidades Principales

##### **Venta**
- **Campos**: NumeroVenta, Fecha, TipoDocumento (RECIBO/FACTURA), EstadoVenta (COMPLETADA/ANULADA/PENDIENTE)
- **Totales**: Subtotal, TotalDescuentos, TotalImpuestos, Total
- **Información del Cliente**: ClienteNombre, ClienteIdentificacion, ClienteTelefono
- **Multi-tenancy**: EmpresaId para aislamiento de datos
- **Auditoría**: CreatedBy, CreatedAt, UpdatedAt, AnuladaAt

##### **VentaItem**
- **Relación**: Asociado a una Venta y Producto
- **Campos**: Cantidad, PrecioUnitario, DescuentoPorcentaje, MontoDescuento, Subtotal, MontoImpuesto, Total
- **Snapshot**: Guarda ProductoNombre, ProductoCodigo, ProductoCategoria para mantener histórico

##### **Pago**
- **Métodos**: EFECTIVO, TARJETA, TRANSFERENCIA, CREDITO
- **Campos**: Monto, Referencia, Notas
- **Relación**: Múltiples pagos por venta (pagos mixtos)

#### 2. DTOs Implementados

**Requests:**
- `CreateVentaRequest`: Para crear nuevas ventas
- `AnularVentaRequest`: Para anular ventas existentes
- `VentaFilterRequest`: Para búsquedas y filtros con paginación

**Responses:**
- `VentaResponse`: Información completa de la venta
- `VentaItemResponse`: Detalle de cada producto en la venta
- `PagoResponse`: Información de cada pago
- `VentaListResponse`: Vista resumida para listados

#### 3. Servicios

**VentaService** (`IVentaService`):
```csharp
- Task<VentaResponse> CreateVentaAsync(CreateVentaRequest request, int userId)
- Task<VentaResponse> GetVentaByIdAsync(int id)
- Task<PagedResult<VentaListResponse>> GetVentasAsync(VentaFilterRequest filter)
- Task<VentaResponse> AnularVentaAsync(int id, AnularVentaRequest request, int userId)
- Task<VentaResponse> GetVentaByNumeroAsync(string numeroVenta)
```

**Características del Servicio:**
- ✅ Validación completa de stock antes de procesar venta
- ✅ Actualización automática de inventario
- ✅ Generación automática de números de recibo/factura
- ✅ Cálculo automático de totales, descuentos e impuestos
- ✅ Validación de métodos de pago configurados
- ✅ Validación de límites de descuento
- ✅ Validación de tiempo límite para anulaciones
- ✅ Restauración de inventario al anular ventas

#### 4. Controlador

**VentasController**:
```csharp
- POST /api/ventas - Crear venta
- GET /api/ventas/{id} - Obtener venta por ID
- GET /api/ventas - Listar ventas con filtros
- POST /api/ventas/{id}/anular - Anular venta
- GET /api/ventas/numero/{numeroVenta} - Buscar por número
```

#### 5. Configuración de Base de Datos

**ApplicationDbContext:**
- Configuración de relaciones entre Venta, VentaItem, Pago, Producto, Usuario, Empresa
- Global Query Filters para multi-tenancy
- Índices para optimización de búsquedas
- Precisión decimal para campos monetarios

### 🎨 Frontend

#### 1. Componente Principal: `SalesPOSComponent`

**Características:**
- ✅ Interfaz de punto de venta moderna y responsive
- ✅ Búsqueda de productos en tiempo real
- ✅ Gestión de carrito de compras
- ✅ Edición de precios y descuentos por ítem
- ✅ Control de cantidades con botones + / -
- ✅ Múltiples métodos de pago (mixtos)
- ✅ Cálculo automático de totales, descuentos e impuestos
- ✅ Indicador de cambio para efectivo
- ✅ Integración con configuraciones POS

#### 2. Servicios Angular

**SalesService** (`sales.service.ts`):
```typescript
- createSale(request: CreateSaleRequest): Observable<Sale>
- getSaleById(id: number): Observable<Sale>
- getSales(filter: SaleFilterRequest): Observable<PagedResult<SaleListItem>>
- cancelSale(id: number, request: CancelSaleRequest): Observable<Sale>
- getSaleByNumber(saleNumber: string): Observable<Sale>
```

#### 3. Modelos TypeScript

- `Sale`: Modelo completo de venta
- `SaleItem`: Ítem de venta
- `Payment`: Pago
- `SaleListItem`: Vista resumida
- `CreateSaleRequest`: DTO de creación
- `CancelSaleRequest`: DTO de anulación
- `SaleFilterRequest`: Filtros de búsqueda

#### 4. UI/UX Destacadas

**Layout de Dos Columnas:**
- **Panel Izquierdo**: Búsqueda y selección de productos
- **Panel Derecho**: Carrito, totales, pagos y procesamiento

**Características de Usabilidad:**
- Grid de productos con precios y códigos
- Campos editables en el carrito (precio, descuento)
- Botones grandes para métodos de pago
- Estados visuales claros (vacío, cargando, error)
- Confirmaciones y alertas con SnackBar
- Diseño responsive para tablets y móviles

## Integraciones

### ✅ Integración con Productos
- Búsqueda y selección de productos
- Validación de stock disponible
- Actualización automática de inventario
- Snapshot de información del producto

### ✅ Integración con Configuraciones POS
- Respeto de métodos de pago habilitados
- Uso de impuesto por defecto configurado
- Generación de números según prefijos y secuencias
- Validación de límites de descuento
- Validación de tiempo límite de anulación

### ✅ Integración con Impuestos
- Aplicación automática del impuesto por defecto
- Cálculo correcto de montos (porcentual o fijo)
- Inclusión en totales de venta

### ✅ Multi-tenancy
- Aislamiento completo de datos por empresa
- TenantId automático desde JWT
- Query filters en todas las consultas
- Validaciones cruzadas entre entidades del mismo tenant

## Validaciones Implementadas

### Backend

1. **Validación de Stock**
   - Verificación de disponibilidad antes de procesar
   - Prevención de ventas con inventario negativo

2. **Validación de Configuración POS**
   - Existencia de configuración para el tenant
   - Métodos de pago habilitados
   - Límites de descuento

3. **Validación de Productos**
   - Existencia y activación
   - Pertenencia al tenant correcto

4. **Validación de Impuestos**
   - Existencia y activación del impuesto por defecto

5. **Validación de Anulaciones**
   - Tiempo límite desde creación
   - Estado de la venta (no anulada previamente)

6. **Validación de Pagos**
   - Monto total coincide con total de venta
   - Al menos un método de pago

### Frontend

1. **Validación de Carrito**
   - Al menos un producto
   - Cantidades válidas (> 0)

2. **Validación de Precios**
   - Precios >= 0
   - Descuentos entre 0-100%

3. **Validación de Pagos**
   - Total de pagos >= total de venta
   - Indicador visual de faltante/cambio

## Seguridad

- ✅ Autenticación JWT requerida
- ✅ Validación de pertenencia al tenant
- ✅ Auditoría de creación y anulación (UserId)
- ✅ Validación de permisos implícita por tenant

## Performance

- ✅ Paginación en listados
- ✅ Índices en campos de búsqueda frecuente
- ✅ Lazy loading del módulo en Angular
- ✅ Búsqueda debounced en frontend
- ✅ TrackBy en listas para optimización de rendering

## Próximos Pasos Sugeridos

### Alta Prioridad
1. **Impresión de Recibos/Facturas**
   - Plantillas de impresión
   - Preview antes de imprimir
   - Soporte para impresoras térmicas

2. **Gestión de Clientes**
   - CRUD de clientes
   - Autocompletado en ventas
   - Historial de compras

3. **Reportes de Ventas**
   - Ventas por período
   - Productos más vendidos
   - Métodos de pago más usados
   - Gráficos y dashboard

### Media Prioridad
4. **Devoluciones**
   - Proceso de devolución de productos
   - Ajuste de inventario
   - Notas de crédito

5. **Cuentas por Cobrar**
   - Seguimiento de ventas a crédito
   - Pagos parciales
   - Estados de cuenta

6. **Arqueo de Caja**
   - Apertura y cierre de caja
   - Reconciliación de pagos
   - Reportes de movimientos

### Baja Prioridad
7. **Modo Offline**
   - Sincronización cuando hay conexión
   - Cola de ventas pendientes
   - Manejo de conflictos

8. **Integraciones Externas**
   - Facturación electrónica
   - Pasarelas de pago
   - Software de contabilidad

## Estructura de Archivos

```
Backend:
├── jam_POS.Core/Entities/
│   ├── Venta.cs
│   ├── VentaItem.cs
│   └── Pago.cs
├── jam_POS.Application/
│   ├── DTOs/Requests/
│   │   ├── CreateVentaRequest.cs
│   │   ├── AnularVentaRequest.cs
│   │   └── VentaFilterRequest.cs
│   ├── DTOs/Responses/
│   │   ├── VentaResponse.cs
│   │   ├── VentaItemResponse.cs
│   │   ├── PagoResponse.cs
│   │   └── VentaListResponse.cs
│   └── Services/
│       ├── IVentaService.cs
│       └── VentaService.cs
├── jam_POS.API/Controllers/
│   └── VentasController.cs
└── jam_POS.Infrastructure/
    └── Data/ApplicationDbContext.cs (configuración)

Frontend:
└── ClientApp/src/app/features/sales/
    ├── models/
    │   ├── sale.model.ts
    │   └── payment.model.ts
    ├── services/
    │   └── sales.service.ts
    ├── components/
    │   └── sales-pos/
    │       ├── sales-pos.component.ts
    │       ├── sales-pos.component.html
    │       └── sales-pos.component.scss
    └── sales.module.ts
```

## Notas Técnicas

- **Precisión Decimal**: Todos los campos monetarios usan `decimal(18,2)`
- **Números de Venta**: Formato `{Prefijo}{Número}` ejemplo: `REC000001`
- **Estado de Venta**: Enum con valores COMPLETADA, ANULADA, PENDIENTE
- **Tipo de Documento**: Enum con valores RECIBO, FACTURA
- **Método de Pago**: Enum con valores EFECTIVO, TARJETA, TRANSFERENCIA, CREDITO
- **Snapshot de Productos**: Preserva información histórica incluso si el producto se modifica

## Testing Sugerido

1. ✅ Crear venta simple con un producto
2. ✅ Crear venta con múltiples productos
3. ✅ Aplicar descuentos a ítems individuales
4. ✅ Usar múltiples métodos de pago
5. ✅ Validar actualización de inventario
6. ✅ Anular venta dentro del tiempo límite
7. ✅ Intentar anular venta fuera del tiempo límite
8. ✅ Validar límites de descuento
9. ✅ Intentar vender producto sin stock
10. ✅ Probar filtros y paginación en listados

---

**Fecha de Implementación**: Octubre 2025  
**Versión**: 1.0.0  
**Estado**: ✅ Completado

