#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BookStore_WebApplication.Controllers
{
    [Authorize(Roles ="admin, user")]
    public class BookStoresController : Controller
    {
        private readonly StoreDBContext _context;

        public BookStoresController(StoreDBContext context)
        {
            _context = context;
        }

        // GET: BookStores
        public async Task<IActionResult> Index()
        {
            return View(await _context.BookStores.ToListAsync());
        }

        // GET: BookStores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookStore = await _context.BookStores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookStore == null)
            {
                return NotFound();
            }

            return View(bookStore);
        }

        // GET: BookStores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookStores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber,Email")] BookStore bookStore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookStore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookStore);
        }

        // GET: BookStores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookStore = await _context.BookStores.FindAsync(id);
            if (bookStore == null)
            {
                return NotFound();
            }
            return View(bookStore);
        }

        // POST: BookStores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,PhoneNumber,Email")] BookStore bookStore)
        {
            if (id != bookStore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookStore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookStoreExists(bookStore.Id))
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
            return View(bookStore);
        }

        // GET: BookStores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookStore = await _context.BookStores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookStore == null)
            {
                return NotFound();
            }

            return View(bookStore);
        }

        // POST: BookStores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookStore = await _context.BookStores.FindAsync(id);

            var books = (from b in _context.Books
                         where b.IdStore == bookStore.Id
                         select b).ToList();

            var emp = (from e in _context.Employees
                         where e.IdStore == bookStore.Id
                         select e).ToList();

            if (books.Count() != 0)
            {
                foreach (var item in books)
                {
                    int author = _context.Database.ExecuteSqlRaw("DELETE FROM BookAuthor WHERE IdBook = {0}", item.Id);
                    int order = _context.Database.ExecuteSqlRaw("DELETE FROM BookOrder WHERE IdBook = {0}", item.Id);
                    _context.Books.Remove(item);
                }
            }

            if (emp.Count() != 0)
            {
                foreach (var item in emp)
                {
                    var orders = (from o in _context.Orders
                                  where o.IdEmployee == item.Id
                                  select o).ToList();

                    if (orders.Count() != 0)
                    {
                        foreach (var order in orders)
                        {
                            int bookOrder = _context.Database.ExecuteSqlRaw("DELETE FROM BookOrder WHERE IdOrder = {0}", order.Id);
                            _context.Orders.Remove(order);
                        }
                    }                  
                    _context.Employees.Remove(item);
                }
            }

            _context.BookStores.Remove(bookStore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookStoreExists(int id)
        {
            return _context.BookStores.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            var allStores = (from BookStore in _context.BookStores
                                             select BookStore.Name).ToList();

                            //перегляд усіх листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                BookStore newstore = new BookStore();

                                var b = (from store in _context.BookStores
                                         where store.Name.Contains(worksheet.Name)
                                         select store).ToList();

                                if (b.Count > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    try
                                    {
                                        newstore.Name = worksheet.Name;
                                        newstore.Address = worksheet.Cell("B1").Value.ToString();
                                        newstore.PhoneNumber = worksheet.Cell("B2").Value.ToString();
                                        newstore.Email = worksheet.Cell("B3").Value.ToString();

                                        _context.BookStores.Add(newstore);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }                                    
                                }                                
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {

                var bookstores = _context.BookStores.ToList();

                foreach (var bs in bookstores)
                {
                    var worksheet = workbook.Worksheets.Add(bs.Name);

                    worksheet.Cell("A1").Value = "Address";
                    worksheet.Cell("A2").Value = "PhoneNumber";
                    worksheet.Cell("A3").Value = "Email";
                    worksheet.Cell("A4").Value = "Employees";
                    worksheet.Cell("B4").Value = "Books";
                    worksheet.Cell("C4").Value = "Orders";

                    worksheet.Cells("A1:A3").Style.Font.Bold = true;
                    worksheet.Cells("A4:C4").Style.Font.Bold = true;

                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 6;
                    worksheet.Column(4).Width = 12;                                        
                                       
                    for (int i = 0, j = 5; i < bookstores.Count; i++)
                    {
                        if (bookstores[i].Name == worksheet.Name)
                        {
                            worksheet.Cell(1, 2).Value = bookstores[i].Address;

                            worksheet.Cell(2, 2).Value = bookstores[i].PhoneNumber;
                            worksheet.Cell(2, 2).SetDataType(XLDataType.Text);

                            worksheet.Cell(3, 2).Value = bookstores[i].Email;


                            var em = _context.Employees.Where(a => a.IdStore == bookstores[i].Id).ToList();
                            foreach (var employee in em)
                            {
                                worksheet.Cell(j, 1).Value = employee.FullName;
                                j++;
                            }

                            j = 5;
                            var b = _context.Books.Where(a => a.IdStore == bookstores[i].Id).ToList();
                            foreach (var book in b)
                            {
                                worksheet.Cell(j, 2).Value = book.Name;
                                j++;
                            }

                            j = 5;
                            var orders = (from ord in _context.Orders
                                          from emp in _context.Employees
                                          where ord.IdEmployee == emp.Id && emp.IdStore == bookstores[i].Id
                                          select ord).ToList();

                            if (orders.Count() == 0)
                            {
                                worksheet.Cell(j, 3).Value = "0";
                            }
                            else
                            {
                                worksheet.Cell(j, 3).Value = orders.Count();
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"BookStores_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
        
        public ActionResult DExport()
        {
            using(MemoryStream ms = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
                {
                    var bookstores = _context.BookStores.ToList();

                    MainDocumentPart mainPart = package.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = new Body();

                    foreach (var store in bookstores)
                    {
                        #region RUNs
                        Run runStoreName = new Run();
                        Run runAddress = new Run();
                        Run runPhoneNumber = new Run();
                        Run runEmail = new Run();

                        Run runEmployees = new Run();
                        Run runEmp = new Run();

                        Run runB = new Run();

                        Run rubOrd = new Run();

                        Run run = new Run();

                        #endregion RUNs

                        #region PARAGRAPHs
                        Paragraph storeName = new Paragraph();
                        Paragraph storeAddress = new Paragraph();
                        Paragraph storePhoneNumber = new Paragraph();
                        Paragraph storeEmail = new Paragraph();

                        Paragraph storeEmployees = new Paragraph();
                        Paragraph parEmp = new Paragraph();

                        Paragraph parB = new Paragraph();

                        Paragraph parOrd = new Paragraph();

                        Paragraph paragraph = new Paragraph();
                        #endregion PARAGRAPHs

                        var em = _context.Employees.Where(a => a.IdStore == store.Id).ToList();
                        var b = _context.Books.Where(a => a.IdStore == store.Id).ToList();

                        var orders = (from ord in _context.Orders
                                      from emp in _context.Employees
                                      where ord.IdEmployee == emp.Id && emp.IdStore == store.Id
                                      select ord).ToList();

                        var d = _context.Deliveries.Where(a => a.Id == store.Id).ToList();

                        RunProperties runHeaderProperties = runStoreName.AppendChild(new RunProperties(new Bold()));
                        RunProperties runProperties = runStoreName.AppendChild(new RunProperties(new Italic()));


                        runStoreName.AppendChild(new Text($"Назва:    		                {store.Name}"));
                        storeName.Append(runStoreName);
                        body.Append(storeName);

                        runAddress.AppendChild(new Text($"Адреса: 		                 {store.Address}"));
                        storeAddress.Append(runAddress);
                        body.Append(storeAddress);

                        runPhoneNumber.AppendChild(new Text($"Номер телефону:  {store.PhoneNumber}"));
                        storePhoneNumber.Append(runPhoneNumber);
                        body.Append(storePhoneNumber);

                        runEmail.AppendChild(new Text($"Email:      	            	    {store.Email}"));
                        storeEmail.Append(runEmail);
                        body.Append(storeEmail);

                        runEmp.AppendChild(new Text("Працівники:"));
                        parEmp.Append(runEmp);
                        body.Append(parEmp);

                        if (em.Count() > 0)
                        {
                            foreach (var employee in em)
                            {
                                runEmployees.AppendChild(new Text($"•   {employee.FullName}"));
                                storeEmployees.Append(runEmployees);
                                body.Append(storeEmployees);
                            }
                        }
                        else
                        {
                            runEmployees.AppendChild(new Text("•   Працівники відсутні."));
                            storeEmployees.Append(runEmployees);
                            body.Append(storeEmployees);
                        }

                        runB.AppendChild(new Text("Книги:"));
                        parB.Append(runB);
                        body.Append(parB);


                        if (b.Count() > 0)
                        {
                            foreach (var book in b)
                            {
                                Run runBooks = new Run();
                                Paragraph storeBooks = new Paragraph();
                                runBooks.AppendChild(new Text($"•   {book.Name}"));
                                storeBooks.Append(runBooks);
                                body.Append(storeBooks);
                            }
                        }
                        else
                        {
                            Run runBooks = new Run();
                            Paragraph storeBooks = new Paragraph();
                            runBooks.AppendChild(new Text("•   Книги відсутні."));
                            storeBooks.Append(runBooks);
                            body.Append(storeBooks);
                        }

                        rubOrd.AppendChild(new Text("Замовлення:"));
                        parOrd.Append(rubOrd);
                        body.Append(parOrd);


                        if (orders.Count() != 0)
                        {
                            Run runOrders = new Run();
                            Paragraph storeOrders = new Paragraph();
                            runOrders.AppendChild(new Text($"•   Кількість замовлень: {orders.Count()}."));
                            storeOrders.Append(runOrders);
                            body.Append(storeOrders);
                        }
                        else
                        {
                            Run runOrders = new Run();
                            Paragraph storeOrders = new Paragraph();
                            runOrders.AppendChild(new Text("•   Замовлення відсутні."));
                            storeOrders.Append(runOrders);
                            body.Append(storeOrders);
                        }

                        run.AppendChild(new Text("------------------------------------------------------------------------------------------------------------------------------------------"));
                        paragraph.Append(run);
                        body.Append(paragraph);
                    }

                    mainPart.Document.Append(body);
                    package.Close();
                }

                ms.Flush();
                return new FileContentResult(ms.ToArray(), "application/vnd.ms-word")
                {
                    //змініть назву файла відповідно до тематики Вашого проєкту
                    FileDownloadName = $"BookStores_{DateTime.UtcNow.ToShortDateString()}.docx"
                };
            }
        }
    }
}
