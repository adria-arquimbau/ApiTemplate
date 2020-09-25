using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Connections;

namespace ApiTemplate.Values.Infrastructure.Data.Tests
{
    public class TestContext
    {
        public readonly ValueItemDbContext ValueItemDbContext;

        public TestContext()
        {
            CheckDatabaseIsForUnitTesting();

            var currentDirectory = GetApplicationRoot();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(currentDirectory);
            configurationBuilder.AddJsonFile(
                Path.Combine(currentDirectory, "ConfigFiles",
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"), false,
                reloadOnChange: true);
            configurationBuilder.AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();

            var options = new DbContextOptionsBuilder<ValueItemDbContext>()
                .UseNpgsql(configuration.GetConnectionString("DBConnection"))
                .Options;
            ValueItemDbContext = new ValueItemDbContext(options);
        }

        private void CheckDatabaseIsForUnitTesting()
        {
            var currentDirectory = GetApplicationRoot();
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(currentDirectory);
            configurationBuilder.AddJsonFile(
                Path.Combine(currentDirectory, "ConfigFiles",
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"), false,
                reloadOnChange: true);
            configurationBuilder.AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();
            var connectionString = configuration.GetConnectionString("DBConnection");

            ValidateTestingEnvironment(connectionString);
        }

        private void ValidateTestingEnvironment(string connectionString)
        {
            if (!connectionString.Contains("localhost") && !connectionString.Contains("Jenkins"))
            {
                throw new ConnectionAbortedException(
                    "This database is for web testing only. Testing it will reset all its data. Please use localhost or Jenkins assigned database");
            }
        }

        public async Task RespawnDb()
        {
            var con = ValueItemDbContext.Database.GetDbConnection();

            await con.OpenAsync();

            await RespawnCheckpoint.Checkpoint().Reset(con);

        }

        public ValueItemDbContext TestDbContext()
        {
            return ValueItemDbContext;
        }

        public IDbConnection TestDbConnection()
        {
            return ValueItemDbContext.Database.GetDbConnection();
        }

        public string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                .Assembly.GetExecutingAssembly().CodeBase);
            var appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot + "\\..\\..\\src\\ApiTemplate.Values.Api";
        }
    }
}
