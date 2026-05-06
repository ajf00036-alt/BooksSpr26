using BooksSpr2026.Data;
using BooksSpr2026.Models;
using BooksSpr2026.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BooksSpr2026.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin, Employee")]
    public class OrderController : Controller
    {
        private readonly BooksDbContext _dbContext;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(BooksDbContext dbContext)
        {
             _dbContext = dbContext;

        }

        public IActionResult Index()
        {
            IEnumerable<Order> listOfOrders = _dbContext.Orders.Include(o => o.ApplicationUser);



            return View(listOfOrders);
        }

        public IActionResult Details(int id)
        {
            var order = _dbContext.Orders.Include(o => o.ApplicationUser).FirstOrDefault(o => o.OrderId == id);

            var orderDetails = _dbContext.OrderDetails.Where(od => od.OrderId == id).Include(od => od.Book).ToList();

            OrderVM orderVM = new OrderVM
            {
                order = order,
                OrderDetails = orderDetails

            };

            return View(orderVM);

        }

        [HttpPost]
        public IActionResult UpdateOrderInformation()
        {
            Order orderFromDB = _dbContext.Orders.Find(OrderVM.order.OrderId);

            orderFromDB.CustomerName = OrderVM.order.CustomerName;

            orderFromDB.StreetAddress = OrderVM.order.StreetAddress;
            orderFromDB.City = OrderVM.order.City;
            orderFromDB.State = OrderVM.order.State;
            orderFromDB.PostalCode = OrderVM.order.PostalCode;
            orderFromDB.Phone = OrderVM.order.Phone;

            orderFromDB.Carrier = OrderVM.order.Carrier;
            orderFromDB.ShippingDate = OrderVM.order.ShippingDate;
            orderFromDB.TrackingNumber = OrderVM.order.TrackingNumber;

            _dbContext.Orders.Update(orderFromDB);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", new { id = orderFromDB.OrderId });

        }

        [HttpPost]
        public IActionResult ProcessOrder()
        {
            Order order = _dbContext.Orders.Find(OrderVM.order.OrderId);

            order.OrderStatus = "Processing";

            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now).AddDays(7);

            order.Carrier = "USPS";

            _dbContext.Orders.Update(order);

            _dbContext.SaveChanges();

            return RedirectToAction("Details", new {id = order.OrderId});

        }

        public IActionResult CompleteOrder()
        {
            Order order = _dbContext.Orders.Find(OrderVM.order.OrderId);

            order.OrderStatus = "Shipped and Completed";

            order.ShippingDate = DateOnly.FromDateTime(DateTime.Now);

            _dbContext.Orders.Update(order);

            _dbContext.SaveChanges();

            return RedirectToAction("Details", new { id = order.OrderId });

        }
    }
}
