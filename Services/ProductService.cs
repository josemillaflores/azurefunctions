using OrderIngestionFunction.Models;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public class ProductService : IProductService
{
    public async Task<ProductInfo> GetProductInfo(string sku)
    {
        // Simulate a call to a product service or database
        await Task.Delay(50); // Simulate network latency

        switch (sku.ToLower())
        {
            case "sku001":
                return new ProductInfo { Sku = "SKU001", Price = 10.50m, IsAvailable = true };
            case "sku002":
                return new ProductInfo { Sku = "SKU002", Price = 25.00m, IsAvailable = true };
            case "sku003":
                return new ProductInfo { Sku = "SKU003", Price = 5.75m, IsAvailable = false }; // Not available
            default:
                return null; // Product not found
        }
    }
}