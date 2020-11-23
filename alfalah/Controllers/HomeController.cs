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
using System.Globalization;

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
        public IActionResult Shop(string type = "All")
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
                ViewBag.type = type;
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

        public IActionResult Delete(CartItem cartItem)
        {
            List<CartItem> cart = new List<CartItem>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                cart = DataDeserialize(HttpContext.Session.GetString("Cart"));
            }

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Name == cartItem.Name && cart[i].Type == cartItem.Type)
                {
                    cart.RemoveAt(i);
                    break;
                }
            }
            HttpContext.Session.SetString("Cart", DataSerialize(cart));
            ViewBag.cart = cart;
            return RedirectToAction("Cart", "Home");
        }

        public IActionResult ChangeQty(IFormCollection collection)
        {
            List<CartItem> cart = new List<CartItem>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                cart = DataDeserialize(HttpContext.Session.GetString("Cart"));
            }

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Name == collection["name"] && (cart[i].Type == collection["type"] || collection["type"] == ""))
                {
                    cart[i].Quantity = Convert.ToInt32(collection["quantity"]);
                    break;
                }
            }
            HttpContext.Session.SetString("Cart", DataSerialize(cart));
            ViewBag.cart = cart;
            return RedirectToAction("Cart", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> newClient(Client client)
        {
            try
            {
                // Email: alfalahnotifications@gmail.com
                // Password: aLfALAh99
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Credentials = new System.Net.NetworkCredential("alfalahnotifications@gmail.com", "aLfALAh99");
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress("alfalahnotifications@gmail.com");
                MailAddress toAddress = new MailAddress("assassined13@gmail.com");

                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Timeout = 5000;

                message.From = fromAddress;
                message.To.Add(toAddress);
                message.IsBodyHtml = false;
                message.Subject = "New Client Notification";
                message.Body =
                    "New Client Notification [Al Falah]" +
                    "\n\nFirst Name: " + client.FirstName +
                    "\nLast Name: " + client.LastName +
                    "\nEmail: " + client.Email +
                    "\nPhone: " + client.Phone +
                    "\nMessage: " + client.Message +
                    "\n\n" + GenerateCart();
                smtpClient.Send(message);
                TempData["msg"] = "<script>alert('Thank you, we will contact you shortly.');</script>";
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                TempData["msg"] = "<script>alert('[Error]' " + ex + ");</script>";
                return RedirectToAction("Index", "Home");
            }
        }

        public string GenerateCart()
        {
            List<CartItem> cart = new List<CartItem>();
            if (HttpContext.Session.GetString("Cart") != null)
            {
                string cartString = "Cart items";
                decimal total = 0;
                cart = DataDeserialize(HttpContext.Session.GetString("Cart"));
                foreach(CartItem item in cart)
                {
                    cartString += ("\n$" + item.Total);
                    cartString += ("\t" + item.Quantity + "x\t" + item.Name);
                    if (!(item.Type == "" || item.Type == null)) {
                        cartString +=(" (" + item.Type + ")");
                    }
                    total += decimal.Parse(item.Total, NumberStyles.Currency);
                }
                cartString += ("\nTotal: $" + total);
                return cartString;
            }
            else
            {
                return "User cart is empty";
            }
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