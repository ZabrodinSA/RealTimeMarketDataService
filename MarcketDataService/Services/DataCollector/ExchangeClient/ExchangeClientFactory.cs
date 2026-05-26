namespace MarcketDataService.Services.DataCollector.ExchangeClient;

public class ExchangeClientFactory : IExchangeClientFactory
{
    public IExchangeClient Create(ExchangeConfig config)
    {
        return new SimpleExchangeClient(config);
    }
}