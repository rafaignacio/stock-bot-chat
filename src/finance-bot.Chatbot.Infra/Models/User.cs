using MongoDB.Bson.Serialization.Attributes;

namespace finance_bot.Chatbot.Infra.Models {
    public class User {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }
    }
}