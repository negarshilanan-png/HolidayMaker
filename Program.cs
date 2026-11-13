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

        static void SelectCustomer() { }
        static void RegisterCustomer() { }
        static void SearchAndBook() { }
        static void ShowCustomerBookings() { }
        static void UpdateBookingOccupants() { }
        static void CancelBooking() { }
    }
}