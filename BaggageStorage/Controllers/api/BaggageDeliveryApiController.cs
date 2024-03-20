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
    [Route("api/BaggageDelivery", Name = "BaggageDeliveryApi")]
    public class BaggageDeliveryApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public BaggageDeliveryApiController(AppDbContext db, ILogger<BaggageDeliveryApiController> logger)
        {
            _logger = logger;
            _db = db;
        }


        private class Model
        {
            private BaggageMoving baggageMoving;
            private Client client;
            private StoragePlace storagePlace;

            public string id;
            public string firstName;
            public string phone;
            public string userInId;
            public string userOutId;
            public DateTime dateIn;
            public DateTime supposedDateOut;
            public DateTime dateOut;
            public decimal toPay;
            public string place;
            public int amountOfDays;
            public int amountOfHour;

            public Model()
            {
                place = "Null";
                firstName = "Null";
                phone = "0000000";
                userInId = "Null";
                userOutId = "Null";
                dateIn = DateTime.Now;
                supposedDateOut = DateTime.Now;
                dateOut = DateTime.Now;
                toPay = 0;
                amountOfDays = 0;
                amountOfHour = 0;
            }

            public Model(AppDbContext _db, string movingId)
            {
                baggageMoving = _db.BaggageMovings.Single(s => s.Id.Equals(movingId));
                client = _db.Clients.Single(s => s.Id.Equals(baggageMoving.ClientId));
                storagePlace = _db.StoragePlaces.Single(s => s.Id.Equals(baggageMoving.StoragePlaceId));

                firstName = client.LastName + " " + client.FirstName;
                phone = client.Phone;
                userInId = baggageMoving.UserInId;
                userOutId = baggageMoving.UserOutId;
                dateIn = baggageMoving.DateIn;
                supposedDateOut = baggageMoving.DateOut;
                place = storagePlace.Place;
                id = movingId;
                amountOfDays = Convert.ToInt32((supposedDateOut - dateIn).TotalDays);
                amountOfHour = Convert.ToInt32((supposedDateOut - dateIn).TotalHours);


                DateTime currentDate = DateTime.Now;
                int d = DateTime.Compare(currentDate, supposedDateOut);

                int dailyPrice = storagePlace.Price * Convert.ToInt32(baggageMoving.AmountOfPlaces);
                int hourlyPrice = storagePlace.HourlyPrice * Convert.ToInt32(baggageMoving.AmountOfPlaces);

                if (baggageMoving.UserOutId != null)
                {
                    dateOut = baggageMoving.DateOut;
                }

                if (d > 0)
                {
                    toPay = dailyPrice * Convert.ToInt32((currentDate - supposedDateOut).TotalDays) | hourlyPrice * Convert.ToInt32((currentDate - supposedDateOut).TotalHours);

                }
                else
                {
                    toPay = 0;

                }
            }
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string storageId, DateTime dateFrom, DateTime dateTo)
        {
            var isCalucateByDays = _db.Storages.Find(storageId).CashType == CashType.DailyPayment;
            

            var model = _db.BaggageMovings
                .AsNoTracking()
                .Where(s => s.StorageId == storageId && s.DateIn>=dateFrom && s.DateIn<=dateTo.AddDays(1))
                .Select(s => new Model
                {
                    firstName = s.Client.FirstName,
                    dateIn = s.DateIn,
                    dateOut = s.DateOut,
                    id = s.Id,
                    phone = s.Client.Phone,
                    userInId = s.UserInId,
                    userOutId = s.UserOutId,
                    place = s.StoragePlace.Place,
                    amountOfDays = s.AmountOfDays,
                    amountOfHour = s.AmountOfHour,
                    supposedDateOut = s.DateOut,
                    toPay = GetPayValue(isCalucateByDays, s.DateIn, s.StoragePlace.Price, s.AmountOfPlaces, s.AmountOfDays, s.StoragePlace.HourlyPrice, s.AmountOfHour, s.DateOut),

                })
                .ToList();

            return DataSourceLoader.Load(model, loadOptions);
        }

        [HttpPost]
        [Route("/api/baggagedelivery/post", Name = "Post")]
        async public Task<IActionResult> Post(string key, string toPay)
        {
            var bm = _db.BaggageMovings.Single(s => s.Id == key);

            try
            {
                CashOperation oper = new CashOperation
                {
                    Operation = TypeOperation.BaggageAdditionalPayment,
                    Amount = Convert.ToDecimal(toPay),
                    BaggageMoving = bm,
                    UserId = User.Identity.GetUserId(),
                    Date = DateTime.Now,
                    StorageId = bm.StorageId
                };

                await _db.CashOperations.AddAsync(oper);

                await _db.SaveChangesWithAudit();
            }
            catch (Exception ex)
            {
                return BadRequest(BaggageStorage.Classes.Utils.GetFullError(ex, false));
            }

            new ConsoleLogger("PUT", "/api/baggagedelivery").newDelivery(bm.Client.Phone, bm.DateIn);

            return Ok();
        }

        [HttpPut]
        [Route("/api/baggagedelivery/put", Name = "Put")]
        async public Task<IActionResult> Put(string key)
        {

            var bm = _db.BaggageMovings.Include(s => s.StoragePlace).Single(a => a.Id == key);

            bm.UserOutId = User.Identity.GetUserId();
            bm.DateOut = DateTime.Now;

            //// смотрим должен ли доплатить клиент и если да, то в Кассу вставляем строку об доплате
            //decimal amountAdd = GetPayValue(bm.DateIn, bm.StoragePlace.Price, bm.AmountOfPlaces, bm.AmountOfDays, bm.AmountOfHour, bm.StoragePlace.HourlyPrice);

            //if (amountAdd > 0)
            //{
            //    CashOperation oper = new CashOperation
            //    {
            //        Operation = TypeOperation.BaggageAdditionalPayment,
            //        Amount = amountAdd,
            //        BaggageMovingId = bm.Id,
            //        UserId = User.Identity.GetUserId(),
            //        Date = DateTime.Now,
            //        StorageId = bm.StorageId
            //    };

            //    await _db.CashOperations.AddAsync(oper);
            //}

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

        private decimal GetPayValue(bool isCalucateByDays, DateTime dateIn, decimal priceOneDay, int amountOfPlaces, int amountOfDays, int amountOfHour, decimal hourlyPrice, DateTime dateOut)
        {
            if (isCalucateByDays)
            {
                if (dateIn.AddDays(amountOfDays) >= DateTime.Now)
                    return 0;
                else
                {
                    DateTime d1 = dateIn.AddDays(amountOfDays);
                    DateTime d2 = DateTime.Now;


                    double delta = (d2 - d1).TotalDays;
                    int intdelta = Convert.ToInt32(Math.Truncate(delta));


                    // когда d1 = 21.01.2018 15:30 а d2 = 21.01.2018 17:30, то считаем что перелимит т.к.
                    // больше суток прошло и в этом случае считаем 1 сутки
                    if (delta > 0 && delta != Convert.ToDouble(intdelta))
                        delta = intdelta + 1;

                    return Convert.ToInt32(delta) * amountOfPlaces * priceOneDay;
                }

            }
            else
            {
                if (dateIn.AddHours(amountOfHour) >= DateTime.Now)
                    return 0;

                else
                {
                    DateTime d3 = dateIn.AddHours(amountOfHour);
                    DateTime d4 = DateTime.Now;

                    double alterDelta = (d4 - d3).TotalHours;
                    int intalterdelta = Convert.ToInt32(Math.Truncate(alterDelta));

                    if (alterDelta > 0 && alterDelta != Convert.ToDouble(intalterdelta))
                        alterDelta = intalterdelta + 1;

                    return Convert.ToInt32(alterDelta) * amountOfPlaces * hourlyPrice;
                }
            }
        }
    }
}