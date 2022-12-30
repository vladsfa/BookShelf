using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.Models
{
    public class AuthorsBookDto
    {
        public int AuthorId { get; set; }
        [Display(Name ="Автор")]
        public string Name { get; set; }
        [Display(Name = "Є автором книги")]
        public bool IsBookAuthor { get; set; }

        public AuthorsBookDto(int _id, string _name, bool _isBookAuthor)
        {
            Name = _name;
            AuthorId = _id;
            IsBookAuthor = _isBookAuthor;
        }
        
    }
}
