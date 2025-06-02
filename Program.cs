using DotNetCore.CAP;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCap(x =>
{
    x.UseMySql(builder.Configuration.GetConnectionString("Default")
              ?? throw new InvalidOperationException("Connection string not found"));

    x.UseRabbitMQ(cfg =>
    {
        cfg.HostName = builder.Configuration["CAP:RabbitMQ:HostName"] ?? "localhost";
        cfg.UserName = builder.Configuration["CAP:RabbitMQ:UserName"] ?? "guest";
        cfg.Password = builder.Configuration["CAP:RabbitMQ:Password"] ?? "guest";
    });

    x.FailedRetryCount = 5; // More robust retry
    x.Version = "v1";       // Version your messages
});

// Register your custom services here, e.g., repositories, subscribers, etc.

builder.Services.AddTransient<OrderRepository>();
builder.Services.AddTransient<PaymentRepository>();
builder.Services.AddTransient<PaymentSubscriber>();
builder.Services.AddTransient<TraceSubscriber>();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
});

app.Run();