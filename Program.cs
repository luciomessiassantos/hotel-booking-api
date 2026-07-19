using System.Text.Json.Serialization;
using BookingAPI.src.Modules.Booking.Application.Services.Implementations;
using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);
string corsPolicySpecifiedOrigins = "_bookingAPIOrigins";
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

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
    cfg.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));


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

// Configure the HTTP request pipeline.
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

app.Logger.LogInformation("API Docs running on http://localhost:5174/swagger ");

app.Run();
