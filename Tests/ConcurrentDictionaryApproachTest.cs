using Xunit;

namespace TestTask.UnitTests;

public class ConcurrentDictionaryApproachTest
{
    [Fact]
    public void CurrentRateIsConsistent()
    {
        ConcurrentDictionaryApproach.RateStorage rateStorage = new();

        Rate rate = new Rate()
        {
            Ask = 0,
            Bid = 330
        };
        
        Parallel.Invoke(() => rateStorage.UpdateRate(new NativeRate
            {
                Symbol = "1",
                Ask = 20,
                Bid = 25
            }),
            () => rate = rateStorage.GetRate("1"),
            () => rateStorage.UpdateRate(new NativeRate
            {
                Symbol = "1",
                Ask = 30,
                Bid = 35
            }));
        
        Assert.True(rate.Ask < rate.Bid);
    }
}