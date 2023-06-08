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
using Microsoft.AspNetCore.Identity;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Entities.ConfigurationModels;
using Microsoft.OpenApi.Models;

namespace CodeMaze_API.Extensions
{
    public static class ServiceExtensions
    {
        #region CORS
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
        #endregion

        #region ISS
        public static void ConfigureISSIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }
        #endregion

        #region Logging
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        #endregion

        #region Repository & Service
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
        #endregion

        #region Custom Formatter (Content Negotiation)
        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(config => config
                                                    .OutputFormatters
                                                    .Add(new CsvOutputFormater()));
        }
        #endregion

        #region HATEOAS
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
        #endregion

        #region API Version
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
        #endregion

        #region Caching
        public static void ConfigureResponseCaching(this IServiceCollection services) =>
                services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
                services.AddHttpCacheHeaders((expirationOpt) =>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                }, (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                });
        #endregion

        #region RateLimit
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule> {
                new RateLimitRule {
                    Endpoint = "*",
                    Limit = 300,
                    Period = "5m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
        #endregion

        #region Identity
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders();
        }
        #endregion

        #region JWT
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // We can't inject IOptions here because the IOptions Service only availble
            // when whole services is register and construct done not before that
            // By the ways there is one way that we can inject service into this function
            // that is using IServiceCollection but this operation is a lot of expensive 
            // => We should use Bind here for better performance and cheap cost.
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind(JwtConfiguration.Section, jwtConfiguration);

            // use this line if we set SECRET in Environment
            //var jwtSecretKey = Environment.GetEnvironmentVariable("SECRET");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // The issuer is the actual server that created the token
                        ValidateIssuer = true,

                        // The receiver of the token is a valid recipient 
                        ValidateAudience = true,

                        // The token has not expired
                        ValidateLifetime = true,

                        // The signing key is valid and is trusted by the server
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtConfiguration.ValidIssuer,
                        ValidAudience = jwtConfiguration.ValidAudience,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey))
                    };
                }); 
        }
        
        public static void AddJWTConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.Section));
            services.Configure<JwtConfiguration>(configuration.GetSection("JwtAPI2Settings"));
        }
        #endregion

        #region Swagger
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Code Maze API",
                    Version = "v1",
                    Description = "CompanyEmployees API by CodeMaze",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "John Doe",
                        Email = "John.Doe@gmail.com",
                        Url = new Uri("https://twitter.com/johndoe"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "CompanyEmployees API LICX",
                        Url = new Uri("https://example.com/license"),
                    }

                });

                s.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Code Maze API",
                    Version = "v2"
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
            });
        }
        #endregion
    }
}
