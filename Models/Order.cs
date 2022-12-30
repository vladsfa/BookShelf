using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BookStore_WebApplication
{
    public partial class Order
    {
        public Order()
        {
            BookOrders = new HashSet<BookOrder>();
        }

        public int Id { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Клієнт")]
        public int IdClient { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Доставка")]
        public int IdDelivery { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Менеджер")]
        public int IdEmployee { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name ="Дата створення замовлення")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }


        [Display(Name ="Клієнт")]
        public virtual Client IdClientNavigation { get; set; } = null!;
        [Display(Name = "Доставка")]
        public virtual Delivery IdDeliveryNavigation { get; set; } = null!;
        [Display(Name = "Менеджер")]
        public virtual Employee IdEmployeeNavigation { get; set; } = null!;
        public virtual ICollection<BookOrder> BookOrders { get; set; }
    }
}
