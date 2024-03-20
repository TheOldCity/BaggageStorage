using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaggageStorage.Classes
{
    public class ConsoleLogger
    {
        public ConsoleLogger(string requestType, string uri) {
            Console.WriteLine("==========NEW REQUEST==========");
            Console.WriteLine("|");
            Console.WriteLine("| Request type: " + requestType);
            Console.WriteLine("| Uri:          " + uri);
            Console.WriteLine("|");
        }

        public void login(string username, string password, bool remember) {
            Console.WriteLine("| Username:     " + username);
            Console.WriteLine("| Password:     " + password);
            Console.WriteLine("| Remember:     " + Convert.ToString(remember));
            Console.WriteLine("|");
        }

        public void logout(string sessionId)
        {
            Console.WriteLine("| Session id:   " + sessionId);
            Console.WriteLine("|");
        }

        public void newPlace(string placeNr, int hourPrice, int daylyPrice)
        {
            Console.WriteLine("| Place number: " + placeNr);
            Console.WriteLine("| Daily price:  " + hourPrice);
            Console.WriteLine("| Hour price:   " + daylyPrice);
            Console.WriteLine("|");
        }

        public void newClient(string phoneNr, string firstName, string lastName)
        {
            Console.WriteLine("| Phone number: " + phoneNr);
            Console.WriteLine("| First name:   " + firstName);
            Console.WriteLine("| Last name:    " + lastName);
            Console.WriteLine("|");
        }

        public void newBaggageReceiving(string phoneNr, string placeNr, int amountOfPlaces, int amountOfHours, int amountOfDays)
        {
            Console.WriteLine("| Phone number: " + phoneNr);
            Console.WriteLine("| Place number: " + placeNr);
            Console.WriteLine("| Places amount:" + amountOfPlaces);
            Console.WriteLine("| Hours:        " + amountOfHours);
            Console.WriteLine("| Days:         " + amountOfDays);
            Console.WriteLine("| Date:         " + Convert.ToString(DateTime.Now));
            Console.WriteLine("|");
        }

        public void newDelivery(string phoneNr, DateTime dateIn)
        {
            Console.WriteLine("| Phone number: " + phoneNr);
            Console.WriteLine("| Receive date: " + Convert.ToString(dateIn));
            Console.WriteLine("| Delivery date:" + Convert.ToString(DateTime.Now));
            Console.WriteLine("|");
        }
    }
}
