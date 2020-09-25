using System;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiTemplate.Values.Api.Behaviors;
using ApiTemplate.Values.Domain.Proxies;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Domain.Repositories;
using ApiTemplate.Values.Infrastructure.CrossCutting.HealthCheck;
using ApiTemplate.Values.Infrastructure.CrossCutting.Swagger;
using ApiTemplate.Values.Infrastructure.Data.Repositories;
using ApiTemplate.Values.Infrastructure.Proxies.NumbersApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace ApiTemplate.Values.Api
{
    public static class StartupExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IValueItemRepository, ValueItemRepository>();
        }

        public static void AddPipelineBehaviors(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(GetValueItemHandler))
                .AddClasses(@class => @class.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces());
        }

        public static void AddApiHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddCheck<WhatchdogFileHealthCheck>("Watchdog File Check", HealthStatus.Unhealthy,
                    new[] { "watchdog", "file" });
        }

        public static void AddProxies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClientsWithRetryPolicy<INumbersProxy, NumbersProxy>(configuration.GetSection("ExternalServices:NumbersApi").Value);
        }
        private static void AddHttpClientsWithRetryPolicy<IT, T>(this IServiceCollection services, string url)
            where T : class, IT
            where IT : class
        {
            services.AddHttpClient<IT, T>(client =>
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Add("x-api-version", "1");
                })
                .AddPolicyHandler(GetHttpClientRetryPolicy<T>());
        }

        private static Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> GetHttpClientRetryPolicy<T>()
        {
            return (policyServices, request) =>
                HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(Backoff.LinearBackoff(TimeSpan.FromMilliseconds(200), 5),
                        (result, timeSpan, retryAttempt, context) =>
                        {
                            var logger = policyServices.GetService<ILogger<T>>();
                            logger.LogWarning(result.Exception, "Error on HTTP Client. Delaying for {Delay} then making retry: {RetryCount} {ResponseStatusCode}", timeSpan.TotalMilliseconds, retryAttempt, result.Result?.StatusCode);
                        }
                    );
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
