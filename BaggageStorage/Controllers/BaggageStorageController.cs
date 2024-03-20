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
    public class BaggageStorageController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<BaggageStorageController> _localizer;

        public BaggageStorageController(AppDbContext db, IStringLocalizer<BaggageStorageController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/baggagestorage", Name = "BaggageStorageIndex")]
        public IActionResult Index()
        {
            // new ConsoleLogger("GET", "/baggagestorage");
            return PartialView("BaggageStorageIndex");
        }
        
    }
}