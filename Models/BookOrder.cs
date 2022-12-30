using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication
{
    public partial class BookOrder
    {
        public int IdOrder { get; set; }
        public int IdBook { get; set; }
        [Display(Name ="Кількість")]
        public int Number { get; set; }

        [Display(Name ="Книга")]
        public virtual Book IdBookNavigation { get; set; } = null!;

        [Display(Name ="Номер замовлення")]
        public virtual Order IdOrderNavigation { get; set; } = null!;
    }
}
