namespace jam_POS.Infrastructure.Services
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(string toEmail, string empresaName, string adminName, string username);
        Task<bool> SendNewUserEmailAsync(string toEmail, string userName, string tempPassword, string empresaName);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}

