using System;
using System.Collections.Generic;
using HolidayMaker.Data;
using HolidayMaker.Models;
using MySqlConnector;

namespace HolidayMaker.Repositories
{
    public class BookingRepository
    {
        private readonly Database _db;

        public BookingRepository(Database db)
        {
            _db = db;
        }

        public ulong CreateBooking(ulong customerId, ulong roomId, DateTime checkIn, DateTime checkOut, int guests, out decimal totalPrice)
        {
            totalPrice = 0;
            using var conn = _db.GetConnection();
            conn.Open();

            // Kontrollera tillgänglighet och kapacitet igen innan sparande
            string checkSql = @"
                SELECT id_price_per_night, capacity
                FROM Rooms
                WHERE id = @roomId
                  AND capacity >= @guests
                  AND id NOT IN (
                      SELECT room_id FROM Bookings
                      WHERE is_cancelled = FALSE
                        AND start_date < @checkOut
                        AND end_date > @checkIn
                  )";

            using var checkCmd = new MySqlCommand(checkSql, conn);
            checkCmd.Parameters.AddWithValue("@roomId", roomId);
            checkCmd.Parameters.AddWithValue("@guests", guests);
            checkCmd.Parameters.AddWithValue("@checkIn", checkIn.Date);
            checkCmd.Parameters.AddWithValue("@checkOut", checkOut.Date);

            decimal pricePerNight;
            using (var reader = checkCmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    throw new Exception("Rummet är inte längre ledigt eller har för liten kapacitet.");
                }
                pricePerNight = reader.GetDecimal("id_price_per_night");
            }

            int nights = (checkOut.Date - checkIn.Date).Days;
            totalPrice = pricePerNight * nights;

            string insertSql = @"
                INSERT INTO Bookings (customer_id, room_id, start_date, end_date, total_price, is_cancelled, is_booked, number_of_occupant)
                VALUES (@customerId, @roomId, @checkIn, @checkOut, @totalPrice, FALSE, TRUE, @guests);
                SELECT LAST_INSERT_ID();";

            using var insertCmd = new MySqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@customerId", customerId);
            insertCmd.Parameters.AddWithValue("@roomId", roomId);
            insertCmd.Parameters.AddWithValue("@checkIn", checkIn.Date);
            insertCmd.Parameters.AddWithValue("@checkOut", checkOut.Date);
            insertCmd.Parameters.AddWithValue("@totalPrice", totalPrice);
            insertCmd.Parameters.AddWithValue("@guests", guests);

            return Convert.ToUInt64(insertCmd.ExecuteScalar());
        }
    }
}

