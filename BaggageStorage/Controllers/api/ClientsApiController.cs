using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;

using BaggageStorage.Classes;
using BaggageStorage.Controllers;
using Microsoft.EntityFrameworkCore;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Clients", Name = "ClientsApi")]
    public class ClientsApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public ClientsApiController(AppDbContext db, ILogger<RoleApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var data = _db.Clients.AsNoTracking().Select(s => new
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Phone = s.Phone,
                Order=s.OrderId,
                Name = $"{s.FirstName} {s.LastName}, {s.Phone}"
            });
            
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var newClient = new Client();

            JsonConvert.PopulateObject(values, newClient);

            if (!TryValidateModel(newClient))
                return BadRequest(ModelState.GetFullErrorMessage());

            //if (newClient.OrderId == 0)
            //    newClient.OrderId = null;

            _db.Clients.Add(newClient);

            try
            {
                await _db.SaveChangesWithAudit();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            new ConsoleLogger("POST", "/api/clients").newClient(newClient.Phone, newClient.FirstName, newClient.LastName);

            return Json(new { order = newClient.OrderId });
        }

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var client = _db.Clients.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, client);


            if (!TryValidateModel(client))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                await _db.SaveChangesWithAudit();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }

        [HttpDelete]
        async public Task<IActionResult> Delete(string key)
        {
            try
            {
                var client = _db.Clients.First(a => a.Id == key);

                _db.Clients.Remove(client);

                try
                {                    
                    await _db.SaveChangesWithAudit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(Utils.GetFullError(ex));
                    return BadRequest(Utils.GetFullError(ex, false));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }
    }
}