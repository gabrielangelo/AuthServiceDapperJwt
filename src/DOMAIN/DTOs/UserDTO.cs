using System;

namespace DOMAIN.DTOs
{
    public class UserDTO
    {

        public UserDTO()
        {
            this.IsActive = false;
        }

        public bool IsActive { get; set;}
        public string Name {get; set;}
        public string Email { get; set; }
        public string  Password { get; set; }
    }
}