#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BookStore_WebApplication.Models;

namespace BookStore_WebApplication.Controllers
{
    [Authorize(Roles ="admin")]
    public class OrdersController : Controller
    {
        private readonly StoreDBContext _context;

        public OrdersController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var storeDBContext = _context.Orders.Include(o => o.IdClientNavigation).Include(o => o.IdDeliveryNavigation).Include(o => o.IdEmployeeNavigation);
            return View(await storeDBContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .Include(o => o.IdDeliveryNavigation)
                .Include(o => o.IdEmployeeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "FullName");
            ViewData["IdDelivery"] = new SelectList(_context.Deliveries, "Id", "DeliveryName");
            ViewData["BooksList"] = new SelectList(_context.Books, "Id", "Name"); 

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdClient,IdDelivery,IdEmployee,OrderDate")] Order order, BookOrderDto model)
        {
            if (ModelState.IsValid)
            {
                var emp = (from e in _context.Employees
                           from bs in _context.BookStores
                           from b in _context.Books
                           where b.IdStore == bs.Id && e.IdStore == bs.Id && b.Id == model.BookId
                           select e.Id).ToList();

                order.IdEmployee = emp.FirstOrDefault();
                _context.Add(order);
                await _context.SaveChangesAsync();

                int raw = _context.Database.ExecuteSqlRaw("INSERT INTO BookOrder(IdOrder, IdBook, Number) VALUES({0}, {1}, {2})", order.Id, model.BookId, model.Number);

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Id", order.IdClient);
            ViewData["IdDelivery"] = new SelectList(_context.Deliveries, "Id", "Id", order.IdDelivery);
            ViewData["BooksList"] = new SelectList(_context.Books, "Id", "Name");
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "FullName", order.IdClient);
            ViewData["IdDelivery"] = new SelectList(_context.Deliveries, "Id", "DeliveryName", order.IdDelivery);
            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "FullName", order.IdEmployee);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdClient,IdDelivery,IdEmployee,OrderDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Id", order.IdClient);
            ViewData["IdDelivery"] = new SelectList(_context.Deliveries, "Id", "Id", order.IdDelivery);
            ViewData["IdEmployee"] = new SelectList(_context.Employees, "Id", "Id", order.IdEmployee);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .Include(o => o.IdDeliveryNavigation)
                .Include(o => o.IdEmployeeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            int raw = _context.Database.ExecuteSqlRaw("DELETE FROM BookOrder WHERE IdOrder = {0}", order.Id);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
