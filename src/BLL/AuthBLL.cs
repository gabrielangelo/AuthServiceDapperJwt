using System;
using System.Security.Claims;
using DOMAIN;
using DAL;
using DOMAIN.DTOs;
using Microsoft.Extensions.Configuration;


namespace BLL
{
    public class AuthBLL
    {
        private readonly IConfiguration _configuration;
        private UsersDAL _usersDAL;
        public AuthBLL(IConfiguration configuration)
        {
            _configuration = configuration;
            _usersDAL = new UsersDAL(_configuration);
        }
        public User CheckUser(UserDTO userDTO)
        {
            try
            {
                return _usersDAL.CheckUser(userDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Authenticate(User baseUser, UserDTO userDTO)
        {
            return this.CheckPassword(baseUser.Password, userDTO.Password);
        }

        private bool CheckPassword(string baseUserPassword, 
            string userDTOPassword)
        {
            //without cryptografy for a while
            return true ? baseUserPassword == userDTOPassword: false;
        }

       
    }
}