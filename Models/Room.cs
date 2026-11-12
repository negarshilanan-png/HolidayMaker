namespace HolidayMaker.Models
{
    public class Room
    {
        public ulong Id { get; set; }
        public decimal IdPricePerNight { get; set; }
        public int Capacity { get; set; }
        public ulong HotelId { get; set; }
        public int RoomNumber { get; set; }
    }
}