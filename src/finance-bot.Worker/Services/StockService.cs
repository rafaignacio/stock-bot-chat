using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace finance_bot.Worker.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public StockService(ILogger<StockService> logger, HttpClient client)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> GetStock(string stock_name)
        {
            _logger.LogInformation("initiating request");

            var timeOut = Policy.TimeoutAsync(3);
            var retry = Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .RetryAsync(3);


            return await retry.ExecuteAsync<string>(async (ct) =>
                await timeOut.ExecuteAsync<string>(async (cancellation) =>
                {
                    _logger.LogInformation("retrieving stock");
                    var request = new HttpRequestMessage(HttpMethod.Get,
                        $"https://stooq.com/q/l/?s={stock_name}&f=sd2t2ohlcv&h&e=csv");
                    var response = await _client.SendAsync(request, cancellation);

                    if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation("stock retrieved successfully");
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        _logger.LogInformation("could not retrieve stock");
                        return null;
                    }
                }, ct), CancellationToken.None
            );

        }
    }
}