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
using BLL;
using DOMAIN.DTOs;
using WebApiJwt.Auth;


namespace WebApiJwt.Controllers
{
    [Route("api/[controller]")]
     
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private JwtTokenProvider _jwtTokenProvider;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtTokenProvider = new JwtTokenProvider(_configuration);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody]UserDTO userDTO)
        {
            var authBLL = new AuthBLL();
            var baseUser = authBLL.CheckUser(userDTO); 
            
            if(baseUser != null &&  baseUser.IsActive)
            {
                if(authBLL.Authenticate(baseUser, userDTO))
                {
                    return Ok(_jwtTokenProvider.GenerateJwtToken(baseUser.Email, baseUser));
                }
            }
                return Unauthorized();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Protect()
        {
            return Ok("PROTECTED AREA");
        }

    }
}