using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(IHttpClientFactory httpFactory, ILogger<Worker> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpFactory.CreateClient("api");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var response = await client.GetAsync("/test", stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(stoppingToken);
                    _logger.LogInformation("Fetched {Bytes} bytes at {Time}", body?.Length ?? 0, DateTimeOffset.UtcNow);
                }
                else
                {
                    _logger.LogWarning("HTTP {StatusCode} from API at {Time}", 
                        (int)response.StatusCode, DateTimeOffset.UtcNow);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Stopping: cancellation requested.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HTTP request failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
