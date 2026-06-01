using AirDreams.ExternalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("InternalAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["InternalApi:BaseUrl"]!);
});

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");
app.MapControllers();
app.Run();