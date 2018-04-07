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
        protected readonly IConfiguration _configuration;
        protected JwtTokenProvider _jwtTokenProvider;
        protected AuthUser _authUser;

        protected bool _success;

        protected bool _unsuccessful;

        public LoginController(IConfiguration configuration)
        {
             _configuration = configuration;
            _jwtTokenProvider = new JwtTokenProvider(_configuration);
            _authUser = new AuthUser();
            _success = true;
            _unsuccessful = !_success;
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Post([FromBody]UserDTO userDTO)
        {
            var baseUser = this._authUser.CheckUser(userDTO); 
            
            if(baseUser != null && baseUser.IsActive)
            {
                if(this._authUser.Authenticate(baseUser, userDTO))
                {
                    var tokens = _jwtTokenProvider.GenerateJwtUserToken(baseUser);
                    var payloadData = _authUser.PackPayloadPart(tokens, baseUser);

                    return Ok(
                        new JsonPayloadPattern().JsonPayload(_success, "", Ok().StatusCode, payloadData));
                }
            }
                return Unauthorized();
        }

        
        // [HttpGet("protect_area"), Authorize, Authorize("IsNotRefreshToken")]
        // public IActionResult Protect()
        // {
        //     //the claims must be insert in the methods 
        //     var userClaims = HttpContext.User.Claims.ToList();
        //     var test = HttpContext.User.HasClaim(c =>
        //                         (c.Type == ClaimTypes.AuthorizationDecision));           
            
        //     // var email = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.AuthorizationDecision).Value;
        //     // var teste = HttpContext.Items;

        //     return Ok(new JsonPayloadPattern().JsonPayload(
        //         _success, "PROTECTED AREA " , Ok().StatusCode, null));
        // }

    }
}