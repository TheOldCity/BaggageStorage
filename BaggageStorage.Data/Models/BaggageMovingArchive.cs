using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace BaggageStorage.Data.Models
{
    public class BaggageMovingArchive
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string StoragePlaceId { get; set; }

        [Required]
        public string StorageId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public int AmountOfDays { get; set; }                    //Количество дней

        [Required]
        public int AmountOfPlaces { get; set; }                  //Количество мест

        [Required]
        public decimal Amount { get; set; }                      //Сумма

        [Required]
        public string UserInId { get; set; }                     //Кто принял (Инициалы работника)

        [Required]
        public DateTime DateIn { get; set; }                     //День/время принятия

        public string UserOutId { get; set; }                    //Кто выдал (Инициалы работника)

        public DateTime DateOut { get; set; }                    //День/время выдачи        

        public string UserDeletedId { get; set; }                  // Id пользователя, кто удалил

    }
}