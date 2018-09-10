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
    public class HomeController : Controller
    {
        DBService database = new DBService();

        //Shows a list of all products in the database
        public ActionResult Index()
        {
            return View(database.GetAllProducts());
        }

        //Shows a page showing all prices for the given product
        public ActionResult Product(string id)
        {
            List<Price> prices = database.GetProductPrices(id);
            return View(prices);
        }
    }
}