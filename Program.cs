using System.Text.Json.Serialization;
using Serilog;
using MediatR;
using TechStoreApi.Common;

// create the web server builder
var builder = WebApplication.CreateBuilder(args);

#region Configure Serilog Logging:
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
#endregion

#region Add services to the container:
// Register IHttpContextAccessor to get access to the HttpContext.
builder.Services.AddHttpContextAccessor();

// Register BaseUrl and SMTP from appsettings.json
builder.Services.Configure<BaseUrl>(builder.Configuration.GetSection("BaseUrl"));
builder.Services.Configure<MailOptions>(builder.Configuration.GetSection("SMTP"));

// Register EmailService
builder.Services.AddSingleton<IEmailService, EmailService>();
// Register FileService
builder.Services.AddSingleton<IFileService, FileService>();
// Register ResultService
builder.Services.AddSingleton<IResultService, ResultService>();

// Register MediatR
builder.Services.AddMediatR(AppDomain.CurrentDomain.Load("Tech_Store_API"));

// Allow CORS
builder.Services.AddCors();

// Add API Controllers and Configure JSON options.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IncludeFields = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
    });

// Register swagger APIs docs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

// build the web server
var app = builder.Build();

#region Configure the HTTP request pipeline:
// global exception handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// swagger API docs
app.UseSwagger();
app.UseSwaggerUI();

// allow CORS
app.UseCors(config => config
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
);

// static files
app.UseStaticFiles();

// auth
app.UseAuthentication();
app.UseAuthorization();

// API routes
app.MapControllers();

// handle client side routes [catch all routes for SPA]
app.MapFallbackToFile("index.html");

// run the Web Server
app.Run();
#endregion
