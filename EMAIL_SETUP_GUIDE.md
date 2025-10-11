# 📧 Guía de Configuración de Emails - jamPOS

## 🚀 Configuración de SendGrid (Plan Gratuito)

### Paso 1: Crear Cuenta en SendGrid

1. Ve a [https://signup.sendgrid.com/](https://signup.sendgrid.com/)
2. Regístrate con tu email
3. Verifica tu cuenta por email
4. Completa el cuestionario inicial

### Paso 2: Crear API Key

1. En el dashboard de SendGrid, ve a **Settings** → **API Keys**
2. Click en **"Create API Key"**
3. Configura:
   - **Name**: jamPOS-Production (o el nombre que prefieras)
   - **Permissions**: Full Access (o Mail Send si solo quieres enviar emails)
4. Click en **"Create & View"**
5. **¡IMPORTANTE!** Copia la API Key inmediatamente (solo se muestra una vez)

### Paso 3: Verificar Remitente (Sender Verification)

**Opción A: Verificación de Email Único (Más Rápido)**

1. Ve a **Settings** → **Sender Authentication**
2. Click en **"Verify a Single Sender"**
3. Completa el formulario:
   - From Name: jamPOS
   - From Email: tu-email@gmail.com (o el que uses)
   - Reply To: tu-email@gmail.com
   - Company Address: (tu dirección)
4. Click **"Create"**
5. Verifica tu email haciendo click en el link que te envían

**Opción B: Autenticar Dominio (Profesional)**

1. Ve a **Settings** → **Sender Authentication**
2. Click en **"Authenticate Your Domain"**
3. Sigue el asistente para configurar DNS

### Paso 4: Configurar jamPOS

Edita `jam_POS.API/appsettings.json`:

```json
{
  "SendGrid": {
    "ApiKey": "SG.xxxxxxxxxxxxxxxxxxxxx",  // ← Tu API Key aquí
    "FromEmail": "tu-email@gmail.com",      // ← Email verificado
    "FromName": "jamPOS"
  }
}
```

**Para Desarrollo (`appsettings.Development.json`):**

```json
{
  "SendGrid": {
    "ApiKey": "SG.xxxxxxxxxxxxxxxxxxxxx",
    "FromEmail": "dev@tuempresa.com",
    "FromName": "jamPOS [DEV]"
  }
}
```

## 📧 Emails Configurados

### 1. Email de Bienvenida (Registro de Empresa)

**Cuándo se envía:**
- Al registrar una nueva empresa en `/auth/register`

**Destinatario:**
- Email del administrador de la empresa

**Contenido:**
- ✅ Bienvenida personalizada
- ✅ Credenciales de acceso
- ✅ Primeros pasos
- ✅ Link de acceso directo
- ✅ Información del plan de prueba

### 2. Email de Nuevo Usuario

**Cuándo se envía:**
- Al crear un nuevo usuario desde `/configuraciones/usuarios`

**Destinatario:**
- Email del nuevo usuario

**Contenido:**
- ✅ Notificación de cuenta creada
- ✅ Usuario y contraseña temporal
- ✅ Nombre de la empresa
- ✅ Link de inicio de sesión
- ✅ Recomendación de cambiar contraseña

### 3. Email de Recuperación de Contraseña

**Cuándo se envía:**
- Al solicitar recuperación de contraseña (pendiente de implementar)

**Contenido:**
- ✅ Link con token de recuperación
- ✅ Tiempo de expiración
- ✅ Advertencia de seguridad

## 🎨 Diseño de Plantillas

Todas las plantillas incluyen:

- ✅ **Responsive**: Se ven bien en móvil y escritorio
- ✅ **Modernas**: Gradientes, colores vibrantes, íconos
- ✅ **Profesionales**: Layout limpio y organizado
- ✅ **Branded**: Logo y colores de jamPOS
- ✅ **CTAs claros**: Botones de acción destacados

## 🔧 Personalización

### Cambiar Colores de los Emails

Edita `EmailService.cs` y busca los estilos inline:

```csharp
// Email de Bienvenida (Morado)
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%)

// Email de Nuevo Usuario (Verde)
background: linear-gradient(135deg, #10b981 0%, #059669 100%)

// Email de Reset Password (Naranja)
background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%)
```

### Agregar Logo de tu Empresa

```html
<img src='https://tudominio.com/logo.png' 
     alt='Logo' 
     style='height: 40px; margin-bottom: 10px;' />
```

## 📊 Plan Gratuito de SendGrid

| Característica | Límite |
|----------------|--------|
| Emails/día | 100 |
| Duración | Permanente |
| API Keys | Ilimitadas |
| Plantillas | Sí |
| Estadísticas | Básicas |
| Soporte | Comunidad |

## ⚡ Testing

### Probar Emails Localmente

1. Configura tu API Key en `appsettings.Development.json`
2. Registra una empresa nueva
3. Verifica que llegue el email de bienvenida
4. Crea un usuario nuevo
5. Verifica que llegue el email con credenciales

### Logs

Los logs te dirán si los emails se enviaron:

```
[Information] Email de bienvenida enviado a: admin@mitienda.com
[Information] Email de nuevo usuario enviado a: usuario@mitienda.com
```

## 🛠️ Troubleshooting

### Email no se envía

1. **Verificar API Key**: Debe estar correcta en appsettings.json
2. **Verificar Sender**: El FromEmail debe estar verificado en SendGrid
3. **Revisar Logs**: Buscar errores en la consola
4. **Verificar Spam**: El email puede estar en spam

### Error 403 Forbidden

- Tu remitente (FromEmail) no está verificado en SendGrid
- Ve a Sender Authentication y verifica tu email

### Error 401 Unauthorized

- Tu API Key es inválida o expiró
- Genera una nueva API Key en SendGrid

## 🎯 Próximos Pasos

Emails adicionales que puedes implementar:

- ✅ Bienvenida al registrar empresa
- ✅ Nuevo usuario creado
- ✅ Recuperación de contraseña
- ⏳ Confirmación de venta
- ⏳ Alerta de stock bajo
- ⏳ Reporte diario/semanal
- ⏳ Vencimiento de plan
- ⏳ Factura mensual

## 📞 Soporte

- **SendGrid Docs**: https://docs.sendgrid.com/
- **jamPOS Support**: soporte@jampos.com

---

**¡Tu sistema de emails está listo!** 📨

