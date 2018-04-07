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
using BLL;
namespace WebApiJwt.Controllers
{

    [Route("api/refresh_token")]

    [Authorize, Authorize("IsRefreshToken")]
    public class RefreshTokenController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected JwtTokenProvider _jwtTokenProvider;
        protected AuthUser _authUser;

        protected bool _success;

        protected bool _unsuccessful;

        protected UsersBLL _usersBLL;

        public RefreshTokenController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtTokenProvider = new JwtTokenProvider(_configuration);
            _authUser = new AuthUser();
            _success = true;
            _unsuccessful = !_success;
            _usersBLL = new UsersBLL();
        }

        [HttpPost]
        public IActionResult RefreshToken([FromHeader]string Authorization)
        {   
            var baseUser = _authUser.CheckRefreshToken(Authorization);
            if(baseUser != null && baseUser.IsActive )
            {
                var tokens = _jwtTokenProvider.GenerateJwtUserToken(baseUser);
                    
                var payloadData = _authUser.PackPayloadPart(tokens, baseUser);

                return Ok(new JsonPayloadPattern().JsonPayload(_success, "", Ok().StatusCode, payloadData));
            }
            return Unauthorized();
        }
    }
}