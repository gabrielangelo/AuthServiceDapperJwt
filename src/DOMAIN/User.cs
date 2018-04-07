 public class User
    {
        public int IdUser { get; set; }
        
        public string Email { get; set; }
        
        public string  Password { get; set; }
        
        public bool IsActive { get; set;}

        public string RefreshToken { get; set;}
        
        public Token Token{get; set;}
    }
