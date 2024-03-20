using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using BaggageStorage.Classes;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/User", Name ="UserApi")]
    public class UserApiController : BaseController
    {

        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public class AnyUserRoles
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Enabled { get; set; }
        }

        public class OnlyId
        {
            public string Id { get; set; }
        }

        public class UpdateUser
        {
            public List<OnlyId> Roles { get; set; }
        }

        public class RoleInfo
        {
            public string RoleId { get; set; }
            public string UserId { get; set; }
            public string RoleName { get; set; }
        }

        public class UserPasswords
        {
            public UserPasswords()
            {
                Password = "";
                ConfirmPassword = "";
            }

            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public UserApiController(
            AppDbContext db, ILogger<UserApiController> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager
            )
        {
            _userManager = userManager;
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string customerId)
        {
            try
            {
                var model = _db.Users.Where(s => s.CustomerId == customerId)
                    .Select(b => new
                    {
                        Id = b.Id,
                        FirstName = b.FirstName,
                        LastName = b.LastName,
                        UserName = b.UserName,
                        Roles = _db.Roles.Where(s => _db.UserRoles.Any(r => r.RoleId == s.Id && r.UserId == b.Id)).Select(s => new RoleInfo { RoleId = s.Id, RoleName = s.Name, UserId = b.Id }).ToList(),
                        LockoutEnabled = b.LockoutEnabled,
                        Email = b.Email,
                        Name=$"{b.FirstName} {b.LastName}"
                    })
                    .ToList();

                return DataSourceLoader.Load(model, loadOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex));
            }
        }


        [HttpGet]
        [Route("/api/user/getrolesbyuserid", Name = "GetRolesByUserId")]
        public object GetRolesByUserId(string userId, string customerId)
        {
            try
            {
                var model = _db.Roles.Where(s => s.CustomerId == customerId).Select(s => new AnyUserRoles
                {
                    Id = s.Id,
                    Name = s.Name.Substring(s.Name.IndexOf("_")+1),
                    Enabled = false
                }).ToList();

                if (!User.IsInRole("developer"))
                {
                    model.Remove(model.FirstOrDefault(s => s.Name.ToUpper().Equals("DEVELOPER")));
                }

                var userRoles = _db.UserRoles.Where(s => s.UserId == userId).ToList();
                foreach (var role in userRoles)
                {
                    model.FirstOrDefault(s => s.Id == role.RoleId).Enabled = true;
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex,false));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            try
            {
                var newUser = new ApplicationUser();

                JsonConvert.PopulateObject(values, newUser);
                newUser.EmailConfirmed = true;
                newUser.LockoutEnd = DateTime.Now;

                if (!TryValidateModel(newUser))
                    return BadRequest(ModelState.GetFullErrorMessage());

                var listRoles = new UpdateUser();
                JsonConvert.PopulateObject(values, listRoles);

                listRoles.Roles = (listRoles.Roles == null) ? new List<OnlyId>() : listRoles.Roles;
                if (listRoles.Roles.Count == 0)
                    return BadRequest("Нобходимо указать роль пользователя");

                var passwords = new UserPasswords();
                JsonConvert.PopulateObject(values, passwords);

                if (passwords.Password.Equals(""))
                    return BadRequest("Нобходимо указать пароль");

                if (!passwords.Password.Equals(passwords.ConfirmPassword))
                    return BadRequest("Пароли не совпадают");

                if (newUser.LockoutEnabled)
                    newUser.LockoutEnd = DateTime.Now.AddYears(100);

                var res = await _userManager.CreateAsync(newUser, passwords.Password);
                if (!res.Succeeded)
                    return BadRequest(String.Join("\r\n", res.Errors.Select(d => d.Description).ToArray()));

                foreach (var r in listRoles.Roles)
                {
                    res = await _userManager.AddToRoleAsync(newUser, _db.Roles.FirstOrDefault(s => s.Id == r.Id).Name);
                    if (!res.Succeeded)
                        return BadRequest(String.Join("\r\n", res.Errors.Select(d => d.Description).ToArray()));
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex,false));
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(string key, string values)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(key);
                if (user != null)
                {
                    JsonConvert.PopulateObject(values, user);

                    if (!TryValidateModel(user))
                        return BadRequest(ModelState.GetFullErrorMessage());

                    if (user.LockoutEnabled)
                        user.LockoutEnd = DateTime.Now.AddYears(100);

                    var passwords = new UserPasswords();
                    JsonConvert.PopulateObject(values, passwords);

                    if (passwords.Password.Equals(passwords.ConfirmPassword) && !passwords.Password.Equals(""))
                    {
                        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, passwords.Password);
                    }

                    var listRoles = new UpdateUser();
                    JsonConvert.PopulateObject(values, listRoles);

                    listRoles.Roles = (listRoles.Roles == null) ? new List<OnlyId>() : listRoles.Roles;
                    if (listRoles.Roles.Count == 0)
                        return BadRequest("Нобходимо указать роль пользователя");

                    var roles = await _userManager.GetRolesAsync(user);
                    var res = await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

                    if (!res.Succeeded)
                        return BadRequest(String.Join("\r\n", res.Errors.Select(d => d.Description).ToArray()));

                    foreach (var r in listRoles.Roles)
                    {
                        var roleName = _db.Roles.FirstOrDefault(s => s.Id == r.Id).Name;
                        res = await _userManager.AddToRoleAsync(user, roleName);
                        if (!res.Succeeded)
                            return BadRequest(String.Join("\r\n", res.Errors.Select(d => d.Description).ToArray()));

                    }
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex,false));
            }

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string key)
        {
            try
            {
                var user = _db.Users.First(a => a.Id == key);

                _db.UserRoles.RemoveRange(_db.UserRoles.Where(s => s.UserId == user.Id));
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex,false));
            }

            return Ok();
        }
    }
}