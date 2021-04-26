using System;

namespace finance_bot.Chatbot.Core.Models {
    public class ChatModel {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime TimeSpan { get; set; }
    }
}