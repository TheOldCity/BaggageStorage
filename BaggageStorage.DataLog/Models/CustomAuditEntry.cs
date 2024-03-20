using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Z.EntityFramework.Plus;

namespace BaggageStorage.DataLog.Models
{
    [Table("AuditEntries")]
    public class CustomAuditEntry : AuditEntry
    {
        [MaxLength(15)]
        public string IpAddress { get; set; }

        [MaxLength(255)]
        public string UserAgent { get; set; }
    }
}
