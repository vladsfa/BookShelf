using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_WebApplication.Models;
using BookStore_WebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_WebApplication.Controllers
{
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public IActionResult UserList() => View(_userManager.Users.ToList());

        public async Task<IActionResult> Edit(string userId)
        {
            //Get User
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //List User`s Role
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            //Get User
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //Roles List
                var userRoles = await _userManager.GetRolesAsync(user);
                //Get role
                var allRoles = _roleManager.Roles.ToList();
                //Added Roles List
                var addedRoles = roles.Except(userRoles);
                //Removed Roles List
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }        
    }
}
