using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BaggageStorage.Classes;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/MainMenu",Name = "MainMenuApi")]
    public class MainMenuApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public MainMenuApiController(AppDbContext db, ILogger<MainMenuApiController> logger)
        {
            _logger = logger;
            _db = db;         
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {            
            var menu = _db.MainMenu.Select(s => new {
                Id=s.Id,
                ParentId=s.ParentId,
                Text=s.Text,
                TextRo=s.TextRo,
                TextEn = s.TextEn,
                Icon =s.Icon,
                JsFunction=s.JsFunction,
                Location = s.Location,
                OrderId=s.OrderId,
                IsActive= s.IsActive,
                PermissionEnumText = s.PermissionEnumText,
                HasItems = _db.MainMenu.Count(m=> m.ParentId== s.Id) > 0
            }).OrderBy(s=>s.ParentId).ThenBy(s=>s.OrderId);

            var list = menu.ToList();
            return DataSourceLoader.Load(menu, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var menuItem = new MainMenu();

            JsonConvert.PopulateObject(values, menuItem);

            if (!TryValidateModel(menuItem))
                return BadRequest(ModelState.GetFullErrorMessage());

            // вычисляем OrderId
            menuItem.OrderId = _db.MainMenu.Where(s => s.ParentId == menuItem.ParentId).Count() + 1;

            _db.MainMenu.Add(menuItem);

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
        async public Task<IActionResult> Put(int key, string values)
        {
            var menuItem = _db.MainMenu.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, menuItem);

            if (!TryValidateModel(menuItem))
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
        async public Task<IActionResult> Delete(int key)
        {
            var children= _db.MainMenu.Where(s => s.ParentId == key).ToList();

            foreach(var child in children)
            {
                await Delete(child.Id);
            }            

            var menuItem = _db.MainMenu.Where(s => s.Id == key);

            _db.MainMenu.RemoveRange(menuItem);
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

        [HttpPost]        
        [Route("/api/mainmenu/reorder")]
        async public Task<IActionResult> Reorder(int draggingRowKey, int targetRowKey, bool shiftPressed)
        {
            var target = _db.MainMenu.Find(targetRowKey);
            var source = _db.MainMenu.Find(draggingRowKey);

            if (shiftPressed)
            {
                source.ParentId = target.Id;
                source.OrderId = _db.MainMenu.Count(s => s.ParentId == target.Id);
            }
            else
            {

                int targetOrderId = target.OrderId;
                int sourceOrderId = source.OrderId;

                int targetParentId = target.ParentId;

                if (targetOrderId < sourceOrderId)
                {
                    var reorderedList = _db.MainMenu.Where(s => s.ParentId == targetParentId && s.OrderId >= targetOrderId).ToList();

                    foreach (var item in reorderedList)
                    {
                        item.OrderId++;
                    }
                }
                else
                {
                    var reorderedList = _db.MainMenu.Where(s => s.ParentId == targetParentId && s.OrderId <= targetOrderId).ToList();

                    foreach (var item in reorderedList)
                    {
                        item.OrderId--;
                    }
                }

                source.OrderId = targetOrderId;
                source.ParentId = targetParentId;
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

        [HttpPost]
        [Route("/api/mainmenu/changeIcon")]
        async public Task<IActionResult> changeIcon(string cellIdForIcon, string result)
        {
            var id = Int32.Parse(cellIdForIcon);

            var allData = (from c in _db.MainMenu
                         where c.Id == id
                         select c).First();

            allData.Icon = result.ToString();

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