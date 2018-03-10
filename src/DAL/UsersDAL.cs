using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using DOMAIN.DTOs;

namespace DAL
{
    public class UsersDAL
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public UsersDAL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public User CheckUser(UserDTO user)
        {
            using (var conection = new MySqlConnection(_connectionString))
            {
                return conection.QueryFirstOrDefault<User>(
                    "SELECT * FROM User WHERE email = @email", new { email = user.Email });
            }
        }
    }
}