using System.Text.Json.Serialization;
using BookingAPI.src.Modules.Booking.Application.Services.Implementations;
using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);
string corsPolicySpecifiedOrigins = "_bookingAPIOrigins";
var databaseConnectionString = builder.Configuration.GetConnectionString("Default")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };

    options.MaximumPayloadBytes = 1024 * 1024; 
});

builder.Services.AddScoped<IHotelService, HotelService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddDbContext<BookingDbContext>(cfg =>
    cfg.UseNpgsql(databaseConnectionString));


builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


// defina suas origens aqui
List<string> origins = [
    "http://localhost:3000"
];

builder.Services.AddCors(options =>
{
   options.AddPolicy(corsPolicySpecifiedOrigins, policy =>
   {
        policy.WithOrigins([..origins])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
   });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
        options.RoutePrefix = "swagger";
    });

}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(corsPolicySpecifiedOrigins);
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

var url = "http://localhost:5174";

app.Logger.LogInformation($"\n API Docs running on  {url}/swagger  \n");

app.Run();
