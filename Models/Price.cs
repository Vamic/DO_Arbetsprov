using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DO_Arbetsprov.Models
{
    public class Price
    {
        [Key]
        public int PriceValueId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CatalogEntryCode { get; set; }
        public string MarketId { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public decimal UnitPrice { get; set; }
    }
}