using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MarcketDataService.Models;

namespace MarcketDataService.Services.DataCollector.ExchangeClient;

public class SimpleExchangeClient(ExchangeConfig config) : IExchangeClient
{
    private readonly ClientWebSocket _socket = new();

    public string Name => config.Name;

    public event Func<TickerModel, Task>? OnMessage;
    public event Action<string>? OnError;

    public async Task ConnectAsync(CancellationToken ct)
    {
        await _socket.ConnectAsync(new Uri(config.Url), ct);
        await ReceiveLoop(ct);
    }

    public async Task DisconnectAsync()
    {
        if (_socket.State == WebSocketState.Open)
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        var buffer = new byte[8192];

        while (_socket.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var result = await _socket.ReceiveAsync(buffer, ct);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            try
            {
                var ticker = Parse(message);
                await OnMessage?.Invoke(ticker)!;

            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error parsing JSON: {message} \n message: {e.Message}");
            }
        }
    }

    private TickerModel Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var symbol = ExtractString(root, config.SymbolNames);
        var price = ExtractDecimal(root, config.PriceNames);
        var volume = ExtractDecimal(root, config.VolumeNames);
        var timestamp = ExtractTimestamp(root, config.TimestampNames);

        return new TickerModel()
        {
            Symbol = symbol,
            Price = price,
            Volume = volume,
            Timestamp = timestamp,
            ExchangeName = Name
        };
    }


    private decimal ExtractDecimal(JsonElement root, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!TryGetProperty(root, name, out var value)) continue;
            if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var d))
                return d;

            if (value.ValueKind == JsonValueKind.String &&
                decimal.TryParse(value.GetString(), out var parsed))
                return parsed;
        }
        
        throw new NullReferenceException();
    }

    private string ExtractString(JsonElement root, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!TryGetProperty(root, name, out var value)) continue;
            if (value.ValueKind != JsonValueKind.String) continue;
            
            var result = value.GetString();
            if (result == null) continue;    
            
            return result;
        }

        throw new NullReferenceException();
    }

    private DateTime ExtractTimestamp(JsonElement root, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            if (!TryGetProperty(root, name, out var value)) continue;
            
            if (value.ValueKind == JsonValueKind.Number &&
                value.TryGetInt64(out var unix))
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;
            }

            if (value.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(value.GetString(), out var dt))
            {
                return dt;
            }
        }
        
        throw new NullReferenceException();
    }

    private bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        if (element.TryGetProperty(name, out value))
            return true;

        foreach (var prop in element.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                if (TryGetProperty(prop.Value, name, out value))
                    return true;
            }
        }

        value = default;
        return false;
    }
}