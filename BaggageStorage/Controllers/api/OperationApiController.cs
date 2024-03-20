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
    [Route("api/Operation", Name = "OperationApi")]
    public class OperationApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public OperationApiController(AppDbContext db, ILogger<OperationApiController> logger)
        {
            _logger = logger;
            _db = db;         
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var model = _db.Operations;

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var oper = new Operation();
            
            JsonConvert.PopulateObject(values, oper);

            if (!TryValidateModel(oper))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Operations.Add(oper);

            foreach(var role in _db.Roles.ToList())
            {
                _db.Permissions.Add(new Data.Models.Permission
                {
                    Operation = oper,
                    Permitted=false,
                    Role=role
                });
            }

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
            var oper = _db.Operations.First(a => a.Id == key);
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
            var permission = _db.Permissions.Where(s => s.OperationId == key);

            _db.Permissions.RemoveRange(permission);

            var oper = _db.Operations.First(a => a.Id == key);
            _db.Operations.Remove(oper);

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