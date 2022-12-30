using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class Employee
    {
        public Employee()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name ="Магазин")]
        public int IdStore { get; set; }


        [Display(Name = "ПІБ")]
        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = null!;


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Заробітня плата")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }


        [Display(Name ="Магазин")]
        public virtual BookStore IdStoreNavigation { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
