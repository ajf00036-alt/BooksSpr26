using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BooksSpr2026.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [DisplayName("Category Name: "), Required(ErrorMessage = "Category Name MUST be provided")]
        public string Name { get; set; }

        [DisplayName("Category Description: "), Required(ErrorMessage = "Category Description MUST be provided"), MaxLength(100, ErrorMessage = "The description cannot be more than 100 characters")]
        public string Description { get; set; }
    }

}
