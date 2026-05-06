using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BooksSpr2026.Models.ViewModels
{
    public class ShoppingCartVM
    {

        [ValidateNever]
        public IEnumerable<CartItem> CartItems { get; set; }

        public Order Order { get; set; }

    }
}
