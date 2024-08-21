using System.Collections.Concurrent;

namespace TestTask.LazyConcurrentDictionaryApproach;

public class RateStorage
{
    private ConcurrentDictionary<string, Lazy<Rate>> rates = new();
    private static int _runCount = 0;

    // Lazy is used for making a trick to reduce multiple calling of AddValueFactory/updateValueFactory delegates
    public void UpdateRate(NativeRate newRate)
    {
        rates.AddOrUpdate(newRate.Symbol,
            key => new Lazy<Rate>(ValueFactoryDelegate(newRate, key)),
            (key, rate) => new Lazy<Rate>(ValueFactoryDelegate(newRate, key)));
    }

    private static Func<Rate> ValueFactoryDelegate(NativeRate newRate, string key)
    {
        return () => new Rate
        {
            Symbol = key,
            Time = newRate.Time,
            Bid = newRate.Bid,
            Ask = newRate.Ask
        };
    }

    public Rate GetRate(string symbol)
    {
        if (rates.TryGetValue(symbol, out var rate) == false)
            return null;

        return rate.Value;
    }
}