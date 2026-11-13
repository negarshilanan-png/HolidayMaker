using System;

namespace HolidayMaker.Models
{
    public class BookingInfo
    {
        public ulong BookingNumber { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfOccupant { get; set; }
        public decimal TotalPrice { get; set; }
    }
}