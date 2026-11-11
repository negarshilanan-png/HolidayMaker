namespace HolidayMaker.Models
{
    public class Hotel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal? Rating { get; set; }
        public string Amenities { get; set; }
    }
}
