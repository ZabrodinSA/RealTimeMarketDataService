namespace MarcketDataService.Models;

public class TickerModel
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    private DateTime _timestamp;

    public DateTime Timestamp
    {
        get => _timestamp;
        set => _timestamp = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    public string ExchangeName { get; set; } 

    public override int GetHashCode()
    {
        return HashCode.Combine(Symbol, ExchangeName, Timestamp);
    }
}