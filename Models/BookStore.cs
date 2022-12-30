using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class BookStore
    {
        public BookStore()
        {
            Books = new HashSet<Book>();
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Назва")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Адреса")]
        [DataType(DataType.Text)]
        public string Address { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Номер телефону")]
        [DataType(DataType.PhoneNumber)]        
        public string PhoneNumber { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
