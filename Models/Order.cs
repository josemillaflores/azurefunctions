namespace OrderIngestionFunction.Models
{
    public class OrderItem
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}