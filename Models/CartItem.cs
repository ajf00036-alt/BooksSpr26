using System.ComponentModel.DataAnnotations.Schema;

namespace BooksSpr2026.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book book { get; set; } //navigational property (so you can navigate to the Book table and get info)

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; } //navigational property

        public int Quantity { get; set; }

        [NotMapped]
        public decimal SubTotal { get; set; }

    }

}
