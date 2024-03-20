using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BaggageStorage.Data.Models
{
    public class CashOperation
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StorageId { get; set; }

        public TypeOperation Operation { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string UserId { get; set; }

        public string BaggageMovingId { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual BaggageMoving BaggageMoving { get; set; }
    }
}
