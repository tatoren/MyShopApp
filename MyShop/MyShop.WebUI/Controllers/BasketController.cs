using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IRepository<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;

        public BasketController(IBasketService BasketService, IOrderService OrderService, IRepository<Customer> Customers)
        {
            this.basketService = BasketService;
            this.orderService = OrderService;
            this.customers = Customers;
        }

        // GET: Basket
        public ActionResult Index()
        {
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string ID)
        {
            basketService.AddToBasket(this.HttpContext, ID);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string ID)
        {
            basketService.RemoveFromBasket(this.HttpContext, ID);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        [Authorize]
        public ActionResult CheckOut()
        {
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            if (customer != null)
            {
                Order order = new Order()
                {
                    City = customer.City,
                    Email = customer.Email,
                    Street = customer.Street,
                    State = customer.State,
                    PostalCode = customer.PostalCode,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                };
                return View(order);
            }
            else
            {
                return RedirectToAction("Error");
            }

            
        }
        [HttpPost]
        [Authorize]
        public ActionResult CheckOut(Order order)
        {
            var basketItem = basketService.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

            //Process Payment

            order.OrderStatus = "Payment Processed";
            orderService.CreateOrder(order, basketItem);
            basketService.ClearBasket(this.HttpContext);

            return RedirectToAction("ThankYou", new { orderID = order.ID});
        }

        public ActionResult ThankYou(string orderID)
        {
            ViewBag.OrderId = orderID;

            return View();
        }

    }
}