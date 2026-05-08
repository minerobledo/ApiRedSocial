using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Aplication.Interfaces.Repository;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Resend;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService

    {
        private readonly IConfiguration _config;
        private readonly RegionEndpoint _region;

        public EmailService(IConfiguration config)
        {
            _config = config;
            _region = RegionEndpoint.GetBySystemName(_config["EmailSettings:Region"] ?? "sa-east-1");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
            var client = new HttpClient();
            var url = Environment.GetEnvironmentVariable("EMAIL_SENDER_API");

            var request = new EmailRequest
            {
                Email = email,
                Subject = subject,
                HtmlMessage = htmlMessage
            };

            try
            {
                var response = await client.PostAsJsonAsync(url, request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo enviado correctamente desde el endpoint.");
                }
                else
                {
                    Console.WriteLine($"Error en el endpoint: {response.StatusCode}");
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al llamar al endpoint: {ex}");
            }
        }

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string body, byte[] attachmentData, string attachmentFileName)
        {
            try
            {
                var fromAddress = Environment.GetEnvironmentVariable("SES_SENDER_EMAIL")!;
                var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID")!;
                var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")!;

                using var client = new AmazonSimpleEmailServiceClient(accessKey, secretKey, _region);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Red Selecta", fromAddress));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                builder.Attachments.Add(attachmentFileName, attachmentData);
                message.Body = builder.ToMessageBody();

                using var ms = new MemoryStream();
                message.WriteTo(ms);

                var rawRequest = new SendRawEmailRequest
                {
                    RawMessage = new RawMessage(ms)
                };

                var response = await client.SendRawEmailAsync(rawRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Error enviando email con adjunto: {response.HttpStatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar email con adjunto: {ex.Message}", ex);
            }
        }

        public async Task SendEmailWithTemplateAsync(string email, string subject, string templateFileName, object model)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", templateFileName);
            var htmlMessage = await File.ReadAllTextAsync(templatePath);

            foreach (var property in model.GetType().GetProperties())
            {
                string placeholder = $"{{{{{property.Name}}}}}";
                string value = property.GetValue(model)?.ToString() ?? string.Empty;
                htmlMessage = htmlMessage.Replace(placeholder, value);
            }

            await SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
public class EmailRequest
{
    public string Email { get; set; } = "";
    public string Subject { get; set; } = "";
    public string HtmlMessage { get; set; } = "";
}

