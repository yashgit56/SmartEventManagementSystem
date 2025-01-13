using System.Text;
using System.Text.Json;
using MimeKit;
using MailKit.Net.Smtp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smart_Event_Management_System.Models;
public class RabbitMQEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IChannel _channel;

    public RabbitMQEmailService(IConfiguration configuration, IConnection connection)
    {
        _configuration = configuration;

        // Create RabbitMQ channel and declare queue
        _channel = connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(
            queue: "EmailQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        Console.WriteLine("RabbitMQ connection and channel established successfully.");
    }

    public void StartListening()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message received: {message}");

            try
            {
                var attendee = JsonSerializer.Deserialize<Attendee>(message);

                if (attendee == null || string.IsNullOrEmpty(attendee.Email))
                {
                    Console.WriteLine("Invalid attendee data. Ignoring message.");
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                // Process email sending
                await SendWelcomeEmailAsync(attendee);
                await _channel.BasicAckAsync(ea.DeliveryTag, false); // Acknowledge message
                Console.WriteLine($"Message acknowledged: {attendee.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true); // Requeue the message
            }
        };

        _channel.BasicConsumeAsync(
            queue: "EmailQueue",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("RabbitMQ consumer initialized and listening to EmailQueue.");
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
                emailSettingsSection["Password"]
            );
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);

            Console.WriteLine($"Email sent successfully to {attendee.Email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email to {attendee.Email}: {ex.Message}");
            throw; // Re-throw exception to trigger Nack
        }
    }
}
