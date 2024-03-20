using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaggageStorage.Classes;

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        [Route("~/role", Name = "RoleIndex")]
        public ActionResult RoleIndex()
        {
            //new ConsoleLogger("GET", "/role");
            return PartialView();
        }


    }
}