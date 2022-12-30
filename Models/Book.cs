using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class Book
    {
        public Book()
        {
            BookOrders = new HashSet<BookOrder>();
            IdAuthors = new HashSet<Author>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name="Назва")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Ціна")]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Тип / Жанр")]
        [DataType(DataType.Text)]
        public string Type { get; set; } = null!;


        [Display(Name = "Магазин")]
        public int IdStore { get; set; }


        [Display(Name="Магазин")]
        public virtual BookStore IdStoreNavigation { get; set; } = null!;


        public virtual ICollection<BookOrder> BookOrders { get; set; }
        [Display(Name ="Автори")]
        public virtual ICollection<Author> IdAuthors { get; set; }
    }
}
