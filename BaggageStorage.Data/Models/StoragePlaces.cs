using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaggageStorage.Data.Models
{
    public class StoragePlace
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StorageId { get; set; }

        [Required]
        public string Place { get; set; }

        [Required]
        public int Price { get; set; }                    //Стоимость за сутки

        [Required]
        public int HourlyPrice { get; set; }              //Стоимость за час

        public virtual Storage Storage { get; set; }
        public virtual ICollection<BaggageMoving> BaggageMovings { get; set; }
    }
}