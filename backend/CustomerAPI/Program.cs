using CustomerAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CustomerAPI.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Use SQLite database for CustomerAPI. The connection string is read from the appsettings.json file.
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "CustomerDatabase")));

////Add DbContext for SQLite database on Linux container. The database file will be created in the /app/data directory.
//builder.Services.AddDbContext<CustomerDbContext>(
//    options =>
//        options.UseSqlite(
//            "Data Source=/app/data/customers.db"));

//used for local development
//builder.Services.AddDbContext<CustomerDbContext>(options =>
//    options.UseSqlite("Data Source=customers.db"));

builder.Services.AddScoped<RabbitMqPublisher>(

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("insurance");

//delibrated error for testing retry policy
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CustomerDbContext>();

    db.Database.EnsureCreated();

    if (!db.Customers.Any())
    {
        db.Customers.AddRange(
            new CustomerAPI.Models.Customer
            {
                Name = "Ravi Kumar",
                Email = "ravi@test.com"
            },
            new CustomerAPI.Models.Customer
            {
                Name = "Priya Sharma",
                Email = "priya@test.com"
            });

        db.SaveChanges();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
    Results.Ok(new
    {
        service = "CustomerAPI",
        status = "Healthy"
    }));

app.Run();