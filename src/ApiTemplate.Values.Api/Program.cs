using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Graylog;

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
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    var configuration = hostingContext.Configuration;
                    loggerConfiguration
                        .Enrich.FromLogContext()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Information()
                        .WriteTo.Console(
                            LogEventLevel.Information,
                            "{NewLine}{Timestamp:HH:mm:ss} [{Level:u3}] {Message} {Exception}")
                        .WriteTo.File(
                            outputTemplate:
                            "{NewLine}{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}): {Message} {Exception}",
                            path: "c:\\temp\\templatewebapi.txt",
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day,
                            shared: true)
                        .WriteTo.Graylog(new GraylogSinkOptions
                        {
                            HostnameOrAddress = configuration.GetValue<string>("Graylog:URL"),
                            Port = configuration.GetValue<int>("Graylog:Port"),
                            Facility = configuration.GetValue<string>("Graylog:Facility")
                        });
                });
    }
}
