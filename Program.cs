using System.Text.Json.Serialization;
using BookingAPI.src.Modules.Booking.Application.Services.Implementations;
using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  

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
app.MapControllers();

app.Logger.LogInformation("API Docs running on http://localhost:5174/swagger ");

app.Run();

