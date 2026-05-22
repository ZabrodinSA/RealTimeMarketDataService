using DotNetEnv;
using MarcketDataService.Repositories.Ticker;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

//Add DB contexts
var dbUsername = builder.Configuration["DB_USERNAME"];
var dbPassword = builder.Configuration["DB_PASSWORD"];
var dbHost = builder.Configuration["BD_HOST"];

if (string.IsNullOrEmpty(dbUsername) || string.IsNullOrEmpty(dbPassword) || string.IsNullOrEmpty(dbHost))
{
    throw new InvalidOperationException("Database configuration is missing. Please set DB_USERNAME, DB_PASSWORD, and BD_HOST environment variables.");
}

builder.Services.AddPooledDbContextFactory<TickerContext>(options =>
    options.UseNpgsql(
        $"Host={dbHost};Port=5432;Database=tasks;Username={dbUsername};Password={dbPassword}"));
builder.Services.AddSingleton<ITickerRepository, EfTickerRepository>();
var app = builder.Build();

app.UseWebSockets();
app.Run();