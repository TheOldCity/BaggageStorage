using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BaggageStorage.Controllers;
using BaggageStorage.Classes;

namespace BaggageStorage.Resources
{
    public class MainMenuController : BaseController
    {
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/mainmenu");
            return PartialView();
        }
    }
}