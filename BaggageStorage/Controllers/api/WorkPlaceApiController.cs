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
    [Route("api/WorkPlace",Name = "WorkPlaceApi")]
    public class WorkPlaceApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public WorkPlaceApiController(AppDbContext db, ILogger<WorkPlaceApiController> logger)
        {
            _logger=logger;
            _db = db;         
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string customerId)
        {
            var model = _db.WorkPlaces.Where(s => s.CustomerId == customerId).ToList();

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var workPlace = new WorkPlace();
            

            JsonConvert.PopulateObject(values, workPlace);

            if (!TryValidateModel(workPlace))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.WorkPlaces.Add(workPlace);

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
            var workPlace = _db.WorkPlaces.Single(a => a.Id == key);
            JsonConvert.PopulateObject(values, workPlace);

            if (!TryValidateModel(workPlace))
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
            var workPlace = _db.WorkPlaces.Single(a => a.Id == key);
            _db.WorkPlaces.Remove(workPlace);

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