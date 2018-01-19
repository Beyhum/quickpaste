using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Quickpaste.StartupSetup;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Quickpaste.Services.AuthServices
{
    public class AuthorizationService
    {
        private readonly IConfiguration _config;
        private readonly TokenValidationParamsProvider _tokenValidationParamsProvider;
        private readonly AuthSettings _authSettings;
        private readonly HostingSettings _hostingSettings;

        public AuthorizationService(IConfiguration config, IOptions<AuthSettings> authSettings, IOptions<HostingSettings> hostingSettings, TokenValidationParamsProvider tokenValidationParamsProvider)
        {
            _config = config;
            _tokenValidationParamsProvider = tokenValidationParamsProvider;
            _authSettings = authSettings.Value;
            _hostingSettings = hostingSettings.Value;
        }

        /// <summary>
        /// Creates a signed JSON Web Token with the username as claims
        /// </summary>
        /// <param name="username"></param>
        /// <returns>TokenResponse containing the access token, expiration date and username</returns>
        public TokenResponse CreateAuthToken(string username)
        {
            // GUID to identify the JWT
            string tokenIdentifier = Guid.NewGuid().ToString();

            Claim[] claims = {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, tokenIdentifier)
            };

            JwtSecurityToken token = CreateTokenHelper(claims, _authSettings.Duration);
            
            return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo, username);
        }


        /// <summary>
        /// Creates a custom JWT with the specified claims and duration
        /// </summary>
        /// <param name="claims">The claims to be added to the token</param>
        /// <param name="durationInMinutes">Amount of time the JWT is valid before it expires</param>
        /// <returns>string representing the created token</returns>
        public string CreateCustomToken(Claim[] claims, int durationInMinutes)
        {

            JwtSecurityToken token = CreateTokenHelper(claims, durationInMinutes);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        private JwtSecurityToken CreateTokenHelper(Claim[] claims, int durationInMinutes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _hostingSettings.Hostname,
                audience: _hostingSettings.Hostname,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
                signingCredentials: credentials

                );
            return token;
        }

        /// <summary>
        /// Checks the token string and returns a ClaimsPrincipal containing the token's claims if it is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns>ClaimsPrincipal if valid token, null otherwise</returns>
        public ClaimsPrincipal ValidateToken(string token)
        {

            TokenValidationParameters validationParameters = _tokenValidationParamsProvider.DefaultParams();
            SecurityToken validatedToken = null;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenClaims = handler.ValidateToken(token, validationParameters, out validatedToken);
                return validatedToken == null ?  null : tokenClaims;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }


    }

    /// <summary>
    /// Class representing the response DTO that the authentication controller returns on successful authentication
    /// </summary>
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }


        public TokenResponse(string token, DateTime expiration, string displayName)
        {
            Token = token;
            Expiration = expiration;
            Username = displayName;

        }
    }
}
