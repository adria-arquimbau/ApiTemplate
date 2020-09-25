using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ApiTemplate.Values.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        private static string GetHostingEnvironment(WebHostBuilderContext hostingContext)
        {
            if (hostingContext == null)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return environment ?? "Production";
            }

            return hostingContext.HostingEnvironment.EnvironmentName;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    config.AddJsonFile(
                        Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "ConfigFiles",
                            $"appsettings.{GetHostingEnvironment(hostingContext)}.json"), false, true);
                    config.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
    }
}
