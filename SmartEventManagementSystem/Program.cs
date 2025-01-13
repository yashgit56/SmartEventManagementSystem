using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client ;
using Smart_Event_Management_System.BackgroundService;
using Smart_Event_Management_System.Context;
using Smart_Event_Management_System.CustomLogic;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;
using Smart_Event_Management_System.Service;
using Smart_Event_Management_System.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true
            };
        }
    );

builder.Services.AddSingleton<IConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var rabbitMqConfig = configuration.GetSection("RabbitMQ");

    var factory = new ConnectionFactory
    {
        HostName = rabbitMqConfig["HostName"]!,
        Port = int.Parse(rabbitMqConfig["Port"]!),
        UserName = rabbitMqConfig["Username"]!,
        Password = rabbitMqConfig["Password"]!
    };

    return factory.CreateConnectionAsync().Result;
});

// cors configuration 
// builder.Services.AddCors(options => 
//     options.AddPolicy("AllowAllOrigins",
//         policy =>
//         {
//             policy.WithOrigins("http://localhost:5665")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod();
//         })
//     );

builder.Services.AddAuthorization();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IAttendeeService, AttendeeService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ICheckInLogService, CheckInLogService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<CustomLogicService>();
builder.Services.AddSingleton<RabbitMQEmailService>();
builder.Services.AddHostedService<RabbitMqEmailHostedService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();
builder.Services.AddScoped<ICheckInLogRepository, CheckInLogRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<IValidator<Admin>, AdminValidator>();
builder.Services.AddScoped<IValidator<Attendee>, AttendeeValidator>();
builder.Services.AddScoped<IValidator<CheckInLog>, CheckInLogValidator>();
builder.Services.AddScoped<IValidator<Event>, EventValidator>();
builder.Services.AddScoped<IValidator<Ticket>, TicketValidator>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new ArgumentException("Connection string can not be null or contain whitespace");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 40)))
);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            context.Response.Redirect("/Home/Error?statusCode=" + context.Response.StatusCode);
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();