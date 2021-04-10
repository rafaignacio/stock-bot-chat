namespace finance_bot.Chatbot.Core
{
    public delegate void StockPushedHandler(string msg);

    public interface IPushStockResult
    {
        event StockPushedHandler StockPushed;
    }
}