using DotNetCore.CAP;

public class PaymentSubscriber : ICapSubscribe
{
    private readonly PaymentRepository _paymentRepo;
    private readonly OrderRepository _orderRepo;

    public PaymentSubscriber(PaymentRepository paymentRepo, OrderRepository orderRepo)
    {
        _paymentRepo = paymentRepo;
        _orderRepo = orderRepo;
    }

    [CapSubscribe("payment.process")]
    public async Task HandlePaymentAsync(Guid orderId)
    {
        // Simulate processing delay
        await Task.Delay(500);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            PaymentStatus = "Completed"
        };

        await _paymentRepo.AddPaymentAsync(payment);

        var order = await _orderRepo.GetOrderWithPaymentAsync(orderId);
        if (order != null)
        {
            order.Status = "Paid";
            // You can create an UpdateOrderStatus method in OrderRepository:
            await _orderRepo.UpdateOrderStatusAsync(order.Id, "Paid");
        }
    }
}
