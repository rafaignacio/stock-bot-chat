using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace stock_bot.API
{

    public class ChatHub : Hub
    {
        private readonly IPushStockResult _stockPusher;
        private readonly ILogger<ChatHub> _logger;
        private IRetrieveStockCommand _stockCommand;
        private readonly IChatRepository _chatRepo;

        public ChatHub(IRetrieveStockCommand stockCommand, IChatRepository chatRepository, ILogger<ChatHub> logger, IPushStockResult stockPusher)
        {
            _stockPusher = stockPusher;
            _logger = logger;
            _stockCommand = stockCommand;
            _chatRepo = chatRepository;

            _stockPusher.StockPushed += (msg) => {
                Task.Run( async () => await SendMessage("stock-bot", msg) );
            };
        }

        public async Task<string> SaveMessage(string user, string msg) {
            return await _chatRepo.SaveMessage(user, msg);
        }

        public async Task ListMessages() {
            var msgs = await _chatRepo.ListMessages();
            await Clients.All.SendAsync("ReceiveMessage", msgs);
        }

        public async Task SendMessage(string user, string msg)
        {
            var msgId = await this.SaveMessage(user, msg);

            if(string.IsNullOrEmpty(msgId)) {
                await Clients.Caller.SendAsync("ReceiveError", "your message was not delivered.");
                return;
            }

            await ListMessages();

            var regex = new System.Text.RegularExpressions.Regex(@"(\/stock=)([^\s]*)(\s)?");
            var m = regex.Match(msg);

            if (m?.Success == true)
            {
                var stock = m.Groups[2].ToString();

                _logger.LogInformation($"user asked for stock {stock}");
                if (!_stockCommand.RetrieveStock(user, stock))
                {
                    _logger.LogInformation("error fetching stock");
                    await SendMessage("stock.bot", $"There was a problem while trying to fetch stock {stock}");
                }
            }

        }
    }
}