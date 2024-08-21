namespace TestTask;

public class Rate
{
    public DateTime Time { get; set; }
    public string Symbol { get; set; } // название инструмента, например EURUSD
    public double Bid { get; set; } // это цена для покупок 
    public double Ask { get; set; } // это цена для продаж
}