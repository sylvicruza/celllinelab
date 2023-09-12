using System.Net.Mail;
using System.Net;
using System.Web.Helpers;

namespace Cell_line_laboratory.Utils
{
    public static class EmailSender
    {

        public const string _smtpServer = "smtp.office365.com";
        public const int _smtpPort = 587;
        public const string _smtpUsername = "s.feedback@outlook.com";
        public const string _smtpPassword = "Bhoyee_01";

       

        public static async Task SendEmailAsync(string to, string subject, string body)
        {
            var fromAddress = new MailAddress(_smtpUsername, "CellLine Notification");
            var toAddress = new MailAddress(to);
            var smtp = new SmtpClient
            {
                Host = _smtpServer,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _smtpPassword)
            };
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            await smtp.SendMailAsync(message);
        }


        // Usage Create an email account for FeedBackSystem.
       /* var emailBody = $"Your new password is: {newPassword}";
        await EmailSender.SendEmailAsync(email, "Password Reset", emailBody);*/
    }
}
