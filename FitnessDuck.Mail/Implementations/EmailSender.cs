using System.Net.Mail;
using FitnessDuck.Exceptions;
using FitnessDuck.Mail.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FitnessDuck.Mail.Implementations;

public class EmailSender: IEmailSender
{
    private readonly IConfiguration _config;
    private readonly MailAddress _sender;
    private readonly string _host;
    private readonly int _port;
    private readonly string? _user;
    private readonly string? _password;
    private readonly SmtpClient _smtpClient;

    public EmailSender(IConfiguration config)
    {
        _config = config;
        
        var smtpSettings = _config.GetSection("Smtp");
        
        _host=smtpSettings["Host"] ?? throw new FitnessDuckServerException("config_missing_smtp_host","Smtp Host is required");
        _port = Convert.ToInt32(smtpSettings["Port"]);
        _user = smtpSettings["Username"];
        _password = smtpSettings["Password"];
        _sender = new MailAddress(smtpSettings["Sender"], smtpSettings["SenderName"]);
        _smtpClient = new SmtpClient(_host,_port);
        _smtpClient.EnableSsl = true;
        // set smtp-client with basicAuthentication
        _smtpClient.UseDefaultCredentials = false;
        System.Net.NetworkCredential basicAuthenticationInfo = new
            System.Net.NetworkCredential(_user, _password);
        _smtpClient.Credentials = basicAuthenticationInfo;

    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        MailAddress to = new MailAddress(email);
        MailMessage msgMail = new System.Net.Mail.MailMessage(_sender, to);

        // set subject and encoding
        msgMail.Subject = subject;
        msgMail.SubjectEncoding = System.Text.Encoding.UTF8;

        // set body-message and encoding
        msgMail.Body = message;
        msgMail.BodyEncoding = System.Text.Encoding.UTF8;
        // text or html
        msgMail.IsBodyHtml = true;

        _smtpClient.SendAsync(msgMail, null);
    }

   
}