using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Common;

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
    public EmailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
        _emailClient = new SmtpClient()
        {
            Host = _mailOptions.Host,
            Port = int.Parse(_mailOptions.Port),
            Credentials = new NetworkCredential(_mailOptions.Email, _mailOptions.Password)
        };
    }
    public async Task SendAsync(string to, string subject, string body)
    {
        await _emailClient.SendMailAsync(new MailMessage(_mailOptions.Email, to, subject, body));
    }
}
