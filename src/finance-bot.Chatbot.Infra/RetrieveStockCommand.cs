using System;
using System.Text;
using finance_bot.Chatbot.Core;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace finance_bot.Chatbot.Infra
{
    public class RetrieveStockCommand : IRetrieveStockCommand
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public RetrieveStockCommand(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("rabbitMQ");
        }

        public bool RetrieveStock(string user, string stock)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };

            Console.WriteLine($"user {user} requested stock information for {stock}");

            try
            {
                using (var conn = factory.CreateConnection())
                {
                    using (var channel = conn.CreateModel())
                    {
                        channel.QueueDeclare("stock-income", false, false, false, null);

                        var body = Encoding.UTF8.GetBytes(stock);
                        var props = channel.CreateBasicProperties();

                        props.ContentType = "text/plain";
                        props.DeliveryMode = 2;

                        channel.BasicPublish("", "stock-income", props, body);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
