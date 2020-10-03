using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.VisualBasic.FileIO;
using alfalah.Models;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace alfalah.Controllers
{

    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
        [Route("/about-us")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/shop")]
        public IActionResult Shop()
        {
            using (TextFieldParser parser = new TextFieldParser(@"wwwroot\items.csv"))
            {
                List<Product> productList = new List<Product>();
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (fields[0] == "Type")
                    {
                        continue;
                    }
                    Product product = new Product();
                    product.Type = fields[0];
                    product.Name = fields[1];
                    product.Price = fields[2];
                    product.Weight = fields[3];
                    product.Variations = fields[4];
                    product.Description = fields[5];
                    product.ImagePath = fields[6];
                    productList.Add(product);
                }
                ViewBag.productList = productList;
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> newCartItem(CartItem cartItem)
        {
            List<CartItem> cart = new List<CartItem>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                cart = DataDeserialize(HttpContext.Session.GetString("Cart"));
            }

            bool flag = false;
            for (int i = 0; i < cart.Count; i++)
            {
                if(cart[i].Name == cartItem.Name && cart[i].Type == cartItem.Type)
                {
                    cart[i].Quantity += cartItem.Quantity;
                    flag = true;
                }
            }

            if(flag == false)
            {
                cart.Add(cartItem);
            }

            HttpContext.Session.SetString("Cart", DataSerialize(cart));
            return RedirectToAction("Cart", "Home");
        }

        [Route("/cart")]
        public IActionResult Cart()
        {
            List<CartItem> cart = new List<CartItem>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                cart = DataDeserialize(HttpContext.Session.GetString("Cart"));
            }
            ViewBag.cart = cart;
            return View();
        }

        public static string DataSerialize(List<CartItem> myList)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer s = new XmlSerializer(myList.GetType());
            s.Serialize(sw, myList);
            return sw.ToString();
        }

        public static List<CartItem> DataDeserialize(string data)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<CartItem>));
            List<CartItem> newList = (List<CartItem>)xs.Deserialize(new StringReader(data));
            return newList;
        }
    }
}