using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace jam_POS.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string? _apiKey;
        private readonly string? _fromEmail;
        private readonly string? _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["SendGrid:ApiKey"];
            _fromEmail = _configuration["SendGrid:FromEmail"];
            _fromName = _configuration["SendGrid:FromName"];
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string empresaName, string adminName, string username)
        {
            var subject = $"🎉 ¡Bienvenido a jamPOS, {empresaName}!";
            var htmlContent = GetWelcomeEmailTemplate(empresaName, adminName, username);
            
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendNewUserEmailAsync(string toEmail, string userName, string tempPassword, string empresaName)
        {
            var subject = $"Tu cuenta en jamPOS - {empresaName}";
            var htmlContent = GetNewUserEmailTemplate(userName, tempPassword, empresaName);
            
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
        {
            var subject = "Recuperar contraseña - jamPOS";
            var htmlContent = GetPasswordResetEmailTemplate(userName, resetToken);
            
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                _logger.LogInformation("📧 Intentando enviar email a: {Email} - Asunto: {Subject}", toEmail, subject);

                if (string.IsNullOrEmpty(_apiKey))
                {
                    _logger.LogWarning("⚠️ SendGrid API Key no configurada. Email no enviado a: {Email}", toEmail);
                    _logger.LogWarning("💡 Configura 'SendGrid:ApiKey' en appsettings.json");
                    return false;
                }

                if (string.IsNullOrEmpty(_fromEmail))
                {
                    _logger.LogWarning("⚠️ SendGrid FromEmail no configurado. Usando email por defecto.");
                }

                _logger.LogDebug("📤 Preparando email con SendGrid...");
                _logger.LogDebug("From: {FromEmail} ({FromName})", _fromEmail ?? "noreply@jampos.com", _fromName ?? "jamPOS");
                _logger.LogDebug("To: {ToEmail}", toEmail);

                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail ?? "noreply@jampos.com", _fromName ?? "jamPOS");
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
                
                _logger.LogDebug("🚀 Enviando email a SendGrid...");
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ Email enviado exitosamente a: {Email} - Status: {Status}", toEmail, response.StatusCode);
                    return true;
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("❌ Error al enviar email. Status: {Status}", response.StatusCode);
                    _logger.LogError("Detalles del error: {Body}", responseBody);
                    _logger.LogError("A: {Email}, Asunto: {Subject}", toEmail, subject);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Excepción al enviar email a: {Email}", toEmail);
                return false;
            }
        }

        #region Email Templates

        private string GetWelcomeEmailTemplate(string empresaName, string adminName, string username)
        {
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Bienvenido a jamPOS</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f3f4f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    
                    <!-- Header con gradiente -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: 700;'>
                                jam<span style='font-weight: 300;'>POS</span>
                            </h1>
                            <p style='color: #ffffff; margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>
                                Tu Sistema POS en la Nube
                            </p>
                        </td>
                    </tr>

                    <!-- Contenido -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px;'>
                                🎉 ¡Bienvenido a bordo, {adminName}!
                            </h2>
                            
                            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Estamos emocionados de tenerte en <strong>jamPOS</strong>. Tu empresa 
                                <strong>{empresaName}</strong> ha sido registrada exitosamente.
                            </p>

                            <div style='background-color: #f9fafb; border-left: 4px solid #3b82f6; padding: 20px; margin: 20px 0; border-radius: 4px;'>
                                <p style='margin: 0 0 10px 0; color: #1f2937; font-weight: 600;'>
                                    📋 Tus credenciales de acceso:
                                </p>
                                <p style='margin: 5px 0; color: #4b5563;'>
                                    <strong>Usuario:</strong> {username}
                                </p>
                                <p style='margin: 5px 0; color: #4b5563;'>
                                    <strong>Rol:</strong> Super Administrador
                                </p>
                            </div>

                            <h3 style='color: #1f2937; margin: 30px 0 15px 0; font-size: 18px;'>
                                ✨ Primeros pasos:
                            </h3>
                            
                            <ol style='color: #4b5563; line-height: 1.8; margin: 0 0 30px 0; padding-left: 20px;'>
                                <li>Configura las categorías de tus productos</li>
                                <li>Agrega tu inventario de productos</li>
                                <li>Crea usuarios para tu equipo</li>
                                <li>Personaliza los roles y permisos</li>
                                <li>¡Comienza a vender!</li>
                            </ol>

                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='https://localhost:44425/auth/login' 
                                   style='display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                          color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; 
                                          font-weight: 600; font-size: 16px; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);'>
                                    🚀 Acceder a jamPOS
                                </a>
                            </div>

                            <div style='background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                                <p style='margin: 0; color: #92400e; font-size: 14px;'>
                                    💡 <strong>Tienes 30 días de prueba gratis</strong> con todas las funciones premium activadas.
                                </p>
                            </div>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='margin: 0 0 10px 0; color: #6b7280; font-size: 14px;'>
                                ¿Necesitas ayuda? Contáctanos en 
                                <a href='mailto:soporte@jampos.com' style='color: #3b82f6; text-decoration: none;'>
                                    soporte@jampos.com
                                </a>
                            </p>
                            <p style='margin: 0; color: #9ca3af; font-size: 12px;'>
                                © 2025 jamPOS. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetNewUserEmailTemplate(string userName, string tempPassword, string empresaName)
        {
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Nueva Cuenta - jamPOS</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f3f4f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    
                    <tr>
                        <td style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: 700;'>
                                jam<span style='font-weight: 300;'>POS</span>
                            </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px;'>
                                👋 ¡Hola! Se ha creado tu cuenta
                            </h2>
                            
                            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Un administrador de <strong>{empresaName}</strong> te ha creado una cuenta en jamPOS.
                            </p>

                            <div style='background-color: #f0fdf4; border-left: 4px solid #10b981; padding: 20px; margin: 20px 0; border-radius: 4px;'>
                                <p style='margin: 0 0 10px 0; color: #1f2937; font-weight: 600;'>
                                    🔑 Tus credenciales de acceso:
                                </p>
                                <p style='margin: 5px 0; color: #4b5563;'>
                                    <strong>Usuario:</strong> {userName}
                                </p>
                                <p style='margin: 5px 0; color: #4b5563;'>
                                    <strong>Contraseña temporal:</strong> <code style='background: #e5e7eb; padding: 4px 8px; border-radius: 4px;'>{tempPassword}</code>
                                </p>
                            </div>

                            <div style='background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                                <p style='margin: 0; color: #991b1b; font-size: 14px;'>
                                    ⚠️ <strong>Importante:</strong> Por seguridad, cambia tu contraseña al iniciar sesión por primera vez.
                                </p>
                            </div>

                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='https://localhost:44425/auth/login' 
                                   style='display: inline-block; background: linear-gradient(135deg, #10b981 0%, #059669 100%); 
                                          color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; 
                                          font-weight: 600; font-size: 16px;'>
                                    Iniciar Sesión
                                </a>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style='background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='margin: 0; color: #9ca3af; font-size: 12px;'>
                                © 2025 jamPOS. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetPasswordResetEmailTemplate(string userName, string resetToken)
        {
            return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Recuperar Contraseña - jamPOS</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f3f4f6;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f3f4f6; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    
                    <tr>
                        <td style='background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 32px; font-weight: 700;'>
                                jam<span style='font-weight: 300;'>POS</span>
                            </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #1f2937; margin: 0 0 20px 0; font-size: 24px;'>
                                🔐 Recuperar Contraseña
                            </h2>
                            
                            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Hola <strong>{userName}</strong>,
                            </p>

                            <p style='color: #4b5563; line-height: 1.6; margin: 0 0 20px 0; font-size: 16px;'>
                                Recibimos una solicitud para restablecer tu contraseña. Haz clic en el botón de abajo para crear una nueva contraseña:
                            </p>

                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='https://localhost:44425/auth/reset-password?token={resetToken}' 
                                   style='display: inline-block; background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); 
                                          color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; 
                                          font-weight: 600; font-size: 16px;'>
                                    Restablecer Contraseña
                                </a>
                            </div>

                            <div style='background-color: #fef2f2; border-left: 4px solid #ef4444; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                                <p style='margin: 0; color: #991b1b; font-size: 14px;'>
                                    ⚠️ Si no solicitaste este cambio, ignora este email. Tu contraseña permanecerá sin cambios.
                                </p>
                            </div>

                            <p style='color: #6b7280; font-size: 14px; margin: 20px 0 0 0;'>
                                Este enlace expirará en 24 horas por seguridad.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #e5e7eb;'>
                            <p style='margin: 0; color: #9ca3af; font-size: 12px;'>
                                © 2025 jamPOS. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        #endregion
    }
}

