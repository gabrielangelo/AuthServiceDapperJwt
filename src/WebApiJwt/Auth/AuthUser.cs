using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using DOMAIN;
using DAL;
using DOMAIN.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public User SetAuthTokens(Dictionary<string, string> tokens, User user)
        {
            user.Token.accessToken.token = tokens["access_token"];
            user.Token.accessToken.expiresIn = DateTime.Parse(tokens["access_token_expires_in"]);
            user.Token.refreshToken.token = tokens["refresh_token"];
            user.Token.refreshToken.expiresIn = DateTime.Parse(tokens["refresh_token"]);
            return user;
        }

        // public void UpdateRefreshToken(string token)
        // {
        //     _usersDAL.UpdateRefreshToken(token);
        // }

        private string GetTokenWithoutBearer(string refreshToken)
        {
            return refreshToken.Split(" ")[1];
        }

        public User CheckRefreshToken(string refreshToken)
        {
            return _usersDAL.CheckRefreshToken(this.GetTokenWithoutBearer(refreshToken));
        }
       
       public object PackPayloadPart(TokenDTO tokens,User user )
       {
            return new {
                success= true,
                userId = user.IdUser,
                access_token= tokens.AccessToken.token,//setTokens["access_token"],//baseUser.Token.accessToken.token,
                refresh_token = tokens.RefreshToken.token,//setTokens["refresh_token"],//baseUser.Token.refreshToken.token,
                refresh_tokens_expires_in = tokens.RefreshToken.expiresIn//setTokens["refresh_token_expires_in"]//baseUser.Token.refreshToken.expiresIn.ToString()//setTokens["refresh_token_expires_in"]
                };
        }
    }
}