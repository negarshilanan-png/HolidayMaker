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

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Namn och e-post måste anges.");
                return;
            }

            ulong newId = _customerRepo.AddCustomer(name, address, email, phone);
            _selectedCustomer = new Customer { Id = newId, Name = name, Address = address, Email = email, Phone = phone };
            Console.WriteLine($"Ny kund registrerad och vald! ID: {newId}");
        }

        static void SearchAndBook() { }
        static void ShowCustomerBookings() { }
        static void UpdateBookingOccupants() { }
        static void CancelBooking() { }
    }
}
