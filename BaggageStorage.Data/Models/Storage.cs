using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaggageStorage.Data.Models
{
    public class Storage
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ShortName { get; set; }

        public string CustomerId { get; set; }

        public CashType CashType { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<StoragePlace> StoragePlaces { get; set; }
        public virtual ICollection<BaggageMoving> BaggageMovings { get; set; }
        public virtual ICollection<BaggageMovingArchive> BaggageMovingArchives { get; set; }
        public virtual ICollection<CashOperation> CashOperations { get; set; }        
        public virtual ICollection<WorkPlace> WorkPlaces { get; set; }
    }
}