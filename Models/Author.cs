using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class Author
    {
        public Author()
        {
            IdBooks = new HashSet<Book>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage ="Поле не повинне бути порожнім")]
        [Display(Name="ПІБ")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;
        [Display(Name="Книги")]
        public virtual ICollection<Book> IdBooks { get; set; }
    }
}
