using System;
using System.Collections.Generic;
using HolidayMaker.Data;
using HolidayMaker.Models;
using MySqlConnector;

namespace HolidayMaker.Repositories
{
    public class RoomRepository
    {
        private readonly Database _db;

        public RoomRepository(Database db)
        {
            _db = db;
        }

        public List<RoomSearchResult> SearchAvailableRooms(
            string location,
            DateTime checkIn,
            DateTime checkOut,
            int guests,
            bool requirePool,
            bool requireRestaurant,
            int? maxBeachDistance,
            int? maxCityDistance,
            bool sortByRatingDesc)
        {
            var results = new List<RoomSearchResult>();
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
                SELECT r.id AS room_id, h.name AS hotel_name, h.location,
                       r.room_number, r.capacity, r.id_price_per_night, h.rating
                FROM Rooms r
                JOIN Hotels h ON r.hotel_id = h.id
                WHERE h.location = @location
                  AND r.capacity >= @guests
                  AND r.id NOT IN (
                      SELECT b.room_id
                      FROM Bookings b
                      WHERE b.is_cancelled = FALSE
                        AND b.start_date < @checkOut
                        AND b.end_date > @checkIn
                  )";

            if (requirePool)
            {
                sql += @" AND h.id IN (
                    SELECT ha.hotel_id FROM Hotel_x_Amenities ha
                    JOIN Amenities a ON ha.amenities_id = a.id
                    WHERE a.name = 'Pool'
                )";
            }

            if (requireRestaurant)
            {
                sql += @" AND h.id IN (
                    SELECT ha.hotel_id FROM Hotel_x_Amenities ha
                    JOIN Amenities a ON ha.amenities_id = a.id
                    WHERE a.name = 'Restaurang'
                )";
            }

            if (maxBeachDistance.HasValue)
            {
                sql += " AND h.distance_to_beach <= @maxBeach";
            }

            if (maxCityDistance.HasValue)
            {
                sql += " AND h.distance_to_city <= @maxCity";
            }

            if (sortByRatingDesc)
            {
                sql += " ORDER BY h.rating DESC, r.id_price_per_night ASC";
            }
            else
            {
                sql += " ORDER BY r.id_price_per_night ASC";
            }

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@guests", guests);
            cmd.Parameters.AddWithValue("@checkIn", checkIn.Date);
            cmd.Parameters.AddWithValue("@checkOut", checkOut.Date);

            if (maxBeachDistance.HasValue)
                cmd.Parameters.AddWithValue("@maxBeach", maxBeachDistance.Value);

            if (maxCityDistance.HasValue)
                cmd.Parameters.AddWithValue("@maxCity", maxCityDistance.Value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new RoomSearchResult
                {
                    RoomId = reader.GetUInt64("room_id"),
                    HotelName = reader.GetString("hotel_name"),
                    Location = reader.GetString("location"),
                    RoomNumber = reader.GetInt32("room_number"),
                    Capacity = reader.GetInt32("capacity"),
                    PricePerNight = reader.GetDecimal("id_price_per_night"),
                    Rating = reader.GetDecimal("rating")
                });
            }

            return results;
        }
    }
}
