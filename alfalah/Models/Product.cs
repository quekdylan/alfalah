
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alfalah.Models
{
    public class Product
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string Weight { get; set; }
        
        public string Variations { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }
    }
}
