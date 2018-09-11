using DO_Arbetsprov.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DO_Arbetsprov.Controllers
{
    public class ProductsController : Controller
    {
        PriceService priceService = new PriceService();
        ProductService productService = new ProductService();

        //Shows a list of all products in the database
        public ActionResult Index()
        {
            return View(productService.GetAllProducts());
        }

        //Shows a page showing the timeline of prices a given product has had
        public ActionResult Timeline(string id)
        {
            return View(priceService.GetProductPriceHistory(id));
        }
    }
}