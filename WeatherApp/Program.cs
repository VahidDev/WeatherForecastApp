using WeatherApp.Models;
using WeatherApp.Services;
using WeatherApp.Services.ThirdPartyApis;
using WeatherApp.Services.ThirdPartyApis.WeatherMicroservice.Services.ThirdPartyApis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHttpClient();

builder.Services.Configure<ThirdPartyApiOptions>(builder.Configuration.GetSection("ThirdPartyApis"));

builder.Services.AddTransient<IThirdPartyWeatherApi, OpenWeatherApiService>();
builder.Services.AddTransient<IThirdPartyWeatherApi, WeatherbitApiService>();
builder.Services.AddTransient<IThirdPartyWeatherApi, WeatherApiService>();

builder.Services.AddSingleton<ICacheService, MemoryCache>();
builder.Services.AddMemoryCache();

builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
