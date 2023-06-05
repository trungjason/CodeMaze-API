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
using Microsoft.AspNetCore.Mvc.Versioning;
using Presentation.Controllers.V2;
using Presentation.Controllers;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;

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

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                // adds the API version to the response header.
                opt.ReportApiVersions = true;

                //It specifies the default API version if the client doesn’t send one.
                opt.AssumeDefaultVersionWhenUnspecified = true;

                // Set DefaultAPI version
                opt.DefaultApiVersion = new ApiVersion(1, 0);

                // We can choose which api version to use by setting
                // api-version header or using URI to specify API version
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");

                // If we don't want to apply [ApiVersion] Attribute to all
                // controller that we have. 
                // We can config their version in this function like below 
                opt.Conventions.Controller<CompaniesController>()
                                .HasApiVersion(new ApiVersion(1, 0));

                opt.Conventions.Controller<CompaniesV2Controller>()
                                .HasDeprecatedApiVersion(new ApiVersion(2, 0));

            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
                services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
                services.AddHttpCacheHeaders((expirationOpt) =>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },(validationOpt) => {
                    validationOpt.MustRevalidate = true;
                });

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule> {
                new RateLimitRule {
                    Endpoint = "*",
                    Limit = 3,
                    Period = "5m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt => {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

    }
}
