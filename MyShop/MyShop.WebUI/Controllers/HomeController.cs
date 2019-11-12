﻿using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;

        public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoriesContext)
        {
            context = productContext;
            productCategories = productCategoriesContext;
        }
        public ActionResult Index(string category = null)
        {
            List<Product> products;
            List<ProductCategory> categories = productCategories.Collection().ToList();

            if (category == null)
            {
                products = context.Collection().ToList();
            }
            else
            {
                products = context.Collection().Where(p => p.Category == category).ToList();
            }
            ProductListViewModel model = new ProductListViewModel();
            model.Products = products;
            model.ProductCategories = categories;

            return View(model);
        }
        public ActionResult Details(string ID)
        {
            Product product = context.Find(ID);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
           
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}