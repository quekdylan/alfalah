using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

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
            return View();
        }

    }
}