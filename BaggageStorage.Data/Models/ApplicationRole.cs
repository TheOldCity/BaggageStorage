using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BaggageStorage.Data.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string UserId { get; set; } // кто создал роль

        [Required]
        public string Alias { get; set; }   // здесь будем хранить название роли, а в поле NAME и NORMALIZEDNAME будем хранить название в виде префикс_alias

        public virtual Customer Customer { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}