namespace HolidayMaker.Models
{
    public class Hotel
    {
        public ulong Id { get; set; }  
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? Rating { get; set; }
        public string Amenities { get; set; } = string.Empty;
    }
}
