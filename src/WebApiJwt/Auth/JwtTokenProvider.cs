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
using DAL;
using DOMAIN.DTOs;


namespace WebApiJwt.Auth
{
    public class JwtTokenProvider
    {
        private readonly IConfiguration _configuration;    

        private UsersDAL _userDAL;

        private double _refresh_token_time_expires;

        private double _access_token_time_expires;

        private SymmetricSecurityKey _key;

        private SigningCredentials _creds;


        // private jwthan
        public JwtTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _userDAL = new UsersDAL();
            _access_token_time_expires = Convert.ToDouble(_configuration["time_refresh_token_expires"]);
            _refresh_token_time_expires = Convert.ToDouble(_configuration["time_access_token_expires"]);
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        }

        private TokenDTO GetTokenDTO(User user, JwtSecurityToken accesToken, DateTime expires)
        {
            return new TokenDTO{
                AccessToken = new AccessTokenDTO {
                    token = new JwtSecurityTokenHandler().WriteToken(accesToken),
                    expiresIn = expires
                }, 
                RefreshToken = this.CreateRefreshToken(user.IdUser)
                };
        }

        private JwtSecurityToken GetAcessToken(Claim[] claims, 
        DateTime expires)
        {
            return new JwtSecurityToken(
              _configuration["JwtIssuer"],
              _configuration["JwtIssuer"],
              claims,
              expires: expires,
              signingCredentials: _creds
              );
        }
        
        public TokenDTO GenerateJwtUserToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.IdUser.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.IdUser.ToString())
               };

            var expires = DateTime.Now.AddDays(_access_token_time_expires);
            
            var access_token = this.GetAcessToken(claims, expires);
            var tokens = this.GetTokenDTO(user, access_token, expires);
            _userDAL.UpdateRefreshToken(tokens.RefreshToken.token, user.IdUser );
            return tokens;
        }

        public RefreshTokenDTO CreateRefreshToken(int userId)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.AddDays(30).ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.AuthorizationDecision, "is_refresh_token","true")
            };

            var expires = DateTime.Now.AddDays(_refresh_token_time_expires);
            var access_token = this.GetAcessToken(claims, expires);

            return new RefreshTokenDTO{
                token = new JwtSecurityTokenHandler().WriteToken(access_token), 
                expiresIn = expires
            };
        }
    }

}
