using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.WindowsServices;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ApiClientOptions>(
    builder.Configuration.GetSection("ApiClient"));

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "OtelDotnetTestWorkerService";
});

// Named HttpClient (point to the API you intend to call)
builder.Services.AddHttpClient("api", (sp, client) =>
{
    var opts = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<ApiClientOptions>>().Value;

    client.BaseAddress = new Uri(opts.BaseAddress);
    client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
});

// Register the worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();
app.Run();