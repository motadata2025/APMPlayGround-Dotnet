using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _orderRepo;
    private readonly ICapPublisher _capBus;

    public OrderController(OrderRepository orderRepo, ICapPublisher capBus)
    {
        _orderRepo = orderRepo;
        _capBus = capBus;
    }

    // Create an order and publish event
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        order.Id = Guid.NewGuid();
        order.OrderDate = DateTime.UtcNow;
        order.Status = "Pending";

        await _orderRepo.AddOrderAsync(order);

        // Publish order created event (optional, depends on your workflow)
        await _capBus.PublishAsync("order.created", order);

        return Ok(order);
    }

    // Get all orders with payment details if available
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderRepo.GetAllOrdersWithPaymentsAsync();
        return Ok(orders);
    }

    // Get single order with payment info
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _orderRepo.GetOrderWithPaymentAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateOrders([FromQuery] int count = 1)
    {
        if (count <= 0)
            return BadRequest("Count must be greater than zero.");

        var rnd = new Random();

        for (int i = 0; i < count; i++)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                CustomerName = $"Customer-{i + 1}",
                TotalAmount = rnd.Next(10, 500),
                Status = "Pending"
            };

            await _orderRepo.AddOrderAsync(order);
            await _capBus.PublishAsync("payment.process", order.Id);
        }

        return Ok(new { Message = $"Successfully created and published {count} orders." });
    }
}
