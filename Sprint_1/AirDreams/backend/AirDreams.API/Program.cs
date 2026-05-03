using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using AirDreams.API.Repositories;
using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IRepository<UserDTO>, UserRepository>();
builder.Services.AddScoped<IService<UserDTO>, UserService>();
builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAirportService, AirportService>();

//builder.Services.AddScoped<IService<AirportDTO>, AirportService>();
//builder.Services.AddScoped<IService<AircraftDTO>, AircraftService>();
//builder.Services.AddScoped<IService<UserDTO>, UserService>();
//builder.Services.AddScoped<IService<RouteDTO>, RouteService>();
//builder.Services.AddScoped<IService<FlightDTO>, FlightService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();
app.Run();