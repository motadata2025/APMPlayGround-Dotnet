using Dapper;
using MySql.Data.MySqlClient;

public class PaymentRepository
{
    private readonly string _connString;

    public PaymentRepository(IConfiguration config)
    {
        _connString = config.GetConnectionString("Default")!;
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        const string sql = @"INSERT INTO Payments (Id, OrderId, PaymentStatus) 
                             VALUES (@Id, @OrderId, @PaymentStatus)";
        using var conn = new MySqlConnection(_connString);
        await conn.ExecuteAsync(sql, payment);
    }
}
