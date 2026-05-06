using BooksSpr2026.Data;
using BooksSpr2026.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksSpr2026.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private BooksDbContext _dbContext;

        public CategoryController(BooksDbContext dbContext)
        {
             _dbContext = dbContext;

        }

        public IActionResult Index()
        {
            var listOfCategories = _dbContext.Categories.ToList();

            return View(listOfCategories);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(Category categoryObj)
        {
            //custom validation
            if(categoryObj.Name != null && categoryObj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("Name", "Category name cannot be 'test'");

            }

            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(categoryObj);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Category");

            }

            return View(categoryObj);

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category category = _dbContext.Categories.Find(id);

            return View(category);

        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("CategoryId, Name, Description")]Category category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(category);
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Category");

            }

            return View(category);

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Category category = _dbContext.Categories.Find(id);

            return View(category);

        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePOST(int id)
        {
            Category category = _dbContext.Categories.Find(id);

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Category");

        }

        public IActionResult Details(int id)
        {
            Category category = _dbContext.Categories.Find(id);

            return View(category);

        }

    }

}
