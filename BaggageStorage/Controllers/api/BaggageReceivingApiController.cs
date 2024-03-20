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
using Microsoft.EntityFrameworkCore;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/BaggageReceiving", Name = "BaggageReceivingApi")]
    public class BaggageReceivingApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public BaggageReceivingApiController(AppDbContext db, ILogger<BaggageReceivingApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        public object Get(DataSourceLoadOptions loadOptions, string storageId)
        {
            var dateFrom = new DateTime(2020, 1, 1);

            var model = _db.BaggageMovings
                .AsNoTracking()
                .Where(s => s.StorageId== storageId && s.DateIn >= dateFrom)
                .Select(s=> new {
                    Id=s.Id,
                    Amount = s.Amount,
                    AmountOfHour = s.AmountOfHour,
                    AmountOfDays = s.AmountOfDays,
                    AmountOfPlaces = s.AmountOfPlaces,
                    StorageId = s.StorageId,
                    ClientId = s.ClientId,
                    DateIn = s.DateIn,
                    DateOut=s.DateOut,
                    UserInId = s.UserInId,
                    UserOutId = s.UserOutId,
                    StoragePlaceId = s.StoragePlaceId,
                    PlaceId = s.StoragePlaceId, // вспомогательное поле
                    Price =s.StoragePlace.Price,
                    HourlyPrice=s.StoragePlace.HourlyPrice,
                    ClientName=$"{s.Client.FirstName} {s.Client.LastName}, {s.Client.Phone}"
                })
                .ToList();

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var bm = new BaggageMoving();

        JsonConvert.PopulateObject(values, bm);
            bm.DateIn = DateTime.Now;
            bm.UserInId = User.Identity.GetUserId();

            if (!TryValidateModel(bm))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                CashOperation oper = new CashOperation
                {
                    Operation=TypeOperation.BaggagePayment,
                    Amount = bm.Amount,
                    BaggageMoving = bm,
                    UserId = User.Identity.GetUserId(),
                    Date = DateTime.Now,
                    StorageId = bm.StorageId
                };

                await _db.BaggageMovings.AddAsync(bm);

                await _db.CashOperations.AddAsync(oper);
                
                await _db.SaveChangesWithAudit();

            }
            catch (Exception ex)
            {
                return BadRequest(BaggageStorage.Classes.Utils.GetFullError(ex, false));
            }

            new ConsoleLogger("POST", "/api/baggagereceiving").newBaggageReceiving(bm.Client.Phone, bm.StoragePlace.Place, bm.AmountOfPlaces, bm.AmountOfHour, bm.AmountOfPlaces);

            return Ok();
        }

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var bm = _db.BaggageMovings.Single(a => a.Id == key);
            JsonConvert.PopulateObject(values, bm);

            if (!TryValidateModel(bm))
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
            var bm = _db.BaggageMovings.Single(a => a.Id == key);
             var co = _db.CashOperations.Where(s => s.BaggageMovingId == bm.Id).ToList();

            if (co.Count() > 0)
            {
                return BadRequest("Ќельз€ удалить операцию т.к. был отпечатан фискальный чек.");
            }

            var bmJSON = Newtonsoft.Json.JsonConvert.SerializeObject(bm, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            BaggageMovingArchive bmArchive = Newtonsoft.Json.JsonConvert.DeserializeObject<BaggageMovingArchive>(bmJSON);
            bmArchive.UserDeletedId = User.Identity.GetUserId();

            _db.BaggageMovingArchives.Add(bmArchive);

            var coJSON = Newtonsoft.Json.JsonConvert.SerializeObject(co, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            List<CashOperationArchive> coArchive = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CashOperationArchive>>(coJSON);
            foreach (var item in coArchive)
            {
                item.UserDeletedId = User.Identity.GetUserId();
            }

            _db.CashOperationArchives.AddRange(coArchive);

            _db.CashOperations.RemoveRange(co);
            _db.BaggageMovings.Remove(bm);

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