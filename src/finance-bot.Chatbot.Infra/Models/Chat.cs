using System;
using MongoDB.Bson.Serialization.Attributes;

namespace finance_bot.Chatbot.Infra.Models {
    public class Chat {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("msg")]
        public string Message { get; set; }
        [BsonElement("timeStamp")]
        public DateTime TimeStamp { get; set;}
    }
}