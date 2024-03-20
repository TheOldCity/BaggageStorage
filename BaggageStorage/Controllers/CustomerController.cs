using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using BaggageStorage.Data;
using Newtonsoft.Json;
using System;
using System.Linq;
using BaggageStorage.Classes;

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<CustomerController> _localizer;

        public CustomerController(AppDbContext db, IStringLocalizer<CustomerController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/customer", Name = "CustomerIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/customer");
            return PartialView("CustomerIndex");
        }
        
    }
}