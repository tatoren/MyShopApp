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
        IBasketService basketService;
        IOrderService orderService;

        public BasketController(IBasketService BasketService, IOrderService OrderService)
        {
            this.basketService = BasketService;
            this.orderService = OrderService;
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

        public ActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckOut(Order order)
        {
            var basketItem = basketService.GetBasketItems(this.HttpContext);
            order.OrderStatus = "Order Created";

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