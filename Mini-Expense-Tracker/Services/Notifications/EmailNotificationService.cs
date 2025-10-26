using Mini_Expense_Tracker.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Mini_Expense_Tracker.Services.Notifications
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IConfiguration _config;
        public EmailNotificationService(IConfiguration config)
        {
            _config = config;
        }
        public async Task NotifyAsync(string message)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];
            var from = _config["Smtp:From"];
            var to = _config["Smtp:To"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            var mail = new MailMessage(from!, to!)
            {
                Subject = "Expense Tracker Notification",
                Body = message,
                IsBodyHtml = false
            };

            await client.SendMailAsync(mail);
        }
    }
}
