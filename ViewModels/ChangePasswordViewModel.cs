using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }

        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        public string Email { get; set; }


        [Display(Name ="Старий пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        public string NewPassword { get; set; }


        [Display(Name ="Новий пароль")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Поле не повинне бути порожнім")]
        public string OldPasword { get; set; }
    }
}
