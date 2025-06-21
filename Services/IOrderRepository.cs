using OrderIngestionFunction.Models;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public interface IOrderRepository
{
    Task<bool> SaveOrderAsync(Order order);
}