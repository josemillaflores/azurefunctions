using OrderIngestionFunction.Models;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public interface ICustomerService
{
    Task<CustomerInfo> GetCustomerInfo(string customerId);
}