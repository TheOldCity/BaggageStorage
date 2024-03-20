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
using Microsoft.AspNetCore.Identity;
using BaggageStorage.Data;
using BaggageStorage.Data.Models;
using BaggageStorage.Classes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Permission", Name = "PermissionApi")]
    public class PermissionApiController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public PermissionApiController(
            AppDbContext db, ILogger<PermissionApiController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
        }

      
        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions, string customerId)
        {
            try
            {
                var model = _db.Permissions.ToList();

                // ����������� �� ���� ��������� � ���� ���� �������� ��� � ������ Permissions , �� ��������� ��
                var roles = _db.Roles.Where(s => s.CustomerId == customerId);

                foreach (var role in roles)
                {
                    foreach (var oper in _db.Operations)
                    {
                        if (model.FirstOrDefault(s => s.OperationId == oper.Id && s.RoleId == role.Id) == null)
                        {
                            var permitted = (role.NormalizedName == "DEVELOPER");
                            _db.Permissions.Add(new Permission
                            {
                                OperationId = oper.Id,
                                Permitted = permitted,
                                RoleId = role.Id
                            });
                        }
                    }
                    _db.SaveChanges();
                }

                var result = _db.Permissions.Select(s => new
                {
                    Id = s.Id,
                    RoleId = s.RoleId,
                    Permitted = (s.Role.NormalizedName.Equals("DEVELOPER")) ? true : s.Permitted,
                    OperationId = s.OperationId,
                    OperationTypeId = s.Operation.OperationTypeId
                }).Where(s=>roles.Any(r=>r.Id==s.RoleId)).ToList();

                // ���� ��� �� Developer �� ������� �������� �� ������� � ������������� (���� �� ���� ����� �� ���������� ����) ��� ����.
                // ����� �� ����������, ��� �� �������� ���� ����� �� ����� ������ � ����� ���������� ����, � ������������ ����� ����� � ��� ���� ��� �������������� �����
                if (!User.IsInRole("developer"))
                {
                    // �������� �������� �� ������� � �������� ������������ ����������� ����� Permitted
                    var userRoles = _db.UserRoles.Where(s => s.UserId == User.Identity.GetUserId());
                    var userOperations = _db.Permissions.Where(s => userRoles.Any(t => t.RoleId == s.RoleId) && s.Permitted == true).Select(s => s.OperationId).Distinct();
                    result = result.Where(s => userOperations.Any(t => t == s.OperationId)).ToList();
                }

                

                return DataSourceLoader.Load(result, loadOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex,false));
            }
        }

        [HttpPut]
        public IActionResult Put(string key, string values, string customerId)
        {
            try
            {
                var permissions = _db.Permissions.Include(s => s.Role).First(a => a.Id == key);

                if (permissions.Role.NormalizedName.Equals("DEVELOPER"))
                    return BadRequest("������ ������ ����� � ��������� �������");

                JsonConvert.PopulateObject(values, permissions);

                if (!TryValidateModel(permissions))
                    return BadRequest(ModelState.GetFullErrorMessage());


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