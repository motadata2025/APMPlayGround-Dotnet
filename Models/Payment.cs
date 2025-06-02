public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}