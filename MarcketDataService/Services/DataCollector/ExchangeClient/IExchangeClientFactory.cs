namespace MarcketDataService.Services.DataCollector.ExchangeClient;

public interface IExchangeClientFactory
{
    IExchangeClient Create(ExchangeConfig config);
}