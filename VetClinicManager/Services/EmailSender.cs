using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace VetClinicManager.Services;

public class EmailSender : IEmailSender
{
    public AuthMessageSenderOptions Options { get; }

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
    {
        Options = optionsAccessor.Value;
    }

    public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        if (string.IsNullOrEmpty(Options.SendGridKey))
        {
            throw new Exception("Klucz SendGridKey nie został skonfigurowany w user-secrets.");
        }
        
        var client = new SendGridClient(Options.SendGridKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("zomek212@gmail.com", "VetClinic Manager"),
            Subject = subject,
            HtmlContent = htmlMessage
        };
        msg.AddTo(new EmailAddress(toEmail));

        msg.SetClickTracking(false, false);

        return client.SendEmailAsync(msg);
    }
}