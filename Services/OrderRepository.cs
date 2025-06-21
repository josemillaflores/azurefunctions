using Microsoft.Extensions.Logging;
using OrderIngestionFunction.Models;
using System;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public class OrderRepository : IOrderRepository
{
    private readonly ILogger<OrderRepository> _logger;
    private static readonly Random _random = new();

    public OrderRepository(ILogger<OrderRepository> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SaveOrderAsync(Order order)
    {
       
        _logger.LogInformation("Simulating database save for OrderId: {OrderId}", order.OrderId);
        await Task.Delay(100); // Simulate database operation time

        // Simulate a database failure 
        if (_random.Next(1, 11) == 1)
        {
            _logger.LogError("Simulated database failure for OrderId: {OrderId}", order.OrderId);
            return false;
        }

        return true;
    }
}