using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quickpaste.StartupSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickpaste.Services.AuthServices
{
    public class TokenValidationParamsProvider
    {
        private readonly HostingSettings _hostingSettings;
        private readonly AuthSettings _authSettings;

        public TokenValidationParamsProvider(IOptions<HostingSettings> hostingSettings, IOptions<AuthSettings> authSettings)
        {
            _hostingSettings = hostingSettings.Value;
            _authSettings = authSettings.Value;
        }

        /// <summary>
        /// Returns the default parameters to use when validating a JWT token
        /// </summary>
        /// <returns>TokenValidationParameters with default settings</returns>
        public TokenValidationParameters DefaultParams()
        {
            return new TokenValidationParameters()
            {
                ValidIssuer = _hostingSettings.Hostname,
                ValidAudience = _hostingSettings.Hostname,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Key)),
                RequireSignedTokens = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = new TimeSpan(0)
            };
        }
    }
}
