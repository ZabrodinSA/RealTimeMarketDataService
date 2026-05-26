using MarcketDataService.Services.DataCollector.ExchangeClient;
using MarcketDataService.Services.DataCollector.TickerProcessor;
using Microsoft.Extensions.Options;

namespace MarcketDataService.Services.DataCollector;

public class DataCollectorService(
    IOptions<List<ExchangeConfig>> configsOptions,
    IOptions<List<string>> symbolNames,
    IOptions<List<string>> priceNames,
    IOptions<List<string>> volumeNames,
    IOptions<List<string>> timestampNames,
    IExchangeClientFactory factory,
    ITickerProcessor processor,
    ILogger<DataCollectorService> logger)
    : IDataCollectorService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var config in configsOptions.Value)
        {
            config.SymbolNames = symbolNames.Value;
            config.PriceNames = priceNames.Value;
            config.VolumeNames = volumeNames.Value;
            config.TimestampNames = timestampNames.Value;
            var client = factory.Create(config);
            
            client.OnMessage += async (raw) =>
            {
                await processor.ProcessAsync(raw);
            };

            client.OnError += message => { logger.LogError(message); };
            
            Task.Run(() => RunClient(client, cancellationToken), cancellationToken);
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("App is stopping...");
        return Task.CompletedTask;
    }
    
    private async Task RunClient(IExchangeClient client, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation($"Connecting to {client.Name}");
                await client.ConnectAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {client.Name}");
            }

            logger.LogWarning($"{client.Name} disconnected. Reconnecting in 5s...");
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }
}