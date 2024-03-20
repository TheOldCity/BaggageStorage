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
    [Route("api/StoragePlaces", Name = "StoragePlacesApi")]
    public class StoragePlacesApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public StoragePlacesApiController(AppDbContext db, ILogger<StoragePlacesApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string storageId)
        {
            decimal d=0;
            var model = _db.StoragePlaces.Where(s => s.StorageId == storageId).OrderBy(s=>Convert.ToInt32(s.Place));
            List<Object> result = new List<Object>();

            foreach(var item in model)
            {
                result.Add(new
                {
                    StorageId = item.StorageId,
                    HourlyPrice = item.HourlyPrice,
                    Place = (Decimal.TryParse(item.Place, out d) ? Convert.ToDecimal(item.Place) : 0),
                    Id = item.Id,
                    Price = item.Price
                });
            }

            return DataSourceLoader.Load(result.ToList(), loadOptions);
        }

        [HttpGet]
        [Route("/api/storageplaces/getfreeplaces", Name = "GetFreePlaces")]
        public object GetFreePlaces(DataSourceLoadOptions loadOptions, string storageId)
        {
            decimal d = 0;
            var model = _db.StoragePlaces.Where(s => s.StorageId == storageId).OrderBy(s => Convert.ToInt32(s.Place));
            List<Object> result = new List<Object>();

            foreach (var item in model.ToList())
            {
                // смотрим место свободное или нет
                var count=_db.BaggageMovings.Count(s => s.StoragePlaceId == item.Id && s.DateOut < new DateTime(2000,1,1));
                if (count == 0)
                {

                    result.Add(new
                    {
                        StorageId = item.StorageId,
                        HourlyPrice = item.HourlyPrice,
                        Place = (Decimal.TryParse(item.Place, out d) ? Convert.ToDecimal(item.Place) : 0),
                        Id = item.Id,
                        Price = item.Price
                    });
                }
            }

            return DataSourceLoader.Load(result.ToList(), loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var sp = new StoragePlace();

            JsonConvert.PopulateObject(values, sp);

            if (!TryValidateModel(sp))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                _db.StoragePlaces.Add(sp);
                await _db.SaveChangesWithAudit();
            }
            catch (Exception ex)
            {
                return BadRequest(BaggageStorage.Classes.Utils.GetFullError(ex, false));
            }

            new ConsoleLogger("POST", "/api/storageplaces").newPlace(sp.Place, sp.Price, sp.HourlyPrice);

            return Ok();
        }

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var bs = _db.StoragePlaces.Single(a => a.Id == key);
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
            var sp = _db.StoragePlaces.Single(a => a.Id == key);
            _db.StoragePlaces.Remove(sp);

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