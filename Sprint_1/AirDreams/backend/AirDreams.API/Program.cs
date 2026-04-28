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

builder.Services.AddScoped<IService<AirportDTO>, AirportService>();
builder.Services.AddScoped<IService<AircraftDTO>, AircraftService>();
builder.Services.AddScoped<IService<UserDTO>, UserService>();
builder.Services.AddScoped<IService<RouteDTO>, RouteService>();
builder.Services.AddScoped<IService<FlightDTO>, FlightService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowVue");

app.UseAuthorization();

app.MapControllers();

app.Run();