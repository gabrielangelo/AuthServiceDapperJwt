using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using DOMAIN.DTOs;
using DAL.Data;

namespace DAL
{
    public class UsersDAL
    {
        private string _connectionString;

        public UsersDAL()
        {
            _connectionString = new DbContext().GetConnection();
        }

        public User CheckUser(UserDTO user)
        {
            using (var conection = new MySqlConnection(_connectionString))
            {
                return conection.QueryFirstOrDefault<User>(
                    "SELECT * FROM User WHERE email = @email", new { email = user.Email });
            }
        }

        public void CreateUser(UserDTO user)
        {
            using( var conection = new MySqlConnection(_connectionString))
            {
                using(var transaction = new TransactionScope())
                {
                    conection.Execute(
                        "INSERT INTO User (email, password, isActive) Values(@email, @password, @isActive)", 
                        user);
                    transaction.Complete();
                }
            }
        }
    }
}