using Microsoft.Extensions.Logging;
using OrderIngestionFunction.Models;
using System;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public class InventoryNotifier : IInventoryNotifier
{
    private readonly ILogger<InventoryNotifier> _logger;
    private static readonly Random _random = new();

    public InventoryNotifier(ILogger<InventoryNotifier> logger)
    {
        _logger = logger;
    }

    public async Task<bool> NotifyForStockUpdateAsync(Order order)
    {
       
        _logger.LogInformation("Simulating inventory notification for OrderId: {OrderId}", order.OrderId);
        await Task.Delay(70);  
      
        if (_random.Next(1, 21) == 1)
        {
            _logger.LogError("Simulated inventory notification failure for OrderId: {OrderId}", order.OrderId);
            return false;
        }

        _logger.LogInformation("Inventory successfully notified for OrderId: {OrderId}.", order.OrderId);
        return true;
    }
}