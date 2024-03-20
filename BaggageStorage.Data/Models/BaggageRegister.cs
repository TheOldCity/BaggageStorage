using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BaggageStorage.Data.Models
{
   public class BaggageRegister
    {
        public string ClientId { get; set; }      //Ф.И.О. клиента

        public string StorageId { get; set; }

        public DateTime DateIn { get; set; }      //Дата приёма

        public int AmountOfDays { get; set; }  //Кол-во дней,на которое сдали багаж

        public int AmountOfHours { get; set; }  //Кол-во часов,на которое сдали багаж
    }
}
