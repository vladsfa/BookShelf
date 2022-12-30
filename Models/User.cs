using Microsoft.AspNetCore.Identity;

namespace BookStore_WebApplication.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
