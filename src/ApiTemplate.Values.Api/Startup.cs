using System;
using System.Net.Http;
using System.Reflection;
using ApiTemplate.Values.Api.ProblemDetails;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ApiTemplate.Values.Domain.Exceptions;
using ApiTemplate.Values.Domain.Queries.GetValueItem;
using ApiTemplate.Values.Infrastructure.CrossCutting.Logging;
using ApiTemplate.Values.Infrastructure.Data;

namespace ApiTemplate.Values.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(GetValueItemHandler).GetTypeInfo().Assembly);
            services.AddApiHealthChecks();
            services.AddRepositories();
            services.AddPipelineBehaviors();
            services.AddProblemDetails(pbo => ConfigureProblemDetails(pbo, Environment));
            services.AddControllers();
            services.AddVersioning();
            services.AddSwagger();
            services.AddDbContext<ValueItemDbContext>(options => { options.UseNpgsql(Configuration.GetConnectionString("DBConnection")); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMiddleware<LogContextMiddleware>(); // This should go in this order
            app.UseSerilogRequestLogging(opts => opts.GetLevel = StartupExtensions.CustomGetLevel); // do not log healthcheck

            app.UseProblemDetails();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = StartupExtensions.WriteResponse
            });
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });

        }

        private static void ConfigureProblemDetails(ProblemDetailsOptions options, IWebHostEnvironment environment)
        {
            // This is the default behavior; only include exception details in a development environment.
            options.IncludeExceptionDetails = (ctx, ex) => environment.IsDevelopment();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.Map<ValueItemNotFound>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status404NotFound));

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));

            options.Map<KeyTooLongException>(ex => new KeyTooLongProblemDetails
            {
                Title = ex.Message,
                Detail = ex.Key,
                Length = ex.Length,
                Status = StatusCodes.Status403Forbidden,
                Type = ex.Type,
                Key = ex.Key
            });

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
        }


    }
}
