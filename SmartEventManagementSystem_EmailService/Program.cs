using MassTransit;
using SmartEventManagementSystem_EmailService;
using SmartEventManagementSystem_EmailService.Models;
using DotNetEnv;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Log to console
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)  // Optional: Log to a file
    .CreateLogger();

Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMassTransit(x =>
{
    var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

    x.AddConsumer<AttendeeEncodedConsumer>();

    x.UsingRabbitMq((context, config) =>
    {
        var host = rabbitMqConfig.GetValue<string>("HostName");
        var username = rabbitMqConfig.GetValue<string>("Username");
        var password = rabbitMqConfig.GetValue<string>("Password");

        config.Host(host, "/", h =>
        {
            h.Username(username!);
            h.Password(password!);
        });

        config.ReceiveEndpoint("attendee_email_queue", e =>
        {
            e.Durable = true;
            e.AutoDelete = false;
            e.Exclusive = false;
            e.ConfigureConsumeTopology = true;
            e.Bind("attendee_email_exchange", x =>
            {
                x.RoutingKey = "attendee_email";
                x.ExchangeType = "direct";
            });
            
            e.ConfigureConsumer<AttendeeEncodedConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

try
{
    Log.Information("Application Starting...");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
