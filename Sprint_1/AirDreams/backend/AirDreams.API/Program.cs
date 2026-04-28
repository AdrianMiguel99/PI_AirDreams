using AirDreams.API.DTOs;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.Repositories;
using AirDreams.API.Repositories.Interfaces;

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