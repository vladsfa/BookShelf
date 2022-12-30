using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookStore_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class ChartsController : ControllerBase
    {
        private readonly StoreDBContext _context;
        public ChartsController(StoreDBContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var stores = _context.BookStores.ToList();
            List<object> numBook = new List<object>();
            numBook.Add(new[] { "Name", "Number of books" });
            foreach (var store in stores)
            {
                numBook.Add(new object[] { store.Name, COUNTER(store.Id) });
            }
            return new JsonResult(numBook);
        }

        [HttpGet("JsonData1")]
        public JsonResult JsonData_Books()
        {
            BookCOUNTER("Навчання");
            var books = _context.Books.ToList();
            var sortBooks = books.GroupBy(x => new { x.Type }).Select(x => x.First()).ToList();
            List<object> typeBook = new List<object>();
            typeBook.Add(new[] { "Type", "Number of books" });
            foreach (var book in sortBooks)
            {
                typeBook.Add(new object[] { book.Type, BookCOUNTER(book.Type.ToString()) });
            }
            return new JsonResult(typeBook);
        }

        private int BookCOUNTER(string typeName)
        {
            var count = from Book in
                        (from Book in _context.Books
                         where
                           Book.Type == typeName
                         select new
                         {
                             Book.Type,
                             Dummy = "x"
                         })
                        group Book by new { Book.Dummy } into g
                        select new
                        {
                            Column1 = g.Count(p => p.Type != null)
                        };

            foreach (var c in count)
            {
                return c.Column1;
            }

            return 0;
        }

        private int COUNTER(int IDSTORE)
        {
            var count = from a in _context.BookStores
                        join b in _context.Books on new { IdStore = a.Id } equals new { IdStore = b.IdStore } into b_join
                        from b in b_join.DefaultIfEmpty()
                        group new { a, b } by new
                        {
                            a.Id
                        } into g
                        select new
                        {
                            Id = (int?)g.Key.Id,
                            countOfBooks = g.Count(p => p.b.IdStore == IDSTORE)
                        };

            foreach (var c in count)
            {
                if (c.Id == IDSTORE)
                {
                    return c.countOfBooks;
                }
            }

            return 0;
        }
    }
}
