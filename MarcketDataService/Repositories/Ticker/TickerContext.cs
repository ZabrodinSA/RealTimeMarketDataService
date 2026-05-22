using DotNetEnv;
using MarcketDataService.Models;
using Microsoft.EntityFrameworkCore;

namespace MarcketDataService.Repositories.Ticker;

public sealed class TickerContext : DbContext
{
    public DbSet<TickerModel> Tickers { get; set; } = null!;

    public TickerContext(DbContextOptions<TickerContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Env.Load();

        var symbolMaxLength = int.Parse(Environment.GetEnvironmentVariable("SYMBOL_MAX_LENGTH") ?? "50");
        var exchangeNameMaxLength = int.Parse(Environment.GetEnvironmentVariable("EXCHANGE_NAME_MAX_LENGTH") ?? "100");
        var pricePrecision = int.Parse(Environment.GetEnvironmentVariable("PRICE_PRECISION") ?? "18");
        var priceScale = int.Parse(Environment.GetEnvironmentVariable("PRICE_SCALE") ?? "8");
        var volumePrecision = int.Parse(Environment.GetEnvironmentVariable("VOLUME_PRECISION") ?? "18");
        var volumeScale = int.Parse(Environment.GetEnvironmentVariable("VOLUME_SCALE") ?? "8");

        modelBuilder.Entity<TickerModel>(entity =>
        {
            entity.HasKey(t => new { t.Symbol, t.ExchangeName, t.Timestamp });

            entity.Property(t => t.Symbol)
                .IsRequired()
                .HasMaxLength(symbolMaxLength);

            entity.Property(t => t.ExchangeName)
                .IsRequired()
                .HasMaxLength(exchangeNameMaxLength);

            entity.Property(t => t.Price)
                .IsRequired()
                .HasColumnType($"decimal({pricePrecision}, {priceScale})");

            entity.Property(t => t.Volume)
                .IsRequired()
                .HasColumnType($"decimal({volumePrecision}, {volumeScale})");

            entity.HasIndex(t => t.Symbol).HasDatabaseName("IX_Ticker_Symbol");
            entity.HasIndex(t => t.ExchangeName).HasDatabaseName("IX_Ticker_ExchangeName");
        });
    }
}