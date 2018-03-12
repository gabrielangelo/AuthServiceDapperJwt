using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using DOMAIN;
using DAL;
using DOMAIN.DTOs;

namespace BLL   
{
    public class AuthBLL
    {
        private UsersDAL _usersDAL;
        
        public AuthBLL()
        {
            _usersDAL = new UsersDAL();
        }

        //Constructor to unit tests
        public AuthBLL(UsersDAL usersDAL)
        {
            _usersDAL = usersDAL;
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

        //###### Method must receive claims from httpcontext and build a user object;
        //this method is fundamental to reuse user info
        // public User GetLoggedUser(List<Claim> claims)
        // {
            
        // }

       
    }
}