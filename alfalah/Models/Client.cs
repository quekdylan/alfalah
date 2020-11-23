using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alfalah.Models
{
    public class Client
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Message { get; set; }

        public List<CartItem> Cart { get; set; }
    }
}
