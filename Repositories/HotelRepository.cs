using System.Collections.Generic;
using HolidayMaker.Data;
using HolidayMaker.Models;
using MySqlConnector;

namespace HolidayMaker.Repositories
{
    public class HotelRepository
    {
        private readonly Database _db;

        public HotelRepository(Database db)
        {
            _db = db;
        }

        public List<Hotel> GetAllHotels()
        {
            var hotels = new List<Hotel>();
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT id, name, location, distance_to_beach, distance_to_city, rating FROM Hotels ORDER BY name";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                hotels.Add(new Hotel
                {
                    Id = reader.GetUInt64("id"),
                    Name = reader.GetString("name"),
                    Location = reader.GetString("location"),
                    DistanceToBeach = reader.GetInt32("distance_to_beach"),
                    DistanceToCity = reader.GetInt32("distance_to_city"),
                    Rating = reader.GetDecimal("rating")
                });
            }
            return hotels;
        }
    }
}