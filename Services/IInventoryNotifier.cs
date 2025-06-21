using OrderIngestionFunction.Models;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public interface IInventoryNotifier
{
    Task<bool> NotifyForStockUpdateAsync(Order order);
}