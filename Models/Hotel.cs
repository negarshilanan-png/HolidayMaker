namespace HolidayMaker.Models
{
    public class Hotel
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int DistanceToBeach { get; set; }
        public int DistanceToCity { get; set; }
        public decimal Rating { get; set; }
    }
}