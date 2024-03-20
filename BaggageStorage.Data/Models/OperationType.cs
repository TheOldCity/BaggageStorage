using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace BaggageStorage.Data.Models
{
    public class OperationType
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Operation> Operations { get; set; }
    }
}