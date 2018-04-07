using System;
using DOMAIN;
using DOMAIN.DTOs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BLL;
using WebApiJwt.Resources;

namespace WebApiJwt.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        private bool _success;

        private bool _unsuccessful;

        private UsersBLL _usersBLL;

        private PasswordHasher<UserDTO> _hashHelper;


        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _success = true;
            _unsuccessful = !_success;
            _hashHelper = new PasswordHasher<UserDTO>();
            _usersBLL = new UsersBLL();
            
        }

        private UserDTO SetPassword(UserDTO user)
        {
            try{

                user.Password = _hashHelper.HashPassword(user, user.Password);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody]UserDTO user)
        {
            this.SetPassword(user);
            _usersBLL.CreateUser(user);
            return StatusCode(201);
        }
    } 
}