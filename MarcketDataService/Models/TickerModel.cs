namespace MarcketDataService.Models;

public class TickerModel
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    public DateTime Timestamp { get; set; } 
    public string ExchangeName { get; set; } 

    public override int GetHashCode()
    {
        return HashCode.Combine(Symbol, ExchangeName, Timestamp);
    }
}