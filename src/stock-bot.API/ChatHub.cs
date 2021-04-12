using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using Microsoft.AspNetCore.SignalR;

namespace stock_bot.API
{

    public class ChatHub : Hub
    {
        private IRetrieveStockCommand _stockCommand;
        public ChatHub(IRetrieveStockCommand stockCommand)
        {
            _stockCommand = stockCommand;
        }

        public async Task SendMessage(string user, string msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, msg);

            var regex = new System.Text.RegularExpressions.Regex(@"(\/stock=)([^\s]*)(\s)?");
            var m = regex.Match(msg);

            if (m?.Success == true)
            {
                var stock = m.Groups[2].ToString();

                if (!_stockCommand.RetrieveStock(user, stock))
                {
                    await Clients.All.SendAsync("ReceiveMessage", "stock.bot", $"There was a problem while trying to fetch stock {stock}");
                }
            }

        }
    }
}