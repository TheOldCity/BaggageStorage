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
using BaggageStorage.Data;
using static BaggageStorage.Data.Models.PermissionEnums;


namespace BaggageStorage.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PermissionEnums", Name = "PermissionEnumsApi")]
    public class PermissionEnumsApiController : BaseController
    {
        /*public enum Permission
        {
            Signin,
            Menu_Administration,
            Menu_Owner,
            Menu_Roles,
            Menu_Permissions,
            Menu_Users,

            Menu_BaggageIn,
            Menu_BaggageOut,

            Menu_References,
                Menu_WorkingPlaces,
                Menu_StoragePlaces,
                Menu_BaggageStorage,

            Menu_Reports,
                Menu_RegistryBaggage,
                Menu_CashOperations,
                Menu_XReport,
                Menu_ZReport,
        }*/

        private class p
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        

        public PermissionEnumsApiController(AppDbContext db)
        {
            
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            string[] enums = Enum.GetNames(typeof(PermissionEnum));
            List<p> data = new List<p>();
            
            for(int i=0; i<enums.Length; i++)
            {
                data.Add(new p {Id= enums[i], Name=enums[i] });
            }

            return DataSourceLoader.Load(data, loadOptions);
        }
    }
}