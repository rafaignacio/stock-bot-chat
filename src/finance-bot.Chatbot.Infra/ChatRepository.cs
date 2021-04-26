using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using finance_bot.Chatbot.Core.Models;
using finance_bot.Chatbot.Infra.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace finance_bot.Chatbot.Infra {
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoCollection<Chat> _chat;

        public ChatRepository(IConfiguration configuration) {
            var connStr = configuration.GetConnectionString("mongoDB");

            var client = new MongoClient(connStr);
            var db = client.GetDatabase("chatDB");

            _chat = db.GetCollection<Chat>("chat");
        }

        public async Task<List<ChatModel>> ListMessages()
        {
            var count = await _chat.CountDocumentsAsync(c => true, null);

            if( count - 50 < 0) {
                count = 0;
            } else {
                count = count - 50;
            }

            var list = await _chat.Find(c => true, null).SortBy(c => c.TimeStamp).Skip( ((int) count) ).ToListAsync();
            var output = new List<ChatModel>();

            list.ForEach( c => {
                output.Add(new ChatModel{
                    Username = c.Username,
                    Id = c.Id,
                    Message = c.Message,
                    TimeSpan = c.TimeStamp
                });
            });

            return output;
        }

        public async Task<string> SaveMessage(string username, string msg)
        {
            var chat = new Chat{
                Username = username,
                Message = msg,
                TimeStamp = DateTime.Now,
            };

            await _chat.InsertOneAsync( chat, 
                options: new InsertOneOptions{ BypassDocumentValidation = false} );

            return chat.Id;
        }
    }
}