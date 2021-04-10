
namespace finance_bot.Chatbot.Core
{
    public interface IRetrieveStockCommand
    {
        bool RetrieveStock(string user, string stock);
    }
}
