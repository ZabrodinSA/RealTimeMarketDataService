using DotNetEnv;
using MarcketDataService.Repositories.Ticker;
using MarcketDataService.Services;
using MarcketDataService.Services.DataCollector;
using MarcketDataService.Services.DataCollector.ExchangeClient;
using MarcketDataService.Services.DataCollector.TickerProcessor;
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
        $"Host={dbHost};Port=5432;Database=tickers;Username={dbUsername};Password={dbPassword}"));
builder.Services.AddSingleton<ITickerRepository, EfTickerRepository>();

// Add configs
builder.Services.Configure<List<ExchangeConfig>>(builder.Configuration.GetSection("Exchanges"));
builder.Services.Configure<List<string>>(builder.Configuration.GetSection("SymbolNames"));
builder.Services.Configure<List<string>>(builder.Configuration.GetSection("PriceNames"));
builder.Services.Configure<List<string>>(builder.Configuration.GetSection("VolumeNames"));
builder.Services.Configure<List<string>>(builder.Configuration.GetSection("TimestampNames"));

//Add data collector service
builder.Services.AddSingleton<IDataCollectorService, DataCollectorService>();
builder.Services.AddHostedService<DataCollectorService>();

builder.Services.AddSingleton<IExchangeClientFactory, ExchangeClientFactory>();
builder.Services.AddSingleton<ITickerProcessor, TickerProcessor>();
builder.Services.AddLogging();

var app = builder.Build();

app.UseWebSockets();
app.Run();