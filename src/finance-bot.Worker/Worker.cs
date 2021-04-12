using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using finance_bot.Worker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace finance_bot.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly IStockService _stockService;

        public Worker(ILogger<Worker> logger, IConfiguration config, IStockService stockService)
        {
            _logger = logger;
            _config = config;
            _stockService = stockService;
        }

        protected List<string> ParseCSV(string csv)
        {
            var lines = csv.Split('\n');

            if (lines?.Length >= 2)
            {
                var line = lines[1];

                return line.Split(',').ToList();
            }

            return null;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_config.GetConnectionString("rabbitMQ")) };
            factory.DispatchConsumersAsync = true;

            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();


            var consumer = new AsyncEventingBasicConsumer(channel);

            channel.QueueDeclare("stock-income", false, false, false, null);
            channel.QueueDeclare("stock-results", false, false, false, null);

            consumer.Received += async (ch, msg) =>
            {
                var stock_name = Encoding.UTF8.GetString(msg.Body.ToArray());
                _logger.LogInformation("received stock name {stock}", stock_name);

                var stock = await _stockService.GetStock(stock_name);

                _logger.LogInformation($"stock retrivied: {stock}");
                var props = channel.CreateBasicProperties();

                props.ContentType = "text/plain";
                props.DeliveryMode = 2;

                if (!string.IsNullOrWhiteSpace(stock))
                {
                    var stockLine = ParseCSV(stock);
                    if (stockLine?.Count > 0)
                    {
                        decimal share = 0;
                        var stockMsg = $"Sorry, there is no information available for stock {stock_name}";

                        if (decimal.TryParse(stockLine[3], out share))
                        {
                            stockMsg = $"{stockLine[0]} price per share is {stockLine[3]}";
                        }

                        channel.BasicPublish("", "stock-results", props, Encoding.UTF8.GetBytes(stockMsg));
                    }
                }
                else
                {
                    var stockMsg = Encoding.UTF8.GetBytes("Sorry, could not fetch the stock info.");
                    channel.BasicPublish("", "stock-results", props, stockMsg);
                }

                _logger.LogInformation("message received: {msg}", stock_name);
            };

            channel.BasicConsume("stock-income", true, consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
