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
using System.Web;
using DOMAIN.DTOs;
using WebApiJwt.Auth;
using WebApiJwt.Resources;



namespace WebApiJwt.Controllers
{
    [Route("api/[controller]")]
     
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private JwtTokenProvider _jwtTokenProvider;
        private AuthUser _authUser;

        private bool _success;

        private bool _unsuccessful;

        private PasswordHasher<User> _hashHelper;


        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtTokenProvider = new JwtTokenProvider(_configuration);
            _authUser = new AuthUser();
            _success = true;
            _unsuccessful = !_success;
            _hashHelper = new PasswordHasher<User>();
            
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody]UserDTO userDTO)
        {
            var baseUser = this._authUser.CheckUser(userDTO); 
            
            if(baseUser != null && baseUser.IsActive)
            {
                if(this._authUser.Authenticate(baseUser, userDTO))
                {
                    var tokenWithDate = _jwtTokenProvider.GenerateJwtToken(baseUser);
                    var token =  tokenWithDate["token"];
                    var expires_in = tokenWithDate["expires_in"];

                    var data = new {
                        success=_success,
                        userId = baseUser.IdUser,
                        token= token,
                        expires_in = expires_in
                        };

                    return Ok(new JsonPayloadPattern().JsonPayload(_success, "", Ok().StatusCode, data));
                }
            }
                return Unauthorized();
        }

        [HttpPost("refresh_token"), Authorize]
        public IActionResult RefreshToken([FromBody] UserDTO userDTO)
        {
            return this.Post(userDTO);
        }
        
        [HttpGet("protect_area"), Authorize]
        public IActionResult Protect()
        {
            //the claims must be insert in the methods 
            var userClaims = HttpContext.User.Claims.ToList();
            var email = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Email).Value;

            return Ok(new JsonPayloadPattern().JsonPayload(
                _success, "PROTECTED AREA " + email, Ok().StatusCode, null));
        }

    }
}