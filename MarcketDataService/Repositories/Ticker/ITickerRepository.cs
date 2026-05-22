using MarcketDataService.Models;

namespace MarcketDataService.Repositories.Ticker;

public interface ITickerRepository
{
    Task AddTicker(TickerModel ticker);
    Task<IEnumerable<TickerModel>> GetTickersBySymbolAsync(string symbol, int limit = 100);
    Task<IEnumerable<TickerModel>> GetTickersByExchangeNameAsync(string exchangeName, int limit = 100);
    Task<IEnumerable<TickerModel>> GetTickersBySymbolTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime);
    Task<IEnumerable<TickerModel>> GetTickersByExchangeNameTimeRangeAsync(string exchangeName, DateTime startTime, DateTime endTime);
}