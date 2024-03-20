using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BaggageStorage.Models.UserViewModels
{
    public class ListRoleViewModel
    {
        public List<IdentityRole> Roles { get; set; }
    }
}