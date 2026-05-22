using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

string[] symbols =
[
    "BTCUSDT",
    "ETHUSDT",
    "BNBUSDT",
    "XRPUSDT",
    "ADAUSDT",
    "SOLUSDT",
    "DOTUSDT",
    "DOGEUSDT",
    "MATICUSDT",
    "LTCUSDT"
];

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Exchanges");
foreach (var filePath in Directory.GetFiles(folderPath, "*.json"))
{
    string fileName = Path.GetFileNameWithoutExtension(filePath);
    app.Map("/ws/{exchange}", async context =>
    {

    });
    app.Map($"/ws/{fileName}", async context  =>
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await SendGeneratedData(webSocket, filePath);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    });
}

app.Run();

async Task SendGeneratedData(WebSocket webSocket, string filePath)
{
    string template = await File.ReadAllTextAsync(filePath);
    var random = new Random();

    while (webSocket.State == WebSocketState.Open)
    {
        string generatedData = GenerateDataFromTemplate(template, random);
        var buffer = Encoding.UTF8.GetBytes(generatedData);
        await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, default);
        await Task.Delay(1000); // Отправка данных раз в секунду
    }
}

string GenerateDataFromTemplate(string template, Random random)
{
    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(template);
    foreach (var key in data.Keys)
    {
        if (data[key] is JsonElement value)
        {
            data[key] = value.ToString() switch
            {
                "{ticker}" => symbols[random.Next(symbols.Length)],
                "{number}" => random.NextDouble() * 100,
                "{ISO 8601 datetime}" => DateTime.UtcNow,
                _ => value
            };
        }
    }
    return JsonSerializer.Serialize(data);
}