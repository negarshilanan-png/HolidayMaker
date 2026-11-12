using System;
using System.Collections.Generic;
using HolidayMaker.Data;
using HolidayMaker.Models;
using MySqlConnector;

namespace HolidayMaker.Repositories
{
    public class CustomerRepository
    {
        private readonly Database _db;

        public CustomerRepository(Database db)
        {
            _db = db;
        }

        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT id, name, address, email, phone FROM Customers ORDER BY id";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(new Customer
                {
                    Id = reader.GetUInt64("id"),
                    Name = reader.GetString("name"),
                    Address = reader.GetString("address"),
                    Email = reader.GetString("email"),
                    Phone = reader.GetString("phone")
                });
            }
            return customers;
        }
    }
}
