using System;

namespace DAL.Data
{
    public class DbContext
    {
        private string _connection;

        public DbContext()
        {
            _connection = "DbUrl";
        }
        public string GetConnection()
        {
            return _connection;
        }
    }    
}
