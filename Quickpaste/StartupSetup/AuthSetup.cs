using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quickpaste.Services.AuthServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.StartupSetup
{
    public static class AuthSetup
    {
        public static void AddAuth(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<AuthSettings>(Configuration.GetSection("Auth"));
            services.AddSingleton<TokenValidationParamsProvider>();
            services.AddScoped<AuthorizationService>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var authSettings = serviceProvider.GetService<IOptions<AuthSettings>>();
                CoreServicesSetup.CheckConfigSettings(authSettings);

            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DefaultPolicy", policyBuilder =>
                {
                    policyBuilder.AuthenticationSchemes = new List<string> { JwtBearerDefaults.AuthenticationScheme };
                    policyBuilder.RequireClaim(JwtRegisteredClaimNames.Sub); // require username in token
                }
                );
            });

            // Clear default mapping of JWT claims to legacy claims
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            })
            .AddJwtBearer(jwtOptions =>
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    jwtOptions.TokenValidationParameters = serviceProvider.GetService<TokenValidationParamsProvider>().DefaultParams();
                    jwtOptions.IncludeErrorDetails = true;

                }

            });
        }
        
        


    }
}
