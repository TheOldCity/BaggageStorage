using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BaggageStorage.Data.Models
{
    public class Client
    {
        [Key]
        public string Id { get; set; }

        public int OrderId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public virtual ICollection<BaggageMoving> BaggageMovings { get; set; }
    }
}
