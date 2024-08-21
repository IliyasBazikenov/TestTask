using System.Collections.Concurrent;

namespace TestTask.ConcurrentDictionaryApproach;

public class RateStorage
{
    private ConcurrentDictionary<string, Rate> rates = new();
    
    
    //Big minus is that AddOrUpdate is not atomic operation, and AddValueFactory/updateValueFactory delegates can call multiple times
    public void UpdateRate(NativeRate newRate)
    {
        var rate = rates.AddOrUpdate(newRate.Symbol, key => new Rate
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
        
                
        Console.WriteLine($"AddOrUpdate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}");
    }

    public Rate GetRate(string symbol)
    {
        if (rates.TryGetValue(symbol, out var rate) == false)
            return null;

        return rate;
    }
}