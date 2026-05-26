using DotNetEnv;
using MarcketDataService.Models;
using MarcketDataService.Repositories.Ticker;
using Microsoft.Extensions.Caching.Memory;

namespace MarcketDataService.Services.DataCollector.TickerProcessor;

//TODO Change to use Redis or other distributed cache to avoid duplicates across multiple instances of the service 
public class TickerProcessor() : ITickerProcessor
{
    private long _counter = 0;
    private readonly int _cacheTtlPerMinutes;
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly ITickerRepository _repository;
    private readonly ILogger<TickerProcessor> _logger;

    public TickerProcessor(ITickerRepository repository, ILogger<TickerProcessor> logger) : this()
    {
        _repository = repository;
        _logger = logger;
        
        Env.Load();
        _cacheTtlPerMinutes = int.Parse(Environment.GetEnvironmentVariable("CACHE_TTL_PER_MINUTES") ?? "1");
    }

    public async Task ProcessAsync(TickerModel ticker)
    {
        if (IsDuplicate(ticker))
            return;

        await _repository.AddTicker(ticker);

        var count = Interlocked.Increment(ref _counter);

        if (count % 100 == 0)
            _logger.LogInformation($"Processed {count} ticks");
    }

    private bool IsDuplicate(TickerModel ticker)
    {
        var hashCode = ticker.GetHashCode();

        if (_cache.TryGetValue(hashCode, out _))
        {
            return true;
        }

        _cache.Set(hashCode, true, TimeSpan.FromMinutes(_cacheTtlPerMinutes));
        return false;
    }
}