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

// Add services to the container.
// ----- Extensions Service -----
builder.Services.ConfigureCors();
builder.Services.ConfigureISSIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
// -- API Versioning
builder.Services.ConfigureVersioning();
// -- Caching
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
// -- HATEOAS
builder.Services.AddCustomMediaTypes();

// ----- Filter -----
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();

// ----- DI Class -----
builder.Services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
builder.Services.AddAutoMapper(typeof(Program));

// ----- API Config -----
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // This will disable default validate model state when using [ApiController] Attribute
    // => It will allow us to return customer error message
    options.SuppressModelStateInvalidFilter = true;
});
// ----- Rate Limit ----- (Because RateLimit use Mememory to store options, counter, rules)
// => We have to enable memoryCache
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();


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

app.MapControllers();

app.Run();
