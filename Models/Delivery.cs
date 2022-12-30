using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class Delivery
    {
        public Delivery()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Назва")]
        [DataType(DataType.Text)]
        public string DeliveryName { get; set; } = null!;



        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Дата доставки замовлення")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }



        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Ціна")]
        [DataType(DataType.Currency)]
        public decimal FullCost { get; set; }



        [Display(Name = "Номер телефону")]
        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
