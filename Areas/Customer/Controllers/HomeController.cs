using System.Diagnostics;
using System.Security.Claims;
using BooksSpr2026.Data;
using BooksSpr2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksSpr2026.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly BooksDbContext _dbContext;

        public HomeController(BooksDbContext dbContext) //dependency injection
        {
             _dbContext = dbContext;

        }

        public IActionResult Index()
        {
            var listOfBooks = _dbContext.Books.Include(c => c.Category);
            //link query

            return View(listOfBooks.ToList());

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Details(int id)
        {
            var book = _dbContext.Books.Include(b => b.Category).FirstOrDefault(b => b.BookId == id);

            var cartItem = new CartItem()
            {
                BookId = id,
                book = book,
                Quantity = 1

            };

            return View(cartItem);

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cartItem.UserId = userId;

            CartItem existingCart = _dbContext.CartItems.FirstOrDefault(c => c.UserId == userId && c.BookId == cartItem.BookId); 
            //BROKEN WHY

            if (existingCart != null)//cartItem exists already
            {
                existingCart.Quantity += cartItem.Quantity;
                _dbContext.CartItems.Update(existingCart);

            }
            else //cartItem does not exist, add as a new cartItem
            {
                _dbContext.CartItems.Add(cartItem);

            }

            _dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

    }
}
