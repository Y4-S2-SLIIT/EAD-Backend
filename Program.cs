using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EADBackend.Models;
using EADBackend.Services;
using EADBackend.Services.Interfaces;
using System.Text;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotEnv.Load();

// Add configuration to the service collection
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configure MongoDB settings from environment variables
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")
        ?? throw new ArgumentException("ConnectionString environment variable is not set.", "MONGODB_CONNECTION_STRING");
    options.DatabaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")
        ?? throw new ArgumentNullException("MONGODB_DATABASE_NAME", "DatabaseName environment variable is not set.");
});

// Register MongoDB client and database
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new ArgumentNullException("JWT_SECRET", "Secret environment variable is not set.");

    var keyBytes = Encoding.UTF8.GetBytes(secretKey);

    // Ensure key length is at least 256 bits (32 bytes)
    if (keyBytes.Length < 32)
    {
        throw new ArgumentException("JWT_SECRET must be at least 256 bits (32 bytes) long.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? throw new ArgumentNullException("JWT_ISSUER", "Issuer environment variable is not set."),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? throw new ArgumentNullException("JWT_AUDIENCE", "Audience environment variable is not set."),
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.FromMinutes(double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_IN_MINUTES")
            ?? throw new ArgumentNullException("JWT_EXPIRATION_IN_MINUTES", "JWT expiration environment variable is not set.")))
    };
});

// Add authorization
builder.Services.AddAuthorization();

// Add controllers
builder.Services.AddControllers();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();