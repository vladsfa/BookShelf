using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookStore_WebApplication.Models
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (user.Email.ToLower().EndsWith("@spam.com"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Даний домен знаходиться в спам-базі. Оберіть інший поштовий сервіс."
                });
            }

            if (user.Email.ToLower().EndsWith("@.ru"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Це москалівський домен. Обери інший якщо УКРАЇНЕЦЬ!."
                });
            }

            if (user.UserName.Contains("admin"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Нік не повинен мати слово 'admin'."
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
