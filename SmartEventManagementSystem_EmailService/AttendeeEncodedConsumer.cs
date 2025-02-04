using System.Text;
using System.Text.Json;
using MailKit.Net.Smtp;
using MassTransit;
using MessageContracts;
using MimeKit;
using Serilog;
using SmartEventManagementSystem_EmailService.Models;

namespace SmartEventManagementSystem_EmailService;
public class AttendeeEncodedConsumer : IConsumer<AttendeeEmailMessage>
{
    private readonly IConfiguration _configuration;

    public AttendeeEncodedConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<AttendeeEmailMessage> context)
    {
        try
        {
            var body = Encoding.UTF8.GetString(context.Message.Message);
            var attendee = JsonSerializer.Deserialize<Attendee>(body);

            if (attendee == null || string.IsNullOrEmpty(attendee.Email))
            {
                Log.Warning("Invalid attendee data. Ignoring message.");
                return;
            }
            
            await SendWelcomeEmailAsync(attendee);
            Log.Information($"Message acknowledged: {attendee.Email}");

        }
        catch (JsonException ex)
        {
            Log.Error($"Error deserializing message: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error processing message: {ex.Message}");
        }
    }
    
    private async Task SendWelcomeEmailAsync(Attendee attendee)
    {
        try
        {
            var emailSettingsSection = _configuration.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("SmartEventManagementSystem", emailSettingsSection["FromEmail"]));
            email.To.Add(new MailboxAddress("", attendee.Email));

            string subject = "Welcome to SmartEventManagementSystem";
            string body = $@"
                <h1>Welcome, {attendee.Username}!</h1>
                <p>Thank you for registering with <strong>SmartEventManagementSystem</strong>.</p>
                <p>Here are your registration details:</p>
                <ul>
                    <li><strong>Username:</strong> {attendee.Username}</li>
                    <li><strong>Email:</strong> {attendee.Email}</li>
                    <li><strong>Phone Number:</strong> {attendee.PhoneNumber}</li>
                </ul>
                <p>You registered on: {DateTime.Now:f}</p>
                <p>If you have any questions, feel free to contact our support team.</p>
                <p>Best regards,<br>SmartEventManagementSystem Team</p>
            ";

            email.Subject = subject;
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(
                emailSettingsSection["Host"],
                int.Parse(emailSettingsSection["Port"]!),
                MailKit.Security.SecureSocketOptions.StartTls
            );
            await smtpClient.AuthenticateAsync(
                emailSettingsSection["Username"],
                Environment.GetEnvironmentVariable("EMAIL_PASSWORD") 
            );
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);

            Log.Information($"Email sent successfully to {attendee.Email}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error sending email to {attendee.Email}: {ex.Message}");
            throw; 
        }
    }

}
