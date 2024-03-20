using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


using Microsoft.EntityFrameworkCore;
using BaggageStorage.Data;
using BaggageStorage.Classes;

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class PermissionController : BaseController
    {
        private readonly AppDbContext _db;
        

        public PermissionController(AppDbContext db)
        {
            
            _db = db;
        }

        
        [HttpGet]
        [Route("~/permission", Name = "PermissionIndex")]
        public ActionResult PermissionIndex()
        {
            //new ConsoleLogger("GET", "/permission");
            return PartialView();
        }

        [HttpGet]
        [Route("~/permission/{customerId}/list", Name = "PermissionList")]
        public ActionResult PermissionList(string customerId)
        {
            return PartialView("PermissionList", customerId);
        }

    }
}