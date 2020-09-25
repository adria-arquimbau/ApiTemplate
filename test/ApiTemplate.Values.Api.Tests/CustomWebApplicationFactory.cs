using System;
using System.IO;
using System.Threading.Tasks;
using ApiTemplate.Values.Infrastructure.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;

namespace ApiTemplate.Values.Api.Tests
{
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()   
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<TStartup>();
        }
        // move to configuration
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                config.AddJsonFile(
                    Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "ConfigFiles",
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"), false,
                    reloadOnChange: true);
                config.AddEnvironmentVariables();
            });

            builder.ConfigureServices(services =>
            {
                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    try
                    {
                        // Seed the database with test data.
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = this.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public async Task ExecuteDbContextAsync(Func<ValueItemDbContext, Task> action)
        {
            await ExecuteScopeAsync(sp => action(sp.GetService<ValueItemDbContext>()));
        }

        private readonly Checkpoint _checkpoint = new Checkpoint
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new []{ "public" }
        };

        public async Task RespawnDbContext()
        {
            await ExecuteDbContextAsync(async context =>
            {
                var con = context.Database.GetDbConnection();
                await con.OpenAsync();
                await _checkpoint.Reset(con);
            });
        }
    }
}
