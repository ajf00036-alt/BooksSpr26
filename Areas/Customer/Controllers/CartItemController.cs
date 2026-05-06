using BooksSpr2026.Data;
using BooksSpr2026.Models;
using BooksSpr2026.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Security.Claims;

namespace BooksSpr2026.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]

    public class CartItemController : Controller
    {
        private readonly BooksDbContext _dbContext;
        public CartItemController(BooksDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.book);


            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItemsList,

                Order = new Order()

            };

            foreach(var cartItem in cartItemsList)
            {
                cartItem.SubTotal = cartItem.Quantity * cartItem.book.Price;

                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;

            }

            return View(shoppingCartVM);

        }
        
        public IActionResult IncrementByOne(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);

            cartItem.Quantity++;

            _dbContext.Update(cartItem);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult DecrementByOne(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);

            if (cartItem.Quantity <= 1)
            {
                _dbContext.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();

            }
            else
            {
                cartItem.Quantity--;

                _dbContext.Update(cartItem);
                _dbContext.SaveChanges();

            }

                return RedirectToAction("Index");


        }

        public IActionResult RemoveFromCart(int id)
        {
            CartItem cartItem = _dbContext.CartItems.Find(id);

            _dbContext.CartItems.Remove(cartItem);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult ReviewOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _dbContext.ApplicationUsers.Find(userId);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.book);

            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                CartItems = cartItemsList,

                Order = new Order()
                {
                    CustomerName = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PosstalCode,
                    Phone = user.PhoneNumber

                }

            };

            //calculate and add the order total to the Order part of the shoppingCartVM
            foreach(var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.book.Price * cartItem.Quantity;

                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;

            }

            return View(shoppingCartVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewOrder(ShoppingCartVM shoppingCartVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var user = _dbContext.ApplicationUsers.Find(userId);

            var cartItemsList = _dbContext.CartItems.Where(c => c.UserId == userId).Include(c => c.book);

            shoppingCartVM.CartItems = cartItemsList;

            if (!ModelState.IsValid)
            {
                return View("ReviewOrder", shoppingCartVM);

            }

            foreach (var cartItem in shoppingCartVM.CartItems)
            {
                cartItem.SubTotal = cartItem.book.Price * cartItem.Quantity;

                shoppingCartVM.Order.OrderTotal += cartItem.SubTotal;

            }

            shoppingCartVM.Order.ApplicationUserId = userId;
            shoppingCartVM.Order.OrderDate = DateOnly.FromDateTime(DateTime.Now);
            shoppingCartVM.Order.OrderStatus = "Pending";
            shoppingCartVM.Order.PaymentStatus = "Pending";

            //save order
            _dbContext.Orders.Add(shoppingCartVM.Order);
            _dbContext.SaveChanges();

            //save order details
            foreach(var eachCartItem in shoppingCartVM.CartItems)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = shoppingCartVM.Order.OrderId,
                    BookId = eachCartItem.BookId,
                    Quantity = eachCartItem.Quantity,
                    Price = eachCartItem.book.Price

                };

                _dbContext.OrderDetails.Add(orderDetail);

            }

            _dbContext.SaveChanges();


           
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = "https://localhost:7155/" + $"customer/cartitem/OrderConfirmation?id={shoppingCartVM.Order.OrderId}",
                CancelUrl = "https://localhost:7155/" + "customer/cartitem/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                //{
                //    new Stripe.Checkout.SessionLineItemOptions
                //    {
                //        Price = "{{PRICE_ID}}",
                //        Quantity = 2,
                //    },
                //},
                Mode = "payment",
            };

            foreach(var item in shoppingCartVM.CartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.book.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.book.BookTitle

                        }

                    },

                    Quantity = item.Quantity

                };

                options.LineItems.Add(sessionLineItem);

            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            shoppingCartVM.Order.SessionID = session.Id;
            _dbContext.SaveChanges();

            Response.Headers.Add("Location", session.Url);


            return new StatusCodeResult(303);
            //return RedirectToAction("OrderConfirmation", new {id = shoppingCartVM.Order.OrderId});

        }

        public IActionResult OrderConfirmation(int id)
        {
            Order order = _dbContext.Orders.Find(id);

            var sessID = order.SessionID;

            var service = new SessionService();

            Session session = service.Get(sessID);//fetches the session info

            if(session.PaymentStatus.ToLower() == "paid")
            {
                order.PaymentIntentID = session.PaymentIntentId;
                order.PaymentStatus = "Approved";

            }
                


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<CartItem> listOfCartItems = _dbContext.CartItems.ToList().Where(c => c.UserId == userId).ToList();

            _dbContext.CartItems.RemoveRange(listOfCartItems);
            _dbContext.SaveChanges();

            return View(id);

        }

    }
}
