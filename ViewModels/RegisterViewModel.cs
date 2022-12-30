using System.ComponentModel.DataAnnotations;

namespace BookStore_WebApplication.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress, ErrorMessage ="E-mail невірний.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail невірний.")]
        public string Email { get; set; }

        [Required]
        [Range(1900,2022, ErrorMessage ="Введіть дійсний рік народження.")]
        [Display(Name ="Рік народження")]
        public int Year { get; set; }

        [Required]
        [Display(Name ="Пароль")]
        [StringLength(100, ErrorMessage = "Поле {0} повинне мати мінімум {2} і максимум {1} символів.", MinimumLength = 8)]
        [RegularExpression(pattern: "^[0-9]+$", ErrorMessage ="Поле Пароль повинне складатись лише з цифр.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage ="Паролі не співпадають")]
        [Display(Name ="Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        public string ReturnUrl { get; set; }
    }
}
