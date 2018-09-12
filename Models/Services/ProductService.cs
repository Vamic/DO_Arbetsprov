using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DO_Arbetsprov.Models
{
    public class ProductService
    {
        DBHandler dbHandler = new DBHandler();

        //Gets all unique product codes
        public List<string> GetAllProducts()
        {
            return dbHandler.GetAllProducts();
        }
    }
}