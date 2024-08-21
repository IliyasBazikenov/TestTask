using System.Collections.Concurrent;

namespace TestTask.ConcurrentDictionaryApproach;

public class RateStorage
{
    private ConcurrentDictionary<string, Rate> rates = new();
    private static int _runCount = 0;

    // Minus of that method is that AddOrUpdate is not atomic operation, and AddValueFactory/updateValueFactory delegates can call multiple times
    // And if AddValueFactory/updateValueFactory do heavy calculations UpdateRate method can be slow 
    public void UpdateRate(NativeRate newRate)
    {
        var rate = rates.AddOrUpdate(newRate.Symbol, key =>
        {
            Interlocked.Increment(ref _runCount);
            return new Rate
            {
                Symbol = key,
                Time = newRate.Time,
                Bid = newRate.Bid,
                Ask = newRate.Ask
            };
        }, (key, rate) =>
        {
            Interlocked.Increment(ref _runCount);
            return new Rate
            {
                Symbol = key,
                Time = newRate.Time,
                Bid = newRate.Bid,
                Ask = newRate.Ask,
            };
        });

#if DEBUG
        Console.WriteLine(
            $"AddOrUpdate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}, _runCount - {_runCount}");
#endif
    }

    // This method is atomic cause updating of rates is using immutable approach, i.e. replace current Rate with modified
    // TryGetValue faster than ContainsKey
    public Rate GetRate(string symbol)
    {
        if (rates.TryGetValue(symbol, out var rate) == false)
            return null;

        return rate;
    }
}