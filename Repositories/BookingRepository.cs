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

        public List<BookingInfo> GetCustomerBookings(ulong customerId)
        {
            var list = new List<BookingInfo>();
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
                SELECT b.booking_number, h.name AS hotel_name, r.room_number,
                       b.start_date, b.end_date, b.number_of_occupant, b.total_price
                FROM Bookings b
                JOIN Rooms r ON b.room_id = r.id
                JOIN Hotels h ON r.hotel_id = h.id
                WHERE b.customer_id = @customerId AND b.is_cancelled = FALSE
                ORDER BY b.start_date";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BookingInfo
                {
                    BookingNumber = reader.GetUInt64("booking_number"),
                    HotelName = reader.GetString("hotel_name"),
                    RoomNumber = reader.GetInt32("room_number"),
                    StartDate = reader.GetDateTime("start_date"),
                    EndDate = reader.GetDateTime("end_date"),
                    NumberOfOccupant = reader.GetInt32("number_of_occupant"),
                    TotalPrice = reader.GetDecimal("total_price")
                });
            }
            return list;
        }

        public bool UpdateOccupants(ulong bookingNumber, ulong customerId, int newCount)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"UPDATE Bookings b
                   JOIN Rooms r ON b.room_id = r.id
                   SET b.num_guests = @newCount
                   WHERE b.id = @bookingNumber
                     AND b.customer_id = @customerId
                     AND @newCount >= 1
                     AND @newCount <= r.capacity;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@bookingNumber", bookingNumber);
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Parameters.AddWithValue("@newCount", newCount);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteBooking(ulong bookingNumber, ulong customerId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "DELETE FROM Bookings WHERE id = @bookingNumber AND customer_id = @customerId;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@bookingNumber", bookingNumber);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}   


