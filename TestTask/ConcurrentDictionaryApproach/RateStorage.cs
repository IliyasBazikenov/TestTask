using System.Collections.Concurrent;

namespace TestTask.ConcurrentDictionaryApproach;

public class RateStorage
{
    private ConcurrentDictionary<string, Rate> rates = new();

    // Minus of this method is that AddOrUpdate is not atomic operation and AddValueFactory/updateValueFactory delegates can be called multiple times
    // And if AddValueFactory/updateValueFactory delegates do heavy calculations UpdateRate method can be slow 
    public void UpdateRate(NativeRate newRate)
    {
        rates.AddOrUpdate(newRate.Symbol,
            key => GetRate(newRate, key),
            (key, rate) => GetRate(newRate, key));
    }

    private static Rate GetRate(NativeRate newRate, string key)
    {
        return new Rate
        {
            Symbol = key,
            Time = newRate.Time,
            Bid = newRate.Bid,
            Ask = newRate.Ask
        };
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