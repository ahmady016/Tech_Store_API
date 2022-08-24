using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MediatR;
using Serilog;

using DB;
using Common;

// Create the Web Server Builder
var builder = WebApplication.CreateBuilder(args);

#region Configure Serilog Logging:
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.AddSerilog(logger);
#endregion

#region Add services to the container:

// Register the db context
var dbConnection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<TechStoreDB>(options => options.UseSqlServer(dbConnection));

// Register DB and CrudService
builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<ICrudService, CrudService>();

// Register AutoMapper
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(assemblies);
// Register MediatR
var assembly = AppDomain.CurrentDomain.Load("_Tech_Store_API");
builder.Services.AddMediatR(x => x.AsScoped(), assembly);

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

// Build the Web Server
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

// setup API routes
app.UseRouting();
// API routes
app.MapControllers();

// handle client side routes [catch all routes for SPA]
app.MapFallbackToFile("index.html");

// run the Web Server
app.Run();

#endregion
