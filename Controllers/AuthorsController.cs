#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore_WebApplication;
using Microsoft.AspNetCore.Authorization;
using System.Collections.ObjectModel;
using BookStore_WebApplication.Models;

namespace BookStore_WebApplication.Controllers
{
    [Authorize(Roles="admin, user")]
    public class AuthorsController : Controller
    {
        private readonly StoreDBContext _context;

        public AuthorsController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }
            var books = (from book in _context.Books
                         from a in _context.Authors
                         where book.IdAuthors.Contains(author) && a.Id == id
                         select book.Name).ToList();

            ViewData["Books"] = books;

            return View(author);
        }

        List<BooksAuthorDto> GetBooksAuthorDto(int id)
        {
            List<BooksAuthorDto> Books = new List<BooksAuthorDto>();
            var booksList = _context.Books.ToList();

            if (id == 0)
            {
                foreach (var item in booksList)
                {
                    BooksAuthorDto badto = new BooksAuthorDto(item.Id, item.Name, item.Cost, item.Type, item.IdStore, false);
                    Books.Add(badto);
                }
            }
            else
            {
                var author = _context.Authors.Where(a => a.Id == id).Include(b => b.IdBooks).FirstOrDefault();
                var bookListForAuthor = author.IdBooks;
                foreach (var item in booksList)
                {
                    BooksAuthorDto badto = new BooksAuthorDto(item.Id, item.Name, item.Cost, item.Type, item.IdStore, (bookListForAuthor.Count(a => a.Id == item.Id) > 0));
                    Books.Add(badto);
                }
            }
            return Books;
        }

        public IActionResult Create()
        {
            ViewData["Books"] = new SelectList(_context.Books, "Id", "Name");

            ViewBag.Books = GetBooksAuthorDto(0);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Author author)
        {
            if (ModelState.IsValid)
            {
                List<Book> bookList = _context.Books.Include(a => a.IdAuthors).ToList();
                foreach (var item in bookList)
                {
                    string authorId = item.Id.ToString();
                    var t = Request.Form[authorId.ToString()];
                    if (t.Count > 0) //true
                    {
                        author.IdBooks.Add(item);
                    }
                }

                _context.Add(author);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Authors = GetBooksAuthorDto(0);
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            ViewBag.Books = GetBooksAuthorDto(author.Id);
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Book> bookList = _context.Books.Include(a => a.IdAuthors).ToList(); //усі книги
                    Author au = (_context.Authors.Where(a => a.Id == author.Id).Include(a => a.IdBooks).FirstOrDefault());
                    au.Name = author.Name;
                    List<Book> bookAuthorList = au.IdBooks.ToList(); // автори книги до змін

                    foreach (var item in bookList)
                    {
                        string bookId = item.Id.ToString();
                        var t = Request.Form[bookId.ToString()];
                        if (t.Count > 0) //true - за оновленими даними автор є автором книги
                        {
                            if (bookAuthorList.Where(a => a.Id == Int32.Parse(bookId)).Count() == 0) //за попереднім списком не був автором книги - потрібно додати
                            {
                                au.IdBooks.Add(item);
                            }
                        }
                        else //true - за оновленими даними автор не є автором книги
                        {
                            if (bookAuthorList.Where(a => a.Id == Int32.Parse(bookId)).Count() > 0) //за попереднім списком був автором книги - потрібно видалити
                            {
                                au.IdBooks.Remove(bookAuthorList.Where((a => a.Id == Int32.Parse(bookId))).FirstOrDefault());
                            }
                        }
                    }

                    _context.Update(au);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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

            ViewBag.Authors = GetBooksAuthorDto(author.Id);
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            int raw = _context.Database.ExecuteSqlRaw("DELETE FROM BookAuthor WHERE IdAuthor = {0}", author.Id);

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
