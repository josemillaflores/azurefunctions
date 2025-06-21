using OrderIngestionFunction.Models;

namespace OrderIngestionFunction.Services;

public class OrderValidator : IOrderValidator
{
    public List<string> Validate(Order order)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(order.OrderId))
        {
            errors.Add("OrderId is required.");
        }
        if (string.IsNullOrWhiteSpace(order.CustomerId))
        {
            errors.Add("CustomerId is required.");
        }
        if (order.Items == null || !order.Items.Any())
        {
            errors.Add("Order must contain at least one item.");
        }
        else
        {
            foreach (var item in order.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Sku)) errors.Add("Item SKU is required.");
                if (item.Quantity <= 0) errors.Add($"Quantity for SKU {item.Sku} must be greater than zero.");
            }
        }
        if (order.TotalAmount <= 0)
        {
            errors.Add("TotalAmount must be greater than zero.");
        }

        return errors;
    }
}