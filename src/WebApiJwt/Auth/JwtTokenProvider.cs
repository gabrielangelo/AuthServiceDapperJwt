using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace WebApiJwt.Auth
{
    public class JwtTokenProvider
    {
        private readonly IConfiguration _configuration;       

        private Dictionary<string, string> _tokenWithExpireDate;
        // private jwthan
        public JwtTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenWithExpireDate = new Dictionary<string, string>();
        }
        
        public Dictionary<string, string> GenerateJwtUserToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.IdUser.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.IdUser.ToString())
               };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);
            var access_token = new JwtSecurityToken(
              _configuration["JwtIssuer"],
              _configuration["JwtIssuer"],
              claims,
              expires: expires,
              signingCredentials: creds);
            
            _tokenWithExpireDate["access_token"] = new JwtSecurityTokenHandler().WriteToken(access_token);
            _tokenWithExpireDate["access_token_expires_in"] = expires.ToString();
            
            _tokenWithExpireDate["refresh_token"] = this.CreateRefreshToken(user.IdUser)["refresh_token"];
            _tokenWithExpireDate["refresh_token_expires_in"] = this.CreateRefreshToken(
                user.IdUser)["refresh_token_expires_in"];
            
            return _tokenWithExpireDate;
        }

        public Dictionary<string, string> CreateRefreshToken(int userId)
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                // new Claim(JwtRegisteredClaimNames.UniqueName, user.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.AddMinutes(30).ToString(), ClaimValueTypes.Integer64),
            };

            var expires = DateTime.Now.AddMinutes(30);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var access_token = new JwtSecurityToken(
              _configuration["JwtIssuer"],
              _configuration["JwtIssuer"],
              claims,
              expires: expires,
              signingCredentials: creds);
            
            _tokenWithExpireDate["refresh_token"] = new JwtSecurityTokenHandler().WriteToken(access_token);
            _tokenWithExpireDate["refresh_token_expires_in"] = expires.ToString();
            
            return _tokenWithExpireDate;
        }

    }
}