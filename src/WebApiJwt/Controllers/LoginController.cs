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
using BLL;
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
        private AuthBLL _authBLL;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtTokenProvider = new JwtTokenProvider(_configuration);
            _authBLL = new AuthBLL();
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody]UserDTO userDTO)
        {
            var baseUser = _authBLL.CheckUser(userDTO); 
            
            if(baseUser != null &&  baseUser.IsActive)
            {
                if(_authBLL.Authenticate(baseUser, userDTO))
                {
                    var data = new {
                        success=true,
                        userId = baseUser.IdUser,
                        token= _jwtTokenProvider.GenerateJwtToken(baseUser.Email, baseUser)
                    };

                    return Ok(new JsonPayloadPattern().JsonPayload(true, "", Ok().StatusCode, data));
                }
            }
                return Unauthorized();
        }

        [HttpGet, Authorize]
        public IActionResult Protect()
        {
            //the claims must be insert in the methods 
            var userClaims = HttpContext.User.Claims.ToList();
            var email = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Email).Value;

            return Ok("PROTECTED AREA " + email );
        }

    }
}