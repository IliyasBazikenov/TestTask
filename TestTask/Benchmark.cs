using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace TestTask;

[MemoryDiagnoser]
[Orderer]
public class Benchmark
{
    [Params(1000, 100000, 1000000)] 
    public int ProcessingSteps { get; set; }

    [Params(100, 10000, 100000)] 
    public int RateStoreDictionarySize { get; set; }

    private static readonly ConcurrentDictionaryApproach.RateStorage RateStorage = new();
    private static readonly LazyConcurrentDictionaryApproach.RateStorage RateStorageLazy = new();
    private static readonly ReaderWriterLockSlimApproach.RateStorage RateStorageRwLock = new();

    [Benchmark]
    public void ParallelForEach()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorage.UpdateRate(GetNativeRate(symbol));
                var rate = RateStorage.GetRate(symbol);

#if DEBUG
                Console.WriteLine($"GetRate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}");
#endif
            });

        
    }

    [Benchmark]
    public void ParallelForEachLazy()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorageLazy.UpdateRate(GetNativeRate(symbol));
                var rate = RateStorageLazy.GetRate(symbol);

#if DEBUG
                Console.WriteLine($"GetRate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}");
#endif
            });
        
        PrintRates();
    }
    
    private void PrintRates()
    {
        Console.WriteLine();
        Console.WriteLine("Results");
        for (int i = 0; i < RateStoreDictionarySize; i++)
        {
            var rate = RateStorageLazy.GetRate(i.ToString());
#if DEBUG
            Console.WriteLine($"GetRate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}");
#endif
        }
    }

    [Benchmark]
    public void ParallelForEachRwLock()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorageRwLock.UpdateRate(GetNativeRate(symbol));
                var rate = RateStorageRwLock.GetRate(symbol);

#if DEBUG
                Console.WriteLine($"GetRate - {rate?.Symbol}, BID - {rate?.Bid}, ASK - {rate?.Ask}");
#endif
            });
    }

    private NativeRate GetNativeRate(string symbol)
    {
        var bid = Random.Shared.Next(1, 1000);

        return new NativeRate()
        {
            Symbol = symbol,
            Bid = bid,
            Ask = bid + 1,
            Time = DateTime.Now
        };
    }
}