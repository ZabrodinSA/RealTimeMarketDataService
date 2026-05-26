namespace MarcketDataService.Services.DataCollector.ExchangeClient;

public class ExchangeConfig
{
    public string Name { get; set; }
    public string Url { get; set; }
    public List<string> SymbolNames { get; set; }
    public List<string> PriceNames { get; set; }
    public List<string> VolumeNames { get; set; }
    public List<string> TimestampNames { get; set; }
}