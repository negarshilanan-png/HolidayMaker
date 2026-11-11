namespace HolidayMaker.Models
{
    public class Room
    {
        public ulong Id { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
    }
}