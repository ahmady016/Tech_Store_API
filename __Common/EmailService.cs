using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace TechStoreApi.Common;

public class BaseUrl
{
    public string Server { get; set; }
    public string Client { get; set; }
}

public class MailOptions
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
public class EmailService : IEmailService
{
    private readonly MailOptions _mailOptions;
    private readonly SmtpClient _emailClient;
    private readonly ILogger<EmailService> _logger;
    private string _errorMessage;
    public EmailService(
        IOptions<MailOptions> mailOptions,
        ILogger<EmailService> logger
    )
    {
        _mailOptions = mailOptions.Value;
        _emailClient = new SmtpClient()
        {
            Host = _mailOptions.Host,
            Port = int.Parse(_mailOptions.Port),
            Credentials = new NetworkCredential(_mailOptions.Email, _mailOptions.Password),
            EnableSsl = false,
            UseDefaultCredentials = false
        };
        _logger = logger;
    }
    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MailMessage()
        {
            From = new MailAddress(_mailOptions.Email, "نوادي الأجيال"),
            Subject = subject,
            Body = body
        };
        message.To.Add(to);
        try
        {
            await _emailClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _errorMessage = $"{nameof(EmailService)} => {ex.Message}";
            _logger.LogError(_errorMessage);
            throw new HttpRequestException(_errorMessage, null, HttpStatusCode.InternalServerError);
        }
    }
}
