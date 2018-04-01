using System;
using DAL;
using DOMAIN.DTOs;

namespace BLL
{
    
    public class UsersBLL
    {
        private UsersDAL _usersDAL;

        public UsersBLL()
        {
            _usersDAL = new UsersDAL();
        }

        public UsersBLL(UsersDAL usersDAL)
        {
            _usersDAL = usersDAL;
        } 

        public void CreateUser(UserDTO user)
        {
            try
            {
                _usersDAL.CreateUser(user);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}