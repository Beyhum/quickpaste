using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Quickpaste
{
    public class Program
    {

        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var currentEnvironment = hostingContext.HostingEnvironment;
                    GenerateAuthTokenKeyIfNotExists(currentEnvironment);

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{currentEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>()
                    .AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls(GetAppHostUrls(args));
        }

        private static string[] GetAppHostUrls(string[] args)
        {
            // check if an arg with format "Hosting:Hostname={hostname}" exists
            foreach (string arg in args)
            {
                if (arg.Contains("Hosting:Hostname"))
                {
                    string hostname = arg.Split("=")[1];
                    return new string[] { hostname };
                }
            }
            IConfigurationRoot config = GetTempConfiguration();

            string hostName = config["Hosting:Hostname"];
            string reverseProxyHost = config["Hosting:ReverseProxyHost"];

            HashSet<string> urlsToListenOn = new HashSet<string> { reverseProxyHost };
            if (hostName.StartsWith("http://localhost:"))
            {
                urlsToListenOn.Add(hostName);
            }
            
            return urlsToListenOn.ToArray();
        }

        /// <summary>
        /// Adds an Auth Section with a Key attribute to the appsettings.{currentEnvironment}.json file
        /// The Key's value is a random HMACSHA256 key encoded as a base 64 string
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        private static void GenerateAuthTokenKeyIfNotExists(IWebHostEnvironment hostingEnvironment)
        {
            IConfigurationRoot configuration = GetTempConfiguration();
            bool keyExists = !string.IsNullOrWhiteSpace(configuration["Auth:Key"]);
            if (keyExists)
            {
                Console.WriteLine("Key exists");
                return;
            }

            var hmacHash = new HMACSHA256();
            string key = Convert.ToBase64String(hmacHash.Key);
            string authSectionToAdd = $"\"Auth\":{{\"Key\":\"{key}\"}},";

            // get current environment and path to appsettings.{currentEnv}.json file
            string currentEnv = hostingEnvironment.EnvironmentName;
            string appsettingsEnvFilePath = Path.Combine(hostingEnvironment.ContentRootPath, $"appsettings.{currentEnv}.json");

            if (File.Exists(appsettingsEnvFilePath))
            {
                // If appsettings.{currentEnv}.json exists, add config section after the first bracket (beginning of json object)
                string configFileString = File.ReadAllText(appsettingsEnvFilePath);
                string modifiedConfigFileString = configFileString.Insert(configFileString.IndexOf('{') + 1, authSectionToAdd);
                File.WriteAllText(appsettingsEnvFilePath, modifiedConfigFileString);
            }
            else
            {
                Console.WriteLine($"Creating appsettings.{currentEnv}.json at {appsettingsEnvFilePath}");
                File.WriteAllText(appsettingsEnvFilePath, $"{{{authSectionToAdd}}}");
            }

        }

        private static IConfigurationRoot GetTempConfiguration()
        {
            // create temporary Webhost to access environment name
            var tempWebHostBuilding = WebHost.CreateDefaultBuilder();
            string environmentName = tempWebHostBuilding.GetSetting("environment");
            
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>()
                .Build();
        }

    }
}
