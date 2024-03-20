using System;
using System.Collections.Generic;
using System.Text;

namespace BaggageStorage.Data.Models
{
    public enum TypeOperation
    {
        AddMoney,       // внесение денег в кассу (например для сдачи)
        RemoveMoney,    // изъятие денег из кассы (например выручку хозяину отдать)
        BaggagePayment,   // оплата за багаж
        BaggageAdditionalPayment   // доплата за багаж (когда превышено кол-во дней на который сдавали багаж)
    }
}
