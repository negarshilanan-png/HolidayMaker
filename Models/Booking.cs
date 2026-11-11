using System;

namespace HolidayMaker.Models
{
    public class Booking
    {
        public ulong BookingNumber { get; set; }
        public ulong CustomerId { get; set; }
        public ulong RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int NumberOfOccupant { get; set; }
        public bool IsCancelled { get; set; }
    }
}