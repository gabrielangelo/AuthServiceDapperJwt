using System;

namespace DOMAIN.DTOs
{
    public class TokenDTO
    {
        public AccessTokenDTO AccessToken {get; set;}

        public RefreshTokenDTO RefreshToken {get; set;}
    }
}