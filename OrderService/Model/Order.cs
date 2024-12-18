namespace OrderService.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ProductIds { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
