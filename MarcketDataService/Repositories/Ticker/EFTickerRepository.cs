using MarcketDataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MarcketDataService.Repositories.Ticker;

public class EfTickerRepository(IDbContextFactory<TickerContext> contextFactory) : ITickerRepository
{
    public async Task AddTicker(TickerModel ticker)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Tickers.AddAsync(ticker);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
        
    }

    public async Task<IEnumerable<TickerModel>> GetTickersBySymbolAsync(string symbol, int limit = 100)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Tickers
            .Where(t => t.Symbol == symbol)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<TickerModel>> GetTickersByExchangeNameAsync(string exchangeName, int limit = 100)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Tickers
            .Where(t => t.ExchangeName == exchangeName)
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<TickerModel>> GetTickersBySymbolTimeRangeAsync(string symbol, DateTime startTime, DateTime endTime)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Tickers
            .Where(t => t.Symbol == symbol && t.Timestamp >= startTime && t.Timestamp <= endTime)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<TickerModel>> GetTickersByExchangeNameTimeRangeAsync(string exchangeName, DateTime startTime, DateTime endTime)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        return await context.Tickers
            .Where(t => t.ExchangeName == exchangeName && t.Timestamp >= startTime && t.Timestamp <= endTime)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }
}