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
    public class OperationTypeController : BaseController
    {
        private readonly AppDbContext _db;

        public OperationTypeController(AppDbContext db)
        {
            
            _db = db;
        }

        
        [HttpGet]
        [Route("~/operationtype", Name="OperationTypeIndex")]
        public ActionResult OperationTypeIndex()
        {
            //new ConsoleLogger("GET", "/operationtype");
            return PartialView();
        }
        
    }
}