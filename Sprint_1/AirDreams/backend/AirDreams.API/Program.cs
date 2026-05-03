using System.Data;
using Microsoft.Data.SqlClient; //Cambiar a Microsoft.Data.SqlCl
using Dapper;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

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

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("AirDreamsContext")));

builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAirportService, AirportService>();

// Comentar por ahora hasta que existan bien estos servicios
// builder.Services.AddScoped<IService<AirportDTO>, AirportService>();
// builder.Services.AddScoped<IService<AircraftDTO>, AircraftService>();
// builder.Services.AddScoped<IService<UserDTO>, UserService>();
// builder.Services.AddScoped<IService<RouteDTO>, RouteService>();
// builder.Services.AddScoped<IService<FlightDTO>, FlightService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    connection.Open();
    connection.Execute(@"CREATE TABLE IF NOT EXISTS Airport (
                        codeAirport TEXT PRIMARY KEY,
                        adminID INTEGER NOT NULL,
                        nameAirport TEXT NOT NULL,
                        city TEXT NOT NULL,
                        country TEXT NOT NULL,
                        timeZone TEXT
                        )");
}

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