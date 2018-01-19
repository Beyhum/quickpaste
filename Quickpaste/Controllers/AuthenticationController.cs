using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Quickpaste.Models.Dtos;
using Quickpaste.Services.UserServices;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Quickpaste.Services.BlobServices;
using Quickpaste.Services.AuthServices;
using static Quickpaste.Services.AuthServices.AuthorizationService;

namespace Quickpaste.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserService _userService;
        private readonly AuthorizationService _authorizationService;
        private readonly IConfiguration _config;



        public AuthenticationController(UserService userService, AuthorizationService authorizationService, IConfiguration config)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _config = config;
        }


        /// <summary>
        /// Takes a username and password to authenticate the user
        /// </summary>
        /// <param name="loginModel"></param>
        /// <response code="200">User authenticated</response>
        /// <response code="401">Invalid user/code</response>
        /// <returns>200 with TokenResponse on success, 401 on invalid user/code</returns>
        [Produces(typeof(TokenResponse))]
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!await _userService.ValidateCredentials(loginModel.Username, loginModel.Password))
            {
                return Unauthorized();
            }

            return Ok(_authorizationService.CreateAuthToken(loginModel.Username));

        }

        /// <summary>
        /// Registers a user using the default username and passcode. Only usable once (when there are no users in the database).
        /// If registration is successful, the user is authenticated with the custom username and password that they chose
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>TokenResponse on success, ApiError on failure</returns>
        [Produces(typeof(TokenResponse))]
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }

            var registerResponse = await _userService.Register(registerModel);
            if (registerResponse.result)
            {
                return Ok(_authorizationService.CreateAuthToken(registerModel.Username));
            }

            switch (registerResponse.reason)
            {
                case UserServiceResp.InvalidDefaults:
                    return BadRequest(new ApiError("Invalid default username/passcode"));
                case UserServiceResp.AccountExists:
                    return BadRequest(new ApiError("Account already exists"));
                case UserServiceResp.AccountLimitReached:
                    return BadRequest(new ApiError("An account has already been set up. Cannot create more accounts"));
                default:
                    break;
            }

            return StatusCode(500, ApiError.Unexpected());
        }

    }

}
