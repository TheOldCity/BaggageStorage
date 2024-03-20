using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using BaggageStorage.Data;
using BaggageStorage.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BaggageStorage.Controllers.Api.PermissionEnumsApiController;
using BaggageStorage.Data.Models;
using static BaggageStorage.Data.Models.PermissionEnums;

namespace BaggageStorage.Classes
{
    public class Utils
    {

        public static List<MenuItem> GetUserMenu(HttpContext context, AppDbContext db)
        {
            string lang = "ru";
            var langCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];

            if (langCookie != null)
            {
                string[] arr = langCookie.Split('|');
                lang = (arr[0].Split('='))[1];
            }

            List<MenuItem> menu = new List<MenuItem>();


            var rootItems = db.MainMenu.Where(s => s.ParentId == 0 && s.IsActive).OrderBy(s => s.OrderId).ToList();

            foreach (var item in rootItems)
            {
                switch (lang)
                {
                    case "ro":
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.TextRo,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                    case "en":
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.TextEn,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                    default:
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.Text,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                }
            }

            var items = db.MainMenu.Where(s => s.ParentId != 0 && s.IsActive).OrderBy(s => s.OrderId).ToList();

            bool isFinish;

            do
            {
                isFinish = true;
                foreach (var item in items.ToList())
                {
                    // смотрим если item уже присутствует в нашем menu то берем следующий
                    if (IsItemExists(menu, item.Id))
                        continue;

                    var parentItem = GetParentItem(menu, item.ParentId);
                    if (parentItem == null)
                    {
                        isFinish = false;
                    }
                    else
                    {
                        switch (lang)
                        {
                            case "ro":
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.TextRo,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                            case "en":
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.TextEn,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                            default:
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.Text,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                        }

                    }
                }
            } while (!isFinish);



            // удаляем пункт меню РАЗРАБОТЧИК если пользователь не Developer
            if (!context.User.IsInRole("developer"))
            {
                var devMenuItem = menu.Where(s => s.id == 1).FirstOrDefault();
                menu.Remove(devMenuItem);
            }

            return menu;
        }

        public static string GetFullError(Exception ex, bool showCallStack = true)
        {
            StringBuilder result = new StringBuilder();
            if (ex != null)
            {
                do
                {
                    result.AppendLine(ex.Message);
                    if (ex is PostgresException)
                        result.AppendLine((ex as PostgresException).Detail);

                    if (showCallStack)
                        result.Append(ex.StackTrace);

                    ex = ex.InnerException;

                }
                while (ex != null);
            }

            return result.ToString().Trim();
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            if (arrBytes == null || arrBytes.Length==0)
                return null;

            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public static bool CheckPermission(AppDbContext db, ClaimsPrincipal user, string permissionEnumText, ILogger logger)
        {
            if (user.IsInRole("developer"))
                return true;

            if (permissionEnumText == null)
                return false;

            try
            {
                var operationId = db.Operations.FirstOrDefault(s => s.EnumName.Equals(permissionEnumText)).Id;
                var userRoles = db.UserRoles.Where(s => s.UserId == user.Identity.GetUserId()).Select(s => new { s.RoleId }).ToList();

                return db.Permissions.Any(s => userRoles.Any(r => r.RoleId == s.RoleId) && s.OperationId == operationId && s.Permitted == true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CheckPermission for user: {user.Identity.GetUserName()}, permissionEnum: {permissionEnumText}." + Utils.GetFullError(ex));
                return false;
            }
        }

        public static bool CheckPermission(AppDbContext db, ClaimsPrincipal user, PermissionEnum permission)
        {
            if (user.IsInRole("developer"))
                return true;

            var permissionEnumText = Enum.GetName(typeof(PermissionEnum), permission);

            var operationId = db.Operations.FirstOrDefault(s => s.EnumName.Equals(permissionEnumText)).Id;
            var userRoles = db.UserRoles.Where(s => s.UserId == user.Identity.GetUserId()).Select(s => new { s.RoleId }).ToList();

            return db.Permissions.Any(s => userRoles.Any(r => r.RoleId == s.RoleId) && s.OperationId == operationId);
        }

        #region Private Methods
        private static bool IsItemExists(List<MenuItem> menu, int id)
        {
            bool result = false;

            foreach (var item in menu)
            {
                if (item.id == id)
                {
                    result = true;
                    break;
                }

                if (item.items.Count > 0)
                {
                    result = IsItemExists(item.items, id);
                    if (result)
                        break;
                }
            }

            return result;
        }

        private static MenuItem GetParentItem(List<MenuItem> menu, int parentId)
        {
            MenuItem result = null;

            foreach (var item in menu)
            {
                if (item.id == parentId)
                {
                    result = item;
                    break;
                }

                if (item.items.Count > 0)
                {
                    result = GetParentItem(item.items, parentId);
                    if (result != null)
                        break;
                }
            }

            return result;
        }
        #endregion
    }
}
