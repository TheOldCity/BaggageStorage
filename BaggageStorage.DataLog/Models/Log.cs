using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BaggageStorage.DataLog.Models
{
    public  enum EventTypes
    {
        Information=1,
        Warning=2,
        Error=3,
        RetailMaster=4,
        RetailMasterHttpLogging = 5
    }

    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(1024)]
        public string UserAgent { get; set; }

        [StringLength(39)]
        public string UserIp { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(50)]
        public string Controller { get; set; }

        [Required]
        [StringLength(1024)]
        public string RawUrl { get; set; }

        public string Method { get; set; }

        public string RequestPostParams { get; set; }

        [Required]
        public EventTypes EventType { get; set; }

        [Required]
        public string Message { get; set; }                
    }
}
