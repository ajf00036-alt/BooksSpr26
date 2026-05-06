using BooksSpr2026.Data;
using BooksSpr2026.Models;
using BooksSpr2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksSpr2026.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly BooksDbContext _dbContext;

        private IWebHostEnvironment _environment;
        //for images

        public BookController(BooksDbContext dbContext, IWebHostEnvironment environment) //dependency injection
        {
            _dbContext = dbContext; 

            _environment = environment;

        }


        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(
                o => new SelectListItem 
                { Text=o.Name, 
                    Value=o.CategoryId.ToString() });
            //projection: taking an object of a certain data type and projecting it
            //(transforming it) to something else

            //ViewBag.ListOfCategories = listOfCategories;
            ////plugged in the set of SelectListItems into the ViewBag's ListOfCategories variable

            //ViewData["ListOfCategoriesVD"] = listOfCategories;

            //ViewModel: a view is used to support a complex view when the view is supposed to display data from multiple models
            BookWithCategoriesVM bookWithCategoriesVMobj = new BookWithCategoriesVM();

            bookWithCategoriesVMobj.Book = new Book();

            bookWithCategoriesVMobj.ListOfCategories = listOfCategories;


            return View(bookWithCategoriesVMobj);

        }

        [HttpPost]
        public IActionResult Create(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _environment.WebRootPath;

                if(imgFile != null)
                {
                    using(var fileStream = new FileStream(Path.
                        Combine(wwwRootPath, "Images\\" + imgFile.FileName),
                        FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream); //saves the file in the specified location
                        //                            wwwroot\images\filename.png

                    }
                    //save the url in the Book model
                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName;

                }

                _dbContext.Books.Add(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(bookWithCategoriesVMobj); //if the model is invalid, the form will be displayed with the appropriate error message

        }

        public IActionResult Index()
        {
            var listOfBooks = _dbContext.Books.ToList();

            return View(listOfBooks);

        }

        public IActionResult Edit(int id)
        {
            Book book = _dbContext.Books.Find(id);

            IEnumerable<SelectListItem> listOfCategories = _dbContext.Categories.ToList().Select(
                o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.CategoryId.ToString()
                });

            BookWithCategoriesVM bookWithCategoriesVM = new BookWithCategoriesVM();

            bookWithCategoriesVM.Book = book;

            bookWithCategoriesVM.ListOfCategories = listOfCategories;

            return View(bookWithCategoriesVM);

        }

        [HttpPost]
        public IActionResult Edit(BookWithCategoriesVM bookWithCategoriesVMobj, IFormFile imgFile)
        {

            string wwwrootPath = _environment.WebRootPath;

            if (ModelState.IsValid)
            {
                if(imgFile != null)
                {
                    if (!string.IsNullOrEmpty(bookWithCategoriesVMobj.Book.ImgUrl))
                    {
                        var oldImgPath = Path.Combine(wwwrootPath, bookWithCategoriesVMobj.Book.ImgUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);

                        }

                    }

                    using (var fileStream = new FileStream(Path.Combine(wwwrootPath, @"\Images" + imgFile.FileName), FileMode.Create))
                    {
                        imgFile.CopyTo(fileStream);

                    }

                    bookWithCategoriesVMobj.Book.ImgUrl = @"\Images\" + imgFile.FileName;

                }

                _dbContext.Books.Update(bookWithCategoriesVMobj.Book);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(bookWithCategoriesVMobj);

        }

    }

}
