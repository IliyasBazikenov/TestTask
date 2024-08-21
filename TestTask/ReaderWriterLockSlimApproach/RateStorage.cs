using System.Collections.Concurrent;

namespace TestTask.ReaderWriterLockSlimApproach;

// This approach is not useful if we need to iterate through Dictionary, because iterating not thread-safe
public class RateStorage
{
    private Dictionary<string, Rate> rates = new();
    private readonly ReaderWriterLockSlim rwLock = new();

    public void UpdateRate(NativeRate newRate)
    {
        rwLock.EnterWriteLock();
        try
        {
            if (rates.ContainsKey(newRate.Symbol) == false)
            {
                rates.Add(newRate.Symbol, new Rate());
            }
  
            var oldRate = rates[newRate.Symbol];
            oldRate.Time = newRate.Time;
            oldRate.Symbol = newRate.Symbol;
            oldRate.Bid = newRate.Bid;
            oldRate.Ask = newRate.Ask;
        }
        finally
        {
            rwLock.ExitWriteLock();
        }

    }

    // This method is atomic because of using ReaderWriterLockSlim
    // TryGetValue faster than ContainsKey
    public Rate GetRate(string symbol)
    {
        rwLock.EnterReadLock();
        try
        {
            if (rates.TryGetValue(symbol, out var rate) == false)
                return null;

            return rate;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }
}