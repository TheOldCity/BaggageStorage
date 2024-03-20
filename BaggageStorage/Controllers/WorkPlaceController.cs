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
    public class WorkPlaceController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<WorkPlaceController> _localizer;

        public WorkPlaceController(AppDbContext db, IStringLocalizer<WorkPlaceController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/workplace", Name = "WorkPlaceIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/workplaces");
            return PartialView("WorkPlaceIndex");
        }
        
    }
}