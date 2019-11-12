using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTest
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            //Setup
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();

            var httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);
            var controller = new BasketController(basketService, orderService);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);


            //Act
            //basketService.AddToBasket(httpContext, "1");
            controller.AddToBasket("1");
            Basket basket = baskets.Collection().FirstOrDefault();

            //Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductID);
        }
        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            //Setup
            IRepository<Basket> baskets = new MockContext<Basket>();
            IRepository<Product> products = new MockContext<Product>();
            IRepository<Order> orders = new MockContext<Order>();

            products.Insert(new Product
            {
                ID = "1",
                Price = 10.00m
            });
            products.Insert(new Product
            {
                ID = "2",
                Price = 5.00m
            });
            products.Insert(new Product
            {
                ID = "3",
                Price = 7.00m
            });

            //Act
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem { ProductID = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem { ProductID = "2", Quantity = 1 });
            basket.BasketItems.Add(new BasketItem { ProductID = "3", Quantity = 1 });
            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);
            IOrderService orderService = new OrderService(orders);

            var controller = new BasketController(basketService, orderService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.ID });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;
            
            //Assert
            Assert.AreEqual(4, basketSummary.BasketCount);
            Assert.AreEqual(32.00m, basketSummary.BasketTotal);


        }
        [TestMethod]
        public void CanCheckOutAndCreatOrder()
        {
            IRepository<Product> products = new MockContext<Product>();
            products.Insert(new Product() { ID = "1", Price = 10.00m });
            products.Insert(new Product() { ID = "2", Price = 5.00m });

            IRepository<Basket> baskets = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductID = "1", Quantity = 2, BasketID = basket.ID });
            basket.BasketItems.Add(new BasketItem() { ProductID = "2", Quantity = 1, BasketID = basket.ID });

            baskets.Insert(basket);

            IBasketService basketService = new BasketService(products, baskets);

            IRepository<Order> orders = new MockContext<Order>();
            IOrderService orderService = new OrderService(orders);

            var controller = new BasketController(basketService, orderService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.ID});
            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);
            Order order = new Order();

            controller.CheckOut(order);


            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            Order orderInRep = orders.Find(order.ID);
            Assert.AreEqual(2, orderInRep.OrderItems.Count);
        }
    }
}
