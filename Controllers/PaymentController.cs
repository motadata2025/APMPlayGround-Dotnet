using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ICapPublisher _capBus;

    public PaymentController(ICapPublisher capBus)
    {
        _capBus = capBus;
    }

    // Trigger payment processing by publishing to CAP
    [HttpPost("pay")]
    public async Task<IActionResult> MakePayment([FromQuery] Guid orderId)
    {
        if (orderId == Guid.Empty)
            return BadRequest("Valid orderId is required");

        // Publish payment.process message with orderId (subscriber will handle DB update)
        await _capBus.PublishAsync("payment.process", orderId);

        return Ok(new { Message = $"Payment process started for OrderId: {orderId}" });
    }
}
