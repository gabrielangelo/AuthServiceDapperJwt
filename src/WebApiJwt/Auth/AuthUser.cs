using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using DOMAIN;
using DAL;
using DOMAIN.DTOs;
using Microsoft.AspNetCore.Identity;
// using WebApiJwt.Auth;

namespace WebApiJwt.Auth   
{
    public class AuthUser
    {
        private UsersDAL _usersDAL;
        private PasswordHasher<User> _hashPasswordHelper;
        
        public AuthUser()
        {
            _usersDAL = new UsersDAL();
            _hashPasswordHelper = new PasswordHasher<User>();
        }

        //Constructor to unit tests
        public AuthUser(UsersDAL usersDAL)
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
            var check_password = this.CheckPassword(baseUser, baseUser.Password, userDTO.Password);
            return check_password == PasswordVerificationResult.Success || 
            check_password == PasswordVerificationResult.SuccessRehashNeeded ? true: false;       
        }

        private PasswordVerificationResult CheckPassword(User user, string baseUserPassword, 
            string userDTOPassword)
        {
            return  _hashPasswordHelper
            .VerifyHashedPassword(user, baseUserPassword, userDTOPassword );
        }

       
    }
}