using CodeMaze_API.Formatter;
using Contacts.Interfaces;
using LoggerService;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;
using Azure;

namespace CodeMaze_API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors((CorsOptions options) =>
            {
                options.AddPolicy("MyCorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .WithMethods("POST", "GET", "PUT", "DELETE")
                            // This will allow client (FE) to read X-Pagination Headers
                            .WithExposedHeaders("X-Pagination");
                });
            });
        }

        public static void ConfigureISSIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }
        
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SQL-Server"));
            });

            // Shouldn't use this approach because it will be hard to config more AddDbContext options
            //services.AddSqlServer<RepositoryContext>(configuration.GetConnectionString("SQL-Server"));
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(config => config
                                                    .OutputFormatters
                                                    .Add(new CsvOutputFormater()));
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            // We are registering two new custom media types for the JSON and XML output formatters.
            // This will ensures we won’t get a 406 Not Acceptable response when sending
            // CustomMediaType like this application/vnd.codemaze.hateoas+json.
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config.OutputFormatters
                            .OfType<SystemTextJsonOutputFormatter>()
                            ?.FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    // This Header will be used when client want using HATEOAS
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                        .Add("application/vnd.codemaze.hateoas+json");

                    // This Header will be used when client want fetch link from API Root
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                        .Add("application/vnd.codemaze.apiroot+json");
                };

                var xmlOutputFormatter = config.OutputFormatters
                            .OfType<XmlDataContractSerializerOutputFormatter>()
                            ?.FirstOrDefault();
                
                if (xmlOutputFormatter != null)
                {
                    // This Header will be used when client want using HATEOAS
                    xmlOutputFormatter.SupportedMediaTypes
                        .Add("application/vnd.codemaze.hateoas+xml");

                    // This Header will be used when client want fetch link from API Root
                    xmlOutputFormatter.SupportedMediaTypes
                        .Add("application/vnd.codemaze.apiroot+xml");
                };
            });
        }

    }
}
