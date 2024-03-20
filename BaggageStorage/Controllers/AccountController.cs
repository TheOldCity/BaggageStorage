using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using BaggageStorage.Models.ViewModels.AccountViewModels;
using BaggageStorage.Classes;
using System.Security.Principal;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BaggageStorage.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private AppDbContext _db;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly ILogger<AccountController> _logger;


        public AccountController(IStringLocalizer<AccountController> localizer, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> usermanager,
            ILogger<AccountController> logger, AppDbContext db, IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
            _logger = logger;
            _localizer = localizer;
            _usermanager = usermanager;
            _db = db;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                return new RedirectToActionResult("Home", "Index", null);
            else
            {
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            String error = "";
            
            ViewData["ReturnUrl"] = returnUrl;

            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _usermanager.FindByNameAsync(model.Login);

                    if (user == null)
                    {
                        user = await _usermanager.FindByEmailAsync(model.Login);
                    }

                    if (user == null)
                    {
                        error = _localizer["Имя пользователя или пароль не верный"].Value;
                    }
                    else
                    {
                        user.RememberMe = model.RememberMe;

                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                        if (!result.Succeeded)
                        {
                            if (result.IsLockedOut)
                            {
                                error = _localizer["Ваш аккаунт заблокирован. Обратитесь к администратору"].Value;
                            }

                            else
                            {
                                error = _localizer["Имя пользователя или пароль не верный"].Value;
                            }
                        }
                        else
                        {
                            _db.UserConnection.Add(new UserConnection
                            {
                                IsOnline = false, // специально делаем, чтобы убедится, когда брайзер установит соединение с Websocket, внутри WebSocketMiddleware сделаем update на true
                                SessionId = HttpContext.Session.Id,
                                IsRemember = model.RememberMe,
                                UserId = user.Id,
                                UserIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                                LastRequestAction = "Login",
                                LastRequestController = "AccountController",
                                LastRequestDate = DateTime.Now,
                                LastRequestPostParams = String.Empty,
                                LastRequestRawUrl = returnUrl ?? String.Empty,
                            });
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                else
                    error = _sharedLocalizer["bad_input_params"].Value;
            }
            catch(Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                error= _sharedLocalizer["InternalServerError"].Value;
            }

            new ConsoleLogger("POST", "/account/login").login(model.Login, model.Password, model.RememberMe);

            return Json(new
            {
                error = error,
                redirect = (returnUrl == null) ? "" : returnUrl
            });
        }

        [HttpGet]
        async public Task<IActionResult> Logout()
        {
            var conn = _db.UserConnection.FirstOrDefault(s => s.SessionId == User.Identity.GetSessionId());
            if (conn != null)
            {
                _db.UserConnection.Remove(conn);
                await _db.SaveChangesAsync();
            }

            Request.HttpContext.Session.Clear();
            Request.HttpContext.Response.Cookies.Delete(".AspNetCore.Session");

            await _signInManager.SignOutAsync();

            new ConsoleLogger("GET", "/account/logout");

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
