using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using BaggageStorage.Models.UserViewModels;
using BaggageStorage.Classes;
using Microsoft.Extensions.Logging;

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<UserController> _localizer;
        private readonly ILogger _logger;

        public UserController(IStringLocalizer<UserController> localizer,
            AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<UserController> logger)
        {
            _logger = logger;
            _localizer = localizer;
            _userManager = userManager;
            _db = db;
        }

        #region Users

        [HttpGet]
        [Route("~/user", Name = "UserIndex")]
        public ActionResult UserIndex()
        {
            return PartialView();
        }

        [HttpGet]
        [Route("~/user/profile", Name = "UserProfile")]
        async public Task<IActionResult> UserProfile()
        {
            var model = new UserViewModel();
            var user = (await _userManager.FindByNameAsync(User.Identity.Name));

            if (user == null)
            {
                throw new Exception("Пользователь не найден");
            }

            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.CustomerId = user.CustomerId;

            return PartialView("UserProfile", model);
        }

        [HttpPost]
        [Route("~/user/save", Name = "UserSave")]
        async public Task<IActionResult> UserSave(UserViewModel model)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    if (!model.Password.Equals(model.ConfirmPassword))
                        throw new Exception(_localizer["Пароли не совпадают"].Value);
                }

                
                var user = _db.Users.Single(x => x.Id == model.Id);

                if (user == null)
                {
                    throw new Exception(_localizer["Пользователь не найден"].Value);
                }


                if (!String.IsNullOrWhiteSpace(model.Password) && model.Password.Equals(model.ConfirmPassword))
                {
                    user.PasswordHash=_userManager.PasswordHasher.HashPassword(user, model.Password);
                }

                

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.CustomerId = model.CustomerId;

                await _db.SaveChangesWithAudit();

                
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }
        #endregion

        [HttpGet]
        [Route("~/user/{customerId:guid}/listroles", Name = "CustomerRoles")]
        public ActionResult CustomerRoles(string customerId)
        {
            var model = _db.Roles.Where(s => s.CustomerId == customerId).ToList();
            //new ConsoleLogger("GET", "/user/{customerId:guid}/listroles");

            return PartialView("CustomerRoles", model);
        }

    }
}