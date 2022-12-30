using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookStore_WebApplication.ViewModels;
using BookStore_WebApplication.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace BookStore_WebApplication.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, Year = model.Year };

                //Add an user
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Installing coocie
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _signInManager.SignInAsync(user, false);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles != null)
                        userRoles.Add("user");
                    await _userManager.AddToRolesAsync(user, userRoles);

                    var res = await _userManager.ConfirmEmailAsync(user, code);
                    var res2 = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                    if (res.Succeeded && res2.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                        return View("Error");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {                
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Невірний логін або пароль");      
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //delete autentification coocie
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
