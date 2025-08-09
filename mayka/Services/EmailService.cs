using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mayka.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Mymayka.kz", "mymaykakz@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", "romkashuriken@mail.ru"));
            emailMessage.Subject = "Новый заказ";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("mymaykakz@gmail.com", "123ASDasd");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        //Отправка клиенту
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Mymayka.kz", "mymaykakz@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("mymaykakz@gmail.com", "123ASDasd");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
