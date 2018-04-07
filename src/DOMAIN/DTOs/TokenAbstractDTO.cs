using System;

namespace DOMAIN.DTOs
{
    public abstract class TokenAbstractDTO
    {
        public string token { get; set;}
        
        public DateTime expiresIn { get; set;}
    }
}