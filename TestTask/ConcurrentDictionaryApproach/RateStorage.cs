using System.Collections.Concurrent;

namespace TestTask;

public class RateStorage
{
    private ConcurrentDictionary<string, Rate> rates = new();

    public void UpdateRate(NativeRate newRate)
    {
        rates.AddOrUpdate(newRate.Symbol, key => new Rate
        {
            Symbol = key,
            Time = newRate.Time,
            Bid = newRate.Bid,
            Ask = newRate.Ask
        }, (key, rate) => new Rate
        {
            Symbol = key,
            Time = newRate.Time,
            Bid = newRate.Bid,
            Ask = newRate.Ask,
        });
        
        Console.WriteLine($"UpdateRate: Symbol - {newRate?.Symbol}, BID - {newRate?.Bid}, ASK - {newRate?.Ask}");
    }

    public Rate GetRate(string symbol)
    {
        if (rates.TryGetValue(symbol, out var rate) == false)
            return null;

        return rate;
    }
}