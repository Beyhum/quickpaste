using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quickpaste.Services.BlobServices;
using Quickpaste.Services.BlobServices.Azure;
using Quickpaste.Services.BlobServices.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quickpaste.StartupSetup
{
    public static class BlobServiceSetup
    {

        public const string AzureProvider = "Azure";
        public const string LocalProvider = "Local";

        /// <summary>
        /// Extension method to add Azure Blob Storage as a storage service for blobs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void AddAzureBlobService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<AzureBlobStorageSettings>(Configuration.GetSection("BlobStorage:Azure"));
            services.AddScoped<IBlobStorageStrategy, AzureBlobStorageStrategy>();
            services.AddScoped<AzureBlobStorageStrategy>();
            services.AddScoped<BlobStorageService>();

            // access DI container to get services we just registered and pass them to the storage initializer
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var storageService = serviceProvider.GetService<AzureBlobStorageStrategy>();
                var azureStorageSettings = serviceProvider.GetService<IOptions<AzureBlobStorageSettings>>();

                CoreServicesSetup.CheckConfigSettings(azureStorageSettings);
                AzureBlobStorageInitializer.InitializeStorage(storageService, azureStorageSettings);
            }


        }

        /// <summary>
        /// Extension method to add Local storage as a storage service for blobs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void AddLocalBlobService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IBlobStorageStrategy, LocalBlobStorageStrategy>();
            services.AddScoped<LocalBlobStorageStrategy>();
            services.AddScoped<BlobStorageService>();

            services.AddDbContext<BlobDbContext>(options =>
            {
                options.UseSqlite(Configuration["BlobStorage:Local:ConnectionString"]);
            });

            using (var serviceProvider = services.BuildServiceProvider())
            {
                var blobContext = serviceProvider.GetService<BlobDbContext>();
                blobContext.Database.Migrate();
            }


        }

        /// <summary>
        /// Extension method which registers an implementation of a Blob Service depending on the application's configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static void AddStorageService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<BlobStorageSettings>(Configuration.GetSection("BlobStorage"));

            switch (Configuration["BlobStorage:Provider"])
            {
                case AzureProvider:
                    services.AddAzureBlobService(Configuration);
                    break;
                case LocalProvider:
                    services.AddLocalBlobService(Configuration);
                    break;
                default:
                    throw new ArgumentException("Invalid Configuration value for BlobStorage:Provider");

            }


        }

        /// <summary>
        /// Middleware to block access to FilesController if Local storage is not used for blobs
        /// </summary>
        /// <param name="app"></param>
        /// <param name="Configuration"></param>
        public static void HandleBlobStoragePath(this IApplicationBuilder app, IConfiguration Configuration)
        {
            if (Configuration["BlobStorage:Provider"] != LocalProvider)
            {
                app.Map("/files", BlockFileController);
            }
        }

        private static void BlockFileController(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.StatusCode = 404;

            });
        }




    }
}
