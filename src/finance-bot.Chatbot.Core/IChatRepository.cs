using System.Collections.Generic;
using System.Threading.Tasks;
using finance_bot.Chatbot.Core.Models;

namespace finance_bot.Chatbot.Core {
    public interface IChatRepository {
        Task<string> SaveMessage(string username, string msg);
        Task<List<ChatModel>> ListMessages();
    }
}