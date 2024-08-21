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
    public void ConcurrentDictionaryApproach()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorage.UpdateRate(GetNativeRate(symbol));
                RateStorage.GetRate(symbol);
            });
    }

    [Benchmark]
    public void LazyConcurrentDictionaryApproach()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorageLazy.UpdateRate(GetNativeRate(symbol));
                RateStorageLazy.GetRate(symbol);
            });
    }

    [Benchmark]
    public void ReaderWriterLockSlimApproach()
    {
        Parallel.ForEach(Enumerable.Range(0, ProcessingSteps),
            (int stepNumber) =>
            {
                var symbol = (stepNumber % RateStoreDictionarySize).ToString();

                RateStorageRwLock.UpdateRate(GetNativeRate(symbol));
                RateStorageRwLock.GetRate(symbol);
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