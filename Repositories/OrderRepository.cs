using Dapper;
using MySql.Data.MySqlClient;

public class OrderRepository
{
    private readonly string _connString;

    public OrderRepository(IConfiguration config)
    {
        _connString = config.GetConnectionString("Default")!;
    }

    public async Task AddOrderAsync(Order order)
    {
        const string sql = @"INSERT INTO Orders (Id, OrderDate, CustomerName, TotalAmount, Status) 
                             VALUES (@Id, @OrderDate, @CustomerName, @TotalAmount, @Status)";
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(sql, order);
    }

    public async Task<Order?> GetOrderWithPaymentAsync(Guid orderId)
    {
        const string sql = @"
            SELECT o.*, p.Id AS PaymentId, p.PaymentStatus
            FROM Orders o
            LEFT JOIN Payments p ON o.Id = p.OrderId
            WHERE o.Id = @OrderId";

        using var conn = new MySqlConnection(_connString);
        var result = await conn.QueryAsync<Order, Payment, (Order, Payment)>(
            sql,
            (order, payment) => (order, payment),
            new { OrderId = orderId },
            splitOn: "PaymentId"
        );

        var tuple = result.FirstOrDefault();
        if (tuple == default) return null;

        var (orderResult, paymentResult) = tuple;
        if (paymentResult != null)
        {
            orderResult.Payment = paymentResult;
        }

        return orderResult;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersWithPaymentsAsync()
    {
        const string sql = @"
            SELECT o.*, p.Id AS PaymentId, p.PaymentStatus
            FROM Orders o
            LEFT JOIN Payments p ON o.Id = p.OrderId";

        using var conn = new MySqlConnection(_connString);
        var lookup = new Dictionary<Guid, Order>();

        var list = await conn.QueryAsync<Order, Payment, Order>(
            sql,
            (order, payment) =>
            {
                if (!lookup.TryGetValue(order.Id, out var o))
                {
                    o = order;
                    lookup.Add(o.Id, o);
                }
                if (payment != null)
                {
                    o.Payment = payment;
                }
                return o;
            },
            splitOn: "PaymentId"
        );

        return lookup.Values;
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, string status)
    {
        const string sql = @"UPDATE Orders SET Status = @Status WHERE Id = @OrderId";
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(sql, new { Status = status, OrderId = orderId });
    }

}
