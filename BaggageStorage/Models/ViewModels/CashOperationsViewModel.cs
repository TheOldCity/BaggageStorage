using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BaggageStorage.Models.ViewModels
{
    public class CashOperationsViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string StorageId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
