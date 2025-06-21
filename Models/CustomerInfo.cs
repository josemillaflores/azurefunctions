namespace OrderIngestionFunction.Models
{
    public class CustomerInfo
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string MembershipLevel { get; set; }
        public List<string> PurchaseHistory { get; set; } // Simplified: List of OrderIds
    }
}