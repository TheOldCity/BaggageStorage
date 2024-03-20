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
    public class CashOperationController : BaseController
    {
        private readonly AppDbContext _db;
        
        private readonly IStringLocalizer<CashOperationController> _localizer;

        public CashOperationController(AppDbContext db, IStringLocalizer<CashOperationController> localizer)
        {
            
            _db = db;
            _localizer = localizer;
        }

        
        [HttpGet]
        [Route("~/cashoperation", Name = "CashOperationIndex")]
        public IActionResult Index()
        {
            //new ConsoleLogger("GET", "/cashoperation");
            return PartialView("CashOperationIndex");
        }
        
    }
}