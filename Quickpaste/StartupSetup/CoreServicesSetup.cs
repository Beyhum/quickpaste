using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quickpaste.Data;
using Quickpaste.Services.PasteServices;
using Quickpaste.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quickpaste.StartupSetup
{
    public static class CoreServicesSetup
    {
        public static void AddCoreAppServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<UserSettings>(Configuration.GetSection("User"));

            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<UserService>();


            services.AddScoped<IUploadLinkRepository, EfUploadLinkRepository>();

            services.AddScoped<IPasteRepository, EfPasteRepository>();
            services.AddScoped<PasteService>();
        }

        /// <summary>
        /// Throws an ArgumentException if any of the configuration section's properties is null/empty
        /// </summary>
        /// <param name="configSettingsSection"></param>
        public static void CheckConfigSettings(IOptions<Object> configSettingsSection)
        {
            PropertyInfo[] properties = configSettingsSection.Value.GetType().GetProperties();
            foreach (var p in properties)
            {
                if (p.GetValue(configSettingsSection.Value) == null)
                {
                    throw new ArgumentException($"Invalid Configuration value for {configSettingsSection.Value.GetType().Name}:{p.Name}");

                }
            }
        }

    }
}
