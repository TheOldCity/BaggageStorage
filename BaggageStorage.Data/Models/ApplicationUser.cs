using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaggageStorage.Data.Models
{

    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public bool RememberMe { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<BaggageMoving> UsersIn { get; set; }
        public virtual ICollection<BaggageMoving> UsersOut { get; set; }
        public virtual ICollection<CashOperation> CashOperations { get; set; }
    }
}
