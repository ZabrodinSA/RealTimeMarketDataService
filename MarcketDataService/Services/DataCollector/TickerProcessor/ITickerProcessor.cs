using MarcketDataService.Models;

namespace MarcketDataService.Services.DataCollector.TickerProcessor;

public interface ITickerProcessor
{
    Task ProcessAsync(TickerModel ticker);
}