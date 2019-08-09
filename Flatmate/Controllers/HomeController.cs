using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flatmate.Models;
using Flatmate.ViewModels.Dashboard;
using Flatmate.Models.EntityModels;
using Flatmate.Data;

namespace Flatmate.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlatmateContext _context;
        public HomeController(FlatmateContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
