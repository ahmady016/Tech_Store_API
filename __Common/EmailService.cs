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
    public EmailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
    }
    public async Task SendAsync(string to, string subject, string body)
    {
        var emailClient = new SmtpClient(_mailOptions.Host, int.Parse(_mailOptions.Port));
        emailClient.Credentials = new NetworkCredential(_mailOptions.Email, _mailOptions.Password);
        await emailClient.SendMailAsync(new MailMessage(_mailOptions.Email, to, subject, body));
    }
}
