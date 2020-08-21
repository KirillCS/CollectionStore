using MailKit.Net.Smtp;
using MimeKit;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class EmailService
    {
        private IConfiguration configuration;

        public EmailService(IConfiguration appConfiguration)
        {
            configuration = appConfiguration;
        }

        public async Task SendEmailAsync(string subject, string message, string emailAddress)
        {
            var emailMessage = GetMessage(subject, message, emailAddress);
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(configuration["Host"], 25, false);
                await client.AuthenticateAsync(configuration["EmailAddress:Address"], configuration["EmailAddress:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        private MimeMessage GetMessage(string subject, string message, string emailAddress)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("CollectionStore", configuration["EmailAddress:Address"]));
            emailMessage.To.Add(new MailboxAddress(string.Empty, emailAddress));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            return emailMessage;
        }
    }
}
