using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BaggageStorage.Data.Models
{
    public class WorkPlace
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string IpAdress { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string PrintName { get; set; }

        [Required]
        public int AmountOfCopies { get; set; }

        [Required]
        public string TemplateName { get; set; }

        
        public string IpAdressHardwareService { get; set; }
        public int PortHardwareService { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
