using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.Models
{
    public class BookOrderDto
    {
        public int BookId { get; set; }
        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Number { get; set; }
        public Order Order { get; set; }
    }
}
