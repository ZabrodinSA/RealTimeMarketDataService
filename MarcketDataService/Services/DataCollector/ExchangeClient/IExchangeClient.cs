using MarcketDataService.Models;

namespace MarcketDataService.Services.DataCollector.ExchangeClient;

public interface IExchangeClient
{
    string Name { get; }
    
    Task ConnectAsync(CancellationToken ct);
    
    event Func<TickerModel, Task> OnMessage;
    event Action<string> OnError;
    
    Task DisconnectAsync();
}