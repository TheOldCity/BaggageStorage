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
    [Route("api/CashOperation", Name = "CashOperationApi")]
    public class CashOperationApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public CashOperationApiController(AppDbContext db, ILogger<CashOperationApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string storageId, DateTime dateFrom, DateTime dateTo)
        {
            DateTime dtTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);
            var model = _db.CashOperations.Where(s => s.StorageId == storageId && s.Date >= dateFrom && s.Date <= dtTo)
            //.Select(s => new
            //{
            //    Id = s.Id,
            //    Amount = s.Amount,
            //    Operation = s.Operation,
            //    StorageId = s.StorageId,
            //    Date = s.Date,
            //    UserId = s.UserId,
            //    BaggageMovingId = s.BaggageMovingId,
            //})
            .ToList();

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var co = new CashOperation();

        JsonConvert.PopulateObject(values, co);

            co.UserId = User.Identity.GetUserId();

            if (!TryValidateModel(co))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                _db.CashOperations.Add(co);
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
            var co = _db.CashOperations.Single(a => a.Id == key);
            JsonConvert.PopulateObject(values, co);

            if (!TryValidateModel(co))
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
            var co = _db.CashOperations.Single(a => a.Id == key);
            _db.CashOperations.Remove(co);

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