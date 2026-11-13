namespace HolidayMaker.Models
{
    public class RoomSearchResult
    {
        public ulong RoomId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal Rating { get; set; }
    }
}