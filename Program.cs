using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using MediatR;
using Serilog;

using DB;
using Common;

using Products.Queries;
using Products.Commands;

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
var assembly = AppDomain.CurrentDomain.Load("Tech_Store_API");
builder.Services.AddMediatR(x => x.AsScoped(), assembly);

// Allow CORS
builder.Services.AddCors();

// Configure JSON options.
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.IncludeFields = true;
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.AllowTrailingCommas = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
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

#region register all endpoints:

#region Products:
app.MapGetRequest<ListProductQuery>("api/products/list");
app.MapGetRequest<FindProductQuery>("api/products/find");
app.MapPostRequest<AddProductCommand>("api/products/add");
app.MapPutRequest<UpdateProductCommand>("api/products/update");
#endregion

#endregion

// handle client side routes [catch all routes for SPA]
app.MapFallbackToFile("index.html");

// run the Web Server
app.Run();

#endregion
