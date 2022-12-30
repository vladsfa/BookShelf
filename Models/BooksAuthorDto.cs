using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.Models
{
    public class BooksAuthorDto
    {
        public int BookId { get; set; }
        [Display(Name = "Книга")]
        public string Name { get; set; }
        [Display(Name = "Ціна")]
        public decimal Cost { get; set; }
        [Display(Name = "Тип / Жанр")]
        public string Type { get; set; } = null!;
        [Display(Name = "Магазин")]
        public int IdStore { get; set; }

        [Display(Name = "Має автора")]
        public bool IsAuthorBook { get; set; }

        public BooksAuthorDto(int _id, string _name,decimal _cost, string _type, int _storeId, bool _isAuthorBook)
        {
            BookId = _id;
            Name = _name;
            Cost = _cost;
            Type = _type;
            IdStore = _storeId;
            IsAuthorBook = _isAuthorBook;
        }
    }
}
