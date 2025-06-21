using OrderIngestionFunction.Models;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public interface IProductService
{
    Task<ProductInfo> GetProductInfo(string sku);
}