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
    public class BaggageRegisterController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<BaggageRegisterController> _localizer;

        public BaggageRegisterController(AppDbContext db, IStringLocalizer<BaggageRegisterController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/baggageregister", Name = "BaggageRegisterIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/baggageregister");
            return PartialView("BaggageRegisterIndex");
        }
        
    }
}