using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using BookStore_WebApplication.Models;
using BookStore_WebApplication.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.ViewModels
{
    public class UserOrderViewModel
    {

        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name ="ПІБ")]
        [DataType(DataType.Text)]
        public string ClientName { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Номер телефону")]
        [DataType(DataType.PhoneNumber)]
        public string ClientPhoneNumber { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Display(Name = "Адрес")]
        [DataType(DataType.Text)]
        public string ClientAddress { get; set; }



        [Display(Name = "Магазин")]
        public int IdStore { get; set; }
        [Display(Name = "Менеджер")]
        public string EmployeesName { get; set; }
        [Display(Name = "Доставка")]
        public int DeliveryName { get; set; }



        public int MyBook { get; set; }
        [Display(Name = "Книги")]
        public IList<Book> BookList { get; set; }


        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        [Range(1,1000)]
        [Display(Name = "Кількість книг")]
        [DataType(DataType.Currency)]
        public int NumberOfBooks { get; set; }


        public int UserId { get; set; }
        public int ClientId { get; set; }


        public Client Client { get; set; }
        public BookStore BookStore { get; set; }
        public Employee Employee { get; set; }
        public Delivery Delivery { get; set; }
        public Book Book { get; set; }
    }
}
