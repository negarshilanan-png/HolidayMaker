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
    }
}
