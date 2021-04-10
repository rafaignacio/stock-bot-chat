using System.Threading;
using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace finance_bot.Chatbot
{
    public class StockReaderService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IPushStockResult _stockResult;

        public StockReaderService(IConfiguration configuration, IPushStockResult result, IHubContext<ChatHub> context)
        {
            _configuration = configuration;
            _stockResult = result;

            _stockResult.StockPushed += (msg) =>
            {
                context.Clients.All.SendAsync("ReceiveMessage", "finance.bot", $"message pushed: {msg}");
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}