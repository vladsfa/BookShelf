using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class Client
    {
        public Client()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name ="ПІБ")]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Номер телефону")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Адреса")]
        [DataType(DataType.Text)]
        public string? Address { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
