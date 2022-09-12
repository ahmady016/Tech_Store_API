using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Serilog;

using DB;
using Common;
using Entities;
using Auth;

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
// Pass Configuration to AuthHelpers static class
AuthHelpers.Initialize(builder.Configuration);

// Register the db context
var dbConnection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<TechStoreDB>(options => options.UseSqlServer(dbConnection));

// Register the Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
  {
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
  })
  .AddEntityFrameworkStores<TechStoreDB>()
  .AddDefaultTokenProviders();

  // Register Auth
builder.Services.AddAuthentication(options =>
  {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = AuthHelpers.GetTokenValidationOptions(validateLifetime: true);
      options.Events = new JwtBearerEvents()
      {
        OnAuthenticationFailed = context =>
        {
          if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            context.Response.Headers.Add("Token-Expired", "true");
          return Task.CompletedTask;
        }
      };
  });
builder.Services.AddAuthorization();

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

// Auth
app.UseAuthentication();
app.UseAuthorization();

// API routes
app.MapControllers();

// handle client side routes [catch all routes for SPA]
app.MapFallbackToFile("index.html");

// run the Web Server
app.Run();

#endregion
