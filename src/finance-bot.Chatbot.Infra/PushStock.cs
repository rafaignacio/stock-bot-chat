using System;
using System.Text;
using finance_bot.Chatbot.Core;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace finance_bot.Chatbot.Infra
{
    public class PushStock : IPushStockResult, IDisposable
    {
        public event StockPushedHandler StockPushed;

        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly EventingBasicConsumer _consumer;

        public PushStock(IConfiguration configuration)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("rabbitMQ")) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += (obj, msg) =>
            {
                if (StockPushed != null)
                {
                    var body = Encoding.UTF8.GetString(msg.Body.ToArray());

                    StockPushed(body);
                }
            };

            _channel.BasicConsume("stock-results", true, _consumer);
        }

        public void Dispose()
        {
            _connection.Close();
            _channel.Close();
        }
    }
}