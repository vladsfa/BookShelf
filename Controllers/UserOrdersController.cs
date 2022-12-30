#nullable disable
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookStore_WebApplication.Models;
using BookStore_WebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BookStore_WebApplication.Controllers
{
    public class UserOrdersController : Controller
    {
        private readonly StoreDBContext _context;

        public UserOrdersController(StoreDBContext context)
        {
            _context = context;
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewData["Books"] = new SelectList(_context.Books, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var client = (from c in _context.Clients
                              where c.FullName == model.ClientName && c.Address == model.ClientAddress && c.PhoneNumber == model.ClientPhoneNumber
                              select c.Id).ToList();

                if (client.Count() == 0)
                {
                    Client newClient = new Client { FullName = model.ClientName, Address = model.ClientAddress, PhoneNumber = model.ClientPhoneNumber };
                    int raw = _context.Database.ExecuteSqlRaw("INSERT INTO Client(FullName, PhoneNumber, Address) VALUES({0}, {1}, {2})", newClient.FullName, newClient.PhoneNumber, newClient.Address);
                }

                var book = (from b in _context.Books
                            where b.Id == model.MyBook
                            select b).ToList();

                var emp = (from e in _context.Employees
                          where e.IdStore == book.FirstOrDefault().IdStore
                          select e.Id).ToList();

                Order newOrder = new Order { IdClient = client.FirstOrDefault(), IdDelivery = _context.Deliveries.FirstOrDefault().Id, IdEmployee = emp.FirstOrDefault(), OrderDate = DateTime.UtcNow };
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                if (book.Count() != 0)
                {
                    foreach (var item in book)
                    {   
                        int raw = _context.Database.ExecuteSqlRaw("INSERT INTO BookOrder(IdOrder, IdBook, Number) VALUES({0}, {1}, {2})", newOrder.Id, item.Id, model.NumberOfBooks);
                    }
                }                 

                return RedirectToAction("Success");
            }
 
            return View();
        }
    }
}
