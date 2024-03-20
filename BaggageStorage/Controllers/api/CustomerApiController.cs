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
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using BaggageStorage.Classes;
using Microsoft.Extensions.Logging;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Customer",Name = "CustomerApi")]
    public class CustomerApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public CustomerApiController(AppDbContext db, ILogger<CustomerApiController> logger)
        {
            _logger=logger;
            _db = db;         
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var custId = User.Identity.GetCustomerId();
            var model = _db.Customers.OrderBy(s=>s.Name).AsQueryable();

            if (!User.IsInRole("developer"))
            {
                model = model.Where(s => s.Id == custId || s.ParentId == custId).OrderBy(s=>s.Name);
            }
            var l = model.ToList();
            return DataSourceLoader.Load(l, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var newCustomer = new Customer
            {
                ParentId = User.Identity.GetCustomerId()
            };

            JsonConvert.PopulateObject(values, newCustomer);

            if (!TryValidateModel(newCustomer))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Customers.Add(newCustomer);

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

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var customer = _db.Customers.Single(a => a.Id == key);
            JsonConvert.PopulateObject(values, customer);

            if (!TryValidateModel(customer))
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
            var customer = _db.Customers.Single(a => a.Id == key);
            _db.Customers.Remove(customer);

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
    }
}