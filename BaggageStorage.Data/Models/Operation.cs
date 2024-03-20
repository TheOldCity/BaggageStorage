using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaggageStorage.Data.Models
{
    public class Operation
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string OperationTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string EnumName { get; set; }

        public virtual OperationType OperationType { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}