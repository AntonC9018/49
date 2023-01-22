using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace fourtynine;

public class SendGridEmailSenderOptions
{
    public string Key { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}

public class SendEmailException : Exception
{
    public SendEmailException(string? message) : base(message)
    {
    }
}

public interface ISendGridEmailSender
{
    Task<Response> SendEmailAsync(SendGridMessage message);
}

public class SendGridEmailSender : IEmailSender, ISendGridEmailSender, IDisposable
{
    private readonly SendGridClient _client;
    private readonly EmailAddress _senderAddress;

    public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> options)
    {
        var options1 = options.Value;
        _client = new SendGridClient(options1.Key);
        _senderAddress = new EmailAddress(options1.SenderEmail, options1.SenderName);
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var msg = new SendGridMessage
        {
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        var response = await Send(msg);
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            throw new SendEmailException(await response.Body.ReadAsStringAsync());
    }

    public async Task<Response> Send(SendGridMessage msg)
    {
        if (msg.From is not null)
            throw new ArgumentException("From address is unique in the system and should not be set");
        
        msg.From = _senderAddress;
        msg.SetClickTracking(false, false);
        msg.SetOpenTracking(false);
        msg.SetGoogleAnalytics(false);
        msg.SetSubscriptionTracking(false);
        return await _client.SendEmailAsync(msg);
    }

    Task<Response> ISendGridEmailSender.SendEmailAsync(SendGridMessage message) => Send(message);
    
    public void Dispose()
    {
        // The client has no dispose? what?
        // Client.Dispose();
    }
}
