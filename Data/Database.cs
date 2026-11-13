using MySqlConnector;

namespace HolidayMaker.Data
{
    public class Database
    {
      
        private readonly string _connectionString = "Server=localhost;Database=HolidayMaker;User ID=root;Password=kurslosenord;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}