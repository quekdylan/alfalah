using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace alfalah.Models
{
    public class CartItem
    {
       
        public string Name { get; set; }

        public string Price { get; set; }

        public string ImagePath { get; set; }

        public string Type { get; set; }

        public int Quantity { get; set; }

        public string Total { get { return (decimal.Parse(Price, NumberStyles.Currency) * Convert.ToDecimal(Quantity)).ToString(); } }
    }
}
