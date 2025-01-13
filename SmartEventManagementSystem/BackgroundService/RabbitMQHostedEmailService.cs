namespace Smart_Event_Management_System.BackgroundService;
using Microsoft.Extensions.Hosting;

public class RabbitMqEmailHostedService : BackgroundService
{
    private readonly RabbitMQEmailService _rabbitMQEmailService;

    public RabbitMqEmailHostedService(RabbitMQEmailService rabbitMQEmailService)
    {
        _rabbitMQEmailService = rabbitMQEmailService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQEmailService.StartListening();
        return Task.CompletedTask;
    }
}
