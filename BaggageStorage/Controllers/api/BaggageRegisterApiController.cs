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
    [Route("api/BaggageRegister", Name = "BaggageRegisterApi")]
    public class BaggageRegisterApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public BaggageRegisterApiController(AppDbContext db, ILogger<BaggageRegisterApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string storageId, DateTime dateFrom, DateTime dateTo)
        {
            DateTime dtTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);
            var model = _db.BaggageMovings.Where(s => s.StorageId == storageId && s.DateIn >= dateFrom && s.DateIn <= dtTo)
                .Select(s => new BaggageRegister
                 {
                     ClientId = $"{s.Client.FirstName} {s.Client.LastName}, {s.Client.Phone}",
                     AmountOfDays = s.AmountOfDays,
                     StorageId = s.StorageId,
                     DateIn = s.DateIn,
                 }).ToList();

            return DataSourceLoader.Load(model ,loadOptions);
        }


        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var bs = new BaggageMoving();

            JsonConvert.PopulateObject(values, bs);

            if (!TryValidateModel(bs))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                _db.BaggageMovings.Add(bs);
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
            var bs = _db.BaggageMovings.Single(a => a.Id == key);
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
            var bs = _db.BaggageMovings.Single(a => a.Id == key);
            _db.BaggageMovings.Remove(bs);

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