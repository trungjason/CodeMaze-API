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

// ----- Controller Config -----
builder.Services.AddControllers(configs =>
{
    configs.RespectBrowserAcceptHeader = true;
    configs.ReturnHttpNotAcceptable = true;

    configs.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
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

app.UseCors("MyCorsPolicy");

app.MapControllers();

app.Run();
