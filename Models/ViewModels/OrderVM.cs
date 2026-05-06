namespace BooksSpr2026.Models.ViewModels
{
    public class OrderVM
    {
        public Order order { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }

    }
}
