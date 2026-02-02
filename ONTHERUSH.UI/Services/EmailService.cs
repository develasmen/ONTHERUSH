using System.Net;
using System.Net.Mail;

namespace ONTHERUSH.UI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarAsync(string para, string asunto, string html)
        {
            var from = _config["EmailSettings:From"];
            var host = _config["EmailSettings:Host"];
            var user = _config["EmailSettings:User"];
            var pass = _config["EmailSettings:AppPassword"];


            int port = int.TryParse(_config["EmailSettings:Port"], out var p) ? p : 587;
            bool enableSsl = bool.TryParse(_config["EmailSettings:EnableSsl"], out var s) ? s : true;

            if (string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass))
            {
                throw new InvalidOperationException(
                    "EmailSettings incompleto en appsettings.json (From/Host/User/AppPassword).");
            }

            using var msg = new MailMessage(from, para, asunto, html);
            msg.IsBodyHtml = true;

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            await smtp.SendMailAsync(msg);
        }
    }
}
