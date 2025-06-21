namespace OrderIngestionFunction.Models
{
    public class ProductInfo
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}