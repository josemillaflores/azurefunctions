using OrderIngestionFunction.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderIngestionFunction.Services;

public class CustomerService : ICustomerService
{
    public async Task<CustomerInfo> GetCustomerInfo(string customerId)
    {
        // Simulate a call to a customer service or database
        await Task.Delay(50);  

        switch (customerId.ToLower())
        {
            case "cust123":
                return new CustomerInfo
                {
                    CustomerId = "CUST123",
                    Name = "Alice Wonderland",
                    MembershipLevel = "Gold",
                    PurchaseHistory = new List<string> { "ORD001", "ORD005" }
                };
            case "cust456":
                return new CustomerInfo
                {
                    CustomerId = "CUST456",
                    Name = "Bob The Builder",
                    MembershipLevel = "Silver",
                    PurchaseHistory = new List<string> { "ORD002" }
                };
            default:
                return null; // Customer not found
        }
    }
}