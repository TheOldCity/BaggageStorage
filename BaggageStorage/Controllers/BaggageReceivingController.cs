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
    public class BaggageReceivingController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<BaggageReceivingController> _localizer;

        public BaggageReceivingController(AppDbContext db, IStringLocalizer<BaggageReceivingController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/baggagereceiving", Name = "BaggageReceivingIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/baggagereceiving");
            return PartialView("BaggageReceivingIndex");
        }
        
    }
}