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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;

using BaggageStorage.Classes;
using BaggageStorage.Controllers;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Role", Name = "RoleApi")]
    public class RoleApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;

        public RoleApiController(AppDbContext db, ILogger<RoleApiController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string customerId)
        {
            if (customerId == null)
                customerId = User.Identity.GetCustomerId();

            var roles = _db.Roles.Where(r => r.CustomerId == customerId ).ToList();
            
            if (!(User.IsInRole("developer")))
            {
                var dev = roles.FirstOrDefault(f => f.NormalizedName == "DEVELOPER");
                if (dev != null)
                    roles.Remove(dev);
            }

            return DataSourceLoader.Load(roles, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var newRole = new ApplicationRole();

            JsonConvert.PopulateObject(values, newRole);

            newRole.Name = $"{newRole.CustomerId}_{newRole.Alias}";
            
            newRole.NormalizedName = newRole.Name.ToUpper();
            newRole.UserId = User.Identity.GetUserId(); // пользователь, кто создал!

            if (!TryValidateModel(newRole))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Roles.Add(newRole);

            foreach (var oper in _db.Operations.ToList())
            {
                _db.Permissions.Add(new BaggageStorage.Data.Models.Permission
                {
                    Operation = oper,
                    Role = newRole,
                    Permitted = false
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
            var role = _db.Roles.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, role);

            // если роль Developer то для неё не подменяем название
            if (!role.Name.ToLower().Equals("developer"))
            {
                role.Name = $"{role.CustomerId}_{ role.Alias}";
                role.NormalizedName = role.Name.ToUpper();
            }

            if (!TryValidateModel(role))
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
            try
            {
                var role = _db.Roles.First(a => a.Id == key);
                var permissions = _db.Permissions.Where(s => s.RoleId == role.Id);
                _db.Permissions.RemoveRange(permissions);

                _db.Roles.Remove(role);
                try
                {
                    await _db.SaveChangesWithAudit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(Utils.GetFullError(ex));
                    return BadRequest(Utils.GetFullError(ex, false));
                }
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