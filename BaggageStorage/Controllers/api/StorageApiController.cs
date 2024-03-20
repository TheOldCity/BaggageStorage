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
using BaggageStorage.Data.Models;
using BaggageStorage.Classes;
using BaggageStorage.Data;
using Microsoft.Extensions.Logging;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/baggagestorage", Name = "StorageApi")]
    public class StorageApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public StorageApiController(AppDbContext db, ILogger<StorageApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string customerId)
        {
            var model = _db.Storages.Where(s => s.CustomerId == customerId).ToList();

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var bs = new Storage();

        JsonConvert.PopulateObject(values, bs);

            if (!TryValidateModel(bs))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                _db.Storages.Add(bs);
                await _db.SaveChangesWithAudit();
            }
            catch (Exception ex)
            {
                return BadRequest(BaggageStorage.Classes.Utils.GetFullError(ex, false));
            }

            return Ok();
        }

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var bs = _db.Storages.Single(a => a.Id == key);
            JsonConvert.PopulateObject(values, bs);

            if (!TryValidateModel(bs))
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
            var bs = _db.Storages.Single(a => a.Id == key);
            _db.Storages.Remove(bs);

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