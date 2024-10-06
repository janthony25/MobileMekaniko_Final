using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SQLitePCL;
using System.Runtime.CompilerServices;

namespace MobileMekaniko_Final.Services
{
    public class EmailPdfService : EmailSender
    {
        private readonly ILogger<EmailPdfService> _logger;
        public EmailPdfService(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailPdfService> logger) : base(optionsAccessor, logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; }

        // Method to send invoice with PDF attachment
        public async Task SendInvoiceEmailAsync(string toEmail, string subject, string message, byte[] pdfAttachment, string attachmentName)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("SendGridKey is not configured.");
            }

            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("mobilemekaniko.nz@gmail.com", "Mobile Mekaniko"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Add PDF attachment if available
            if (pdfAttachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                msg.AddAttachment(attachmentName, Convert.ToBase64String(pdfAttachment));
            }

            // Disable click tracking
            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation(response.IsSuccessStatusCode
                    ? $"Email to {toEmail} queued successfully!"
                    : $"Failure sending email to {toEmail}");
        }
    }
}
