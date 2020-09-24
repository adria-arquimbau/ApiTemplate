using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiTemplate.Values.PublicApi.HttpClients;
using ApiTemplate.Values.PublicApi.Settings;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using ApiTemplate.Values.Infrastructure.CrossCutting.HealthCheck;
using ApiTemplate.Values.Infrastructure.CrossCutting.Swagger;

namespace ApiTemplate.Values.PublicApi
{
    public static class StartupExtensions
    {
        public static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            return services;
        }


        public static void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider,
            Func<IApplicationBuilder, IApplicationBuilder> configureHost)

        {
            configureHost(app);

            app.UseRouting();
            app.UseProblemDetails();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = WriteResponse
            });
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                });
        }

        public static IServiceCollection AddApiHealthChecks(this IServiceCollection services,
            IConfiguration configuration)
        {
            var apisSettingsSection = configuration.GetSection("Apis");
            var apisSettings = apisSettingsSection.Get<ApiSettings>();

            services
                .AddHealthChecks()
                .AddCheck<WhatchdogFileHealthCheck>("Watchdog File Check", HealthStatus.Unhealthy,
                    new[] { "watchdog.html", "file" })
                .AddUrlGroup(new Uri($"{apisSettings.ValueItemsApi}/health"), HttpMethod.Get, "Internal Values API",
                    HealthStatus.Unhealthy, new[] { "url", "api", "values internal api" });

            return services;
        }

        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var apisSettingsSection = configuration.GetSection("Apis");
            var apisSettings = apisSettingsSection.Get<ApiSettings>();

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        // Add logging?
                    }
                );

            // Inject the client depending on the version? Could we do that here?
            services
                .AddHttpClient<IValuesApiClient, ValuesApiClient>(client =>
                {
                    client.BaseAddress = new Uri(apisSettings.ValueItemsApi);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "ValuesWebApi");
                })
                .AddPolicyHandler(retryPolicy);

            return services;
        }

        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    // integrate xml comments -> do we need to do this? I don't think so.
                    // options.IncludeXmlComments(XmlCommentsFilePath);
                });
        }


        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var path = ctx?.Request?.Path.Value;
            if (path != null)
            {
                if (path.ToLower().Contains("watchdog"))
                    return true;
                if (path.ToLower().Contains("/health"))
                    return true;
            }
            // No endpoint, so not a health check endpoint
            return false;
        }

        /// <summary>
        /// Nivel por defecto según github, pero si es healthcheck se cambia a verbose
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="_"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static LogEventLevel CustomGetLevel(HttpContext ctx, double _, Exception ex) =>
               ex != null
                    ? LogEventLevel.Error
                    : ctx.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                            ? LogEventLevel.Debug   // Was a health check, use Verbose
                            : LogEventLevel.Information;

        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                                    typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());

                return context.Response.WriteAsync(json);
            }
        }
    }
}