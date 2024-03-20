using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BaggageStorage.Data;
using BaggageStorage.Classes;

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class OperationController : BaseController
    {
        private readonly AppDbContext _db;
        


        public OperationController(AppDbContext db)
        {
            
            _db = db;
        }

        
        [HttpGet]
        [Route("~/operation", Name="OperationIndex")]
        public ActionResult OperationIndex()
        {
            //new ConsoleLogger("GET", "/operation");
            return PartialView();
        }
        
        
    }
}