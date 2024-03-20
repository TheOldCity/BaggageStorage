using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaggageStorage.DataLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using BaggageStorage.Classes;
using BaggageStorage.Data;
using Microsoft.Extensions.Logging;
using BaggageStorage.Data.Models;

namespace BaggageStorage.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            
        }

        // обрабатываем событие, для логирования HTTP запросов
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string currentAction = "";
            string currentController = "";
            string requestPath = "";
            string requestPostParam = "";
            string method = "";
            string requesrUserAgent = "";
            string userIp = "";

            IConfiguration configuration = Startup.ServiceProvider.GetService<IConfiguration>();
            LogDbContext logDb = Startup.ServiceProvider.GetService<LogDbContext>();
            AppDbContext db = Startup.ServiceProvider.GetService<AppDbContext>();
            ILogger logger= (Startup.ServiceProvider.GetService<ILoggerFactory>()).CreateLogger("Controller_OnActionExecutionAsync");

            // если стоит в настройках признак, что нужно логировать и пользователь аутентифицирован
            if (configuration.GetSection("AppConfigurations").GetValue<int>("UseHttpLogging") == 1 && User.Identity.IsAuthenticated)
            {

                currentAction = ControllerContext.RouteData.Values["action"].ToString();
                currentController = ControllerContext.RouteData.Values["controller"].ToString();

                requestPath = Request.Path;
                requestPostParam = ((Request.ContentType == null)) ? null : Newtonsoft.Json.JsonConvert.SerializeObject(Request.Form);

                method = Request.Method;
                requesrUserAgent = Request.Headers["User-Agent"].ToString();
                userIp = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();

                // записываем информация о запросе в логи
                logDb.Logs.Add(new DataLog.Models.Log
                {
                    Action = currentAction,
                    Controller = currentController,
                    Date = DateTime.Now,
                    RawUrl = requestPath,
                    RequestPostParams = requestPostParam,
                    UserAgent = requesrUserAgent,
                    UserIp = userIp,
                    UserId = User.Identity.GetUserId(),
                    EventType = DataLog.Models.EventTypes.RetailMasterHttpLogging,
                    Message = "",
                    Method = method
                });
                logDb.SaveChangesAsync();




                // в таблице UserConnections обновляем поля LastRequestAction, LastRequestController
                string sessionId = User.Identity.GetSessionId();
                var conn = db.UserConnection.SingleOrDefault(s => s.SessionId == sessionId);

                if (conn == null)
                {
                    logger.LogCritical($"В UserConnection отсутсвует сессия {sessionId}");
                    db.UserConnection.Add(new UserConnection
                    {
                        IsOnline = true, // специально ставим, чтобы убедится, когда браузер установит соединение с Websocket, внутри WebSocketMiddleware сделаем update на true
                        SessionId = sessionId,
                        IsRemember = User.Identity.GetIsRememberMe(),
                        UserId = User.Identity.GetUserId(),
                        UserIp = userIp,
                        UserAgent = requesrUserAgent,
                        LastRequestAction = currentAction,
                        LastRequestController = currentController,
                        LastRequestDate = DateTime.UtcNow,
                        LastRequestPostParams = String.Empty,
                        LastRequestRawUrl = requestPath ?? String.Empty,
                    });
                }
                else
                {
                    conn.LastRequestController = currentController;
                    conn.LastRequestAction = currentAction;
                    conn.LastRequestDate = DateTime.UtcNow;
                    conn.LastRequestPostParams = requestPostParam;
                    conn.UserAgent = requesrUserAgent;
                    conn.UserIp = userIp;
                    conn.LastRequestRawUrl = requestPath;
                    conn.UserId = User.Identity.GetUserId();
                    conn.IsRemember = User.Identity.GetIsRememberMe();
                }

                db.SaveChangesAsync();
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}
