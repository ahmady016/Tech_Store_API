using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Serilog;
using MediatR;

using TechStoreApi.Common;
using TechStoreApi.Auth;
using TechStoreApi.DB;
using TechStoreApi.Entities;

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
// Register the db context
var dbConnection = builder.Configuration.GetConnectionString("Local");
builder.Services.AddDbContext<TechStoreDB>(options => options.UseSqlServer(dbConnection));

// Register the Identity
builder.Services.AddIdentity<User, Role>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 3;
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
    options.SaveToken = true;
    options.TokenValidationParameters = JwtService.GetTokenValidationOptions(validateLifetime: true);
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

// Register DB Services
builder.Services.AddScoped<IDBService, DBService>();
builder.Services.AddScoped<IRawDBService, RawDBService>();

// Register AuthService
// builder.Services.AddScoped<IAuthService, AuthService>();

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
