using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Quickpaste.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Serves the Angular application which is referenced in the Index.cshtml file in the Views/Home folder
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
