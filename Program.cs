using System;
using System.Globalization;
using HolidayMaker.Data;
using HolidayMaker.Models;
using HolidayMaker.Repositories;

namespace HolidayMaker
{
    class Program
    {
        static Database _db = new Database();
        static CustomerRepository _customerRepo = new CustomerRepository(_db);
        static RoomRepository _roomRepo = new RoomRepository(_db);
        static BookingRepository _bookingRepo = new BookingRepository(_db);
        static Customer? _selectedCustomer = null;

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n=== HOLIDAY MAKER - BOKNINGSSYSTEM ===");
                Console.WriteLine($"Vald kund: {(_selectedCustomer != null ? $"{_selectedCustomer.Name} (ID: {_selectedCustomer.Id})" : "Ingen kund vald")}");
                Console.WriteLine("1. Välj befintlig kund");
                Console.WriteLine("2. Registrera ny kund");
                Console.WriteLine("3. Sök lediga rum och boka");
                Console.WriteLine("4. Visa vald kunds bokningar");
                Console.WriteLine("5. Ändra antal personer i en bokning");
                Console.WriteLine("6. Avboka (ta bort bokning)");
                Console.WriteLine("0. Avsluta");
                Console.Write("Välj ett alternativ: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1": SelectCustomer(); break;
                        case "2": RegisterCustomer(); break;
                        case "3": SearchAndBook(); break;
                        case "4": ShowCustomerBookings(); break;
                        case "5": UpdateBookingOccupants(); break;
                        case "6": CancelBooking(); break;
                        case "0": return;
                        default: Console.WriteLine("Ogiltigt val, försök igen."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fel: {ex.Message}");
                }
            }
        }

        static void SelectCustomer()
        {
            var customers = _customerRepo.GetAllCustomers();
            Console.WriteLine("\n--- Befintliga kunder ---");
            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.Id} | {c.Name} | {c.Email} | {c.Phone}");
            }

            Console.Write("Ange kundens ID: ");
            if (ulong.TryParse(Console.ReadLine(), out ulong id))
            {
                _selectedCustomer = customers.Find(c => c.Id == id);
                Console.WriteLine(_selectedCustomer != null
                    ? $"Kund vald: {_selectedCustomer.Name}"
                    : "Kunden hittades inte.");
            }
            else
            {
                Console.WriteLine("Ogiltigt ID.");
            }
        }

        static void RegisterCustomer()
        {
            Console.Write("Namn: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Adress: ");
            string address = Console.ReadLine() ?? "";
            Console.Write("E-post: ");
            string email = Console.ReadLine() ?? "";
            Console.Write("Telefon: ");
            string phone = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name) || !name.Trim().Contains(' ') || string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Du måste ange både för- och efternamn samt e-post.");
            return;
        }

            ulong newId = _customerRepo.AddCustomer(name, address, email, phone);
            _selectedCustomer = new Customer { Id = newId, Name = name, Address = address, Email = email, Phone = phone };
            Console.WriteLine($"Ny kund registrerad och vald! ID: {newId}");
        }

        static void SearchAndBook()
        {
            if (_selectedCustomer == null)
            {
                Console.WriteLine("Du måste välja eller registrera en kund först (val 1 eller 2).");
                return;
            }

            Console.Write("Ort (t.ex. Halmstad eller Malmö): ");
            string location = Console.ReadLine() ?? "";

            Console.Write("Incheckningsdatum (YYYY-MM-DD): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime checkIn))
            {
                Console.WriteLine("Felaktigt datumformat.");
                return;
            }
            if (checkIn.Date < DateTime.Today)
            {
                Console.WriteLine("Incheckningsdatum kan inte vara i det förflutna.");
                return;
            }

            Console.Write("Utcheckningsdatum (YYYY-MM-DD): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime checkOut))
            {
                Console.WriteLine("Felaktigt datumformat.");
                return;
            }

            if (checkOut <= checkIn)
            {
                Console.WriteLine("Utcheckningsdatum måste ligga efter incheckningsdatum.");
                return;
            }

            Console.Write("Antal personer: ");
            if (!int.TryParse(Console.ReadLine(), out int guests) || guests <= 0)
            {
                Console.WriteLine("Ogiltigt antal personer.");
                return;
            }

            Console.Write("Krävs pool? (j/n): ");
            bool needPool = Console.ReadLine()?.Trim().ToLower() == "j";

            Console.Write("Krävs restaurang? (j/n): ");
            bool needRestaurant = Console.ReadLine()?.Trim().ToLower() == "j";

            Console.Write("Max avstånd till strand i meter (lämna tomt för inget krav): ");
            string? beachInput = Console.ReadLine();
            int? maxBeach = int.TryParse(beachInput, out int bVal) ? bVal : null;

            Console.Write("Max avstånd till centrum i meter (lämna tomt för inget krav): ");
            string? cityInput = Console.ReadLine();
            int? maxCity = int.TryParse(cityInput, out int cVal) ? cVal : null;

            Console.Write("Sortera efter: 1 = Lägsta pris först, 2 = Högsta omdöme först: ");
            bool sortByRating = Console.ReadLine()?.Trim() == "2";

            var rooms = _roomRepo.SearchAvailableRooms(
                location, checkIn, checkOut, guests,
                needPool, needRestaurant, maxBeach, maxCity, sortByRating);

            if (rooms.Count == 0)
            {
                Console.WriteLine("\nInga lediga rum matchade dina sökkriterier.");
                return;
            }

            Console.WriteLine("\n--- Lediga rum ---");
            foreach (var r in rooms)
            {
                Console.WriteLine($"Rum-ID: {r.RoomId} | {r.HotelName} ({r.Location}) | Rum #{r.RoomNumber} | Kapacitet: {r.Capacity} | Pris/natt: {r.PricePerNight} kr | Omdöme: {r.Rating}");
            }

            Console.Write("\nAnge Rum-ID att boka (eller 0 för att avbryta): ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong chosenRoomId) || chosenRoomId == 0)
            {
                return;
            }

            if (!rooms.Exists(r => r.RoomId == chosenRoomId))
            {
                Console.WriteLine("Ogiltigt Rum-ID från sökresultatet.");
                return;
            }

            ulong bookingNumber = _bookingRepo.CreateBooking(_selectedCustomer.Id, chosenRoomId, checkIn, checkOut, guests, out decimal totalPrice);
            Console.WriteLine($"\nBokning bekräftad! Bokningsnummer: {bookingNumber} | Totalpris: {totalPrice} kr");
        }

        static void ShowCustomerBookings()
        {
            if (_selectedCustomer == null)
            {
                Console.WriteLine("Välj en kund först.");
                return;
            }

            var bookings = _bookingRepo.GetCustomerBookings(_selectedCustomer.Id);
            if (bookings.Count == 0)
            {
                Console.WriteLine("Kunden har inga aktiva bokningar.");
                return;
            }

            Console.WriteLine($"\n--- Bokningar för {_selectedCustomer.Name} ---");
            foreach (var b in bookings)
            {
                Console.WriteLine($"Bokningsnr: {b.BookingNumber} | {b.HotelName} (Rum #{b.RoomNumber}) | {b.StartDate:yyyy-MM-dd} till {b.EndDate:yyyy-MM-dd} | Personer: {b.NumberOfOccupant} | Totalpris: {b.TotalPrice} kr");
            }
        }

        static void UpdateBookingOccupants()
        {
            ShowCustomerBookings();
            if (_selectedCustomer == null) return;

            Console.Write("Ange bokningsnummer att ändra: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong bookingNumber))
            {
                Console.WriteLine("Ogiltigt bokningsnummer.");
                return;
            }

            Console.Write("Nytt antal personer: ");
            if (!int.TryParse(Console.ReadLine(), out int newCount) || newCount <= 0)
            {
                Console.WriteLine("Ogiltigt antal personer.");
                return;
            }

            bool updated = _bookingRepo.UpdateOccupants(bookingNumber, _selectedCustomer.Id, newCount);
            Console.WriteLine(updated
                ? "Antal personer har uppdaterats."
                : "Kunde inte uppdatera (kontrollera att bokningsnumret stämmer och att rummets kapacitet räcker).");
        }

        static void CancelBooking()
        {
            ShowCustomerBookings();
            if (_selectedCustomer == null) return;

            Console.Write("Ange bokningsnummer att avboka: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong bookingNumber))
            {
                Console.WriteLine("Ogiltigt bokningsnummer.");
                return;
            }

           bool deleted = _bookingRepo.DeleteBooking(bookingNumber, _selectedCustomer.Id);
            Console.WriteLine(deleted
                ? "Bokningen har tagits bort och rummet är nu ledigt igen."
                : "Bokningen hittades inte.");
        }
    }
}