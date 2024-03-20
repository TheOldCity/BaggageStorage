using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace BaggageStorage.Data.Models
{
    public class Customer
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ParentId { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(254)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<ApplicationRole> Roles { get; set; }
        public virtual ICollection<Storage> Storages { get; set; }
        public virtual ICollection<WorkPlace> WorkPlaces { get; set; }
    }
}
