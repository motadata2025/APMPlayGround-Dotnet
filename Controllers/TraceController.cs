using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TraceController : ControllerBase
{
    private readonly ICapPublisher _capBus;

    public TraceController(ICapPublisher capBus)
    {
        _capBus = capBus;
    }

    [HttpPost("generate-spans")]
    public async Task<IActionResult> GenerateSpans([FromQuery] int count = 100)
    {
        for (int i = 0; i < count; i++)
        {
            var message = new { Index = i, Timestamp = DateTime.UtcNow };
            await _capBus.PublishAsync("trace.sample", message);
        }

        return Ok(new { Message = $"Published {count} trace messages" });
    }
}
