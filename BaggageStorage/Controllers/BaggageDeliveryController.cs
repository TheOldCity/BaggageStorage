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
    public class BaggageDeliveryController : BaseController
    {
        public class DateRange
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<BaggageDeliveryController> _localizer;

        public BaggageDeliveryController(AppDbContext db, IStringLocalizer<BaggageDeliveryController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/baggagedelivery", Name = "BaggageDeliveryIndex")]
        public IActionResult BaggageDeliveryIndex()
        {
            
            var dateTo = DateTime.Now.Date;
            var dateFrom = dateTo.AddDays(-30);

            DateRange model = new DateRange
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            };

            //new ConsoleLogger("GET", "/baggagedelivery");

            return PartialView("BaggageDeliveryIndex", model);
        }
        
    }
}