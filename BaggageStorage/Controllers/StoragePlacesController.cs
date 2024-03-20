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
    public class StoragePlacesController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<StoragePlacesController> _localizer;

        public StoragePlacesController(AppDbContext db, IStringLocalizer<StoragePlacesController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/storageplaces", Name = "StoragePlacesIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/storageplaces");
            return PartialView("StoragePlacesIndex");
        }
        
    }
}