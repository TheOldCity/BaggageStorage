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
    [Route("api/OperationType", Name = "OperationTypeApi")]
    public class OperationTypeApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public OperationTypeApiController(AppDbContext db, ILogger<OperationTypeApiController> logger)
        {
            _logger = logger;
            _db = db;         
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var model = _db.OperationTypes;

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var oper = new OperationType();
            
            JsonConvert.PopulateObject(values, oper);

            if (!TryValidateModel(oper))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.OperationTypes.Add(oper);

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
            var oper = _db.OperationTypes.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, oper);

            if (!TryValidateModel(oper))
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
        async public Task<ActionResult> Delete(string key)
        {
            var oper = _db.OperationTypes.First(a => a.Id == key);
            _db.OperationTypes.Remove(oper);

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