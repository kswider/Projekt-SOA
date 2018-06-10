using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.Extensions.Logging.AzureAppServices.Internal;
using Microsoft.Extensions.Logging.Debug;

namespace Toulbar2RestCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"certificate.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .Build();

            var certificateSettings = config.GetSection("certificateSettings");
            string certificateFileName = certificateSettings.GetValue<string>("filename");
            string certificatePassword = certificateSettings.GetValue<string>("password");

            var certificate = new X509Certificate2(certificateFileName, certificatePassword);
            BuildWebHost(args, certificate).Run();
        }

        public static IWebHost BuildWebHost(string[] args, X509Certificate2 certificate) =>
        new WebHostBuilder()
                .UseKestrel(
                    options =>
                    {
                        options.AddServerHeader = false;
                        options.Listen(IPAddress.Any, 443, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                    }
                )
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    //logging.AddDebug();
                    logging.AddFilter("System", LogLevel.Debug)
                        .AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Error);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
