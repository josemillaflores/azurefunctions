using OrderIngestionFunction.Models;

namespace OrderIngestionFunction.Services;

public interface IOrderValidator
{
    List<string> Validate(Order order);
}