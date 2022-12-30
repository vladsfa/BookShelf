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
using BookStore_WebApplication.Models;

namespace BookStore_WebApplication.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class BooksController : Controller
    {
        private readonly StoreDBContext _context;

        public BooksController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var storeDBContext = _context.Books.Include(b => b.IdStoreNavigation);
            return View(await storeDBContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.IdStoreNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var authors = (from author in _context.Authors
                         from b in _context.Books
                         where author.IdBooks.Contains(b) && b.Id == id
                         select author.Name).ToList();

            ViewData["Authors"] = authors;

            return View(book);
        }

        List<AuthorsBookDto> GetAuthorsBookDto(int id)
        {
            List<AuthorsBookDto> Authors = new List<AuthorsBookDto>();
            var authorsList = _context.Authors.ToList();

            if (id == 0)
            {
                foreach (var item in authorsList)
                {
                    AuthorsBookDto abdto = new AuthorsBookDto(item.Id, item.Name, false);
                    Authors.Add(abdto);
                }
            }
            else
            {
                var book = _context.Books.Where(b => b.Id == id).Include(a => a.IdAuthors).FirstOrDefault();
                var authListForBook = book.IdAuthors;
                foreach (var item in authorsList)
                {
                    AuthorsBookDto abdto = new AuthorsBookDto(item.Id, item.Name, (authListForBook.Count(a => a.Id == item.Id) > 0));
                    Authors.Add(abdto);
                }
            }
            return Authors;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["IdStore"] = new SelectList(_context.BookStores, "Id", "Name");            
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name");

            ViewBag.Authors = GetAuthorsBookDto(0);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Cost,Type,IdStore")] Book book)
        {
            if (ModelState.IsValid)
            {
                List<Author> authorList = _context.Authors.Include(a => a.IdBooks).ToList();
                foreach (var item in authorList)
                {
                    string authorId = item.Id.ToString();
                    var t = Request.Form[authorId.ToString()];
                    if (t.Count > 0) //true
                    {
                        book.IdAuthors.Add(item);
                    }
                }

                _context.Add(book);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdStore"] = new SelectList(_context.BookStores, "Id", "Id", book.IdStore);
            ViewData["IdAuthors"] = new SelectList(_context.Authors, "Id", "Id", book.IdAuthors);

            ViewBag.Authors = GetAuthorsBookDto(0);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["IdStore"] = new SelectList(_context.BookStores, "Id", "Name", book.IdStore);

            ViewBag.Authors = GetAuthorsBookDto(book.Id);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Cost,Type,IdStore")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Author> authorList = _context.Authors.Include(a => a.IdBooks).ToList(); //усі автори
                    Book bk = (_context.Books.Where(b => b.Id == book.Id).Include(b => b.IdAuthors).FirstOrDefault());
                    bk.Name = book.Name;
                    bk.Cost = book.Cost;
                    bk.Type = book.Type;
                    bk.IdStore = book.IdStore;
                    List<Author> authorBookList = bk.IdAuthors.ToList(); // автори книги до змін

                    foreach (var item in authorList)
                    {
                        string authorId = item.Id.ToString();
                        var t = Request.Form[authorId.ToString()];
                        if (t.Count > 0) //true - за оновленими даними автор є автором книги
                        {
                            if (authorBookList.Where(a => a.Id == Int32.Parse(authorId)).Count() == 0) //за попереднім списком не був автором книги - потрібно додати
                            {
                                bk.IdAuthors.Add(item);
                            }
                        }
                        else //true - за оновленими даними автор не є автором книги
                        {
                            if (authorBookList.Where(a => a.Id == Int32.Parse(authorId)).Count() > 0) //за попереднім списком був автором книги - потрібно видалити
                            {
                                bk.IdAuthors.Remove(authorBookList.Where((a => a.Id == Int32.Parse(authorId))).FirstOrDefault());
                            }
                        }
                    }

                    _context.Update(bk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["IdStore"] = new SelectList(_context.BookStores, "Id", "Id", book.IdStore);
            ViewBag.Authors = GetAuthorsBookDto(book.Id);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.IdStoreNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = _context.Books.Find(id);
            int author = _context.Database.ExecuteSqlRaw("DELETE FROM BookAuthor WHERE IdBook = {0}", book.Id);
            int order = _context.Database.ExecuteSqlRaw("DELETE FROM BookOrder WHERE IdBook = {0}", book.Id);

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }    
    }
}
