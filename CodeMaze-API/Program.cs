using CodeMaze_API.Extensions;
using Contacts.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Presentation.ActionFilters;
using Service.DataShaping;
using Shared.DataTransferObjects;
using CodeMaze_API.Utility;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// ----- Setup ------
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Local function: This function configures support for JSON Patch using 
// Newtonsoft.Json while leaving the other formatters unchanged.
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();

// ----- SERVICE CONFIGURATION ------
#region Extension Service
// ----- Extensions Service -----
builder.Services.ConfigureCors();
builder.Services.ConfigureISSIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
#endregion

#region API Versioning
// -- API Versioning
builder.Services.ConfigureVersioning();
#endregion

#region Caching
// -- Caching
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
#endregion

#region HATEOAS
// -- HATEOAS
builder.Services.AddCustomMediaTypes();
#endregion

#region Filter
// ----- Filter -----
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
#endregion

#region DI Class
// ----- DI Class -----
builder.Services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
builder.Services.AddAutoMapper(typeof(Program));
#endregion

#region API Config
// ----- API Config -----
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // This will disable default validate model state when using [ApiController] Attribute
    // => It will allow us to return customer error message
    options.SuppressModelStateInvalidFilter = true;
});
#endregion

#region Rate Limit
// ----- Rate Limit ----- 
// (Because RateLimit use Mememory to store options, counter, rules) => We have to enable memoryCache
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
#endregion

#region Identity
// ---- Identity -----
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
#endregion

#region Controller Config
// ----- Controller Config -----
builder.Services.AddControllers(configs =>
{
    // RespectBroweser Accept Header told our server 
    // to read Accept header and choose the right InputFormatter.
    // If we don't set to true the default InputFormatter will always be JSON
    configs.RespectBrowserAcceptHeader = true;
    configs.ReturnHttpNotAcceptable = true;

    configs.InputFormatters.Insert(0, GetJsonPatchInputFormatter());

    // We don't have to config ResponsCaching attribute at all Action in Controller
    // we can create a CacheProfile with all setting and option that we want to config
    // then in Action we can reuse this CacheProfile
    configs.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
})
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
#endregion

#region JWT
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJWTConfiguration(builder.Configuration);
#endregion

#region Swagger
builder.Services.ConfigureSwagger();
#endregion

// ----- HTTP Request Pipeline -----
var app = builder.Build();

var logger = app.Services.GetService<ILoggerManager>();

// Can't use app.UseDeveloperExceptionPage(); because this is WEB API
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseIpRateLimiting();

app.UseCors("MyCorsPolicy");

// Microsoft recommend we should use responseCaching after CORS middleware
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
