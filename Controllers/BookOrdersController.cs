using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore_WebApplication;
using Microsoft.AspNetCore.Authorization;

namespace BookStore_WebApplication.Controllers
{
    [Authorize(Roles ="admin")]
    public class BookOrdersController : Controller
    {
        private readonly StoreDBContext _context;

        public BookOrdersController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: BookOrders
        public async Task<IActionResult> Index()
        {
            var storeDBContext = _context.BookOrders.Include(b => b.IdBookNavigation).Include(b => b.IdOrderNavigation);
            return View(await storeDBContext.ToListAsync());
        }

        // GET: BookOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookOrder = await _context.BookOrders
                .Include(b => b.IdBookNavigation)
                .Include(b => b.IdOrderNavigation)
                .FirstOrDefaultAsync(m => m.IdOrder == id);
            if (bookOrder == null)
            {
                return NotFound();
            }

            return View(bookOrder);
        }

        // GET: BookOrders/Create
        public IActionResult Create()
        {
            ViewData["IdBook"] = new SelectList(_context.Books, "Id", "Name");
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id");
            return View();
        }

        // POST: BookOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdOrder,IdBook,Number")] BookOrder bookOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdBook"] = new SelectList(_context.Books, "Id", "Id", bookOrder.IdBook);
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", bookOrder.IdOrder);
            return View(bookOrder);
        }

        // GET: BookOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookOrder = await _context.BookOrders.FindAsync(id);
            if (bookOrder == null)
            {
                return NotFound();
            }
            ViewData["IdBook"] = new SelectList(_context.Books, "Id", "Id", bookOrder.IdBook);
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", bookOrder.IdOrder);
            return View(bookOrder);
        }

        // POST: BookOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOrder,IdBook,Number")] BookOrder bookOrder)
        {
            if (id != bookOrder.IdOrder)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookOrderExists(bookOrder.IdOrder))
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
            ViewData["IdBook"] = new SelectList(_context.Books, "Id", "Id", bookOrder.IdBook);
            ViewData["IdOrder"] = new SelectList(_context.Orders, "Id", "Id", bookOrder.IdOrder);
            return View(bookOrder);
        }

        // GET: BookOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookOrder = await _context.BookOrders
                .Include(b => b.IdBookNavigation)
                .Include(b => b.IdOrderNavigation)
                .FirstOrDefaultAsync(m => m.IdOrder == id);
            if (bookOrder == null)
            {
                return NotFound();
            }

            return View(bookOrder);
        }

        // POST: BookOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookOrder = await _context.BookOrders.FindAsync(id);
            _context.BookOrders.Remove(bookOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookOrderExists(int id)
        {
            return _context.BookOrders.Any(e => e.IdOrder == id);
        }
    }
}
