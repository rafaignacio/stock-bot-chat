using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using finance_bot.Chatbot.Core;
using finance_bot.Chatbot.Infra.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace finance_bot.Chatbot.Infra {
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IConfiguration configuration) {
            var connStr = configuration.GetConnectionString("mongoDB");

            var client = new MongoClient(connStr);
            var db = client.GetDatabase("userDB");

            _users = db.GetCollection<User>("users");
        }

        private string HashPassword(string password) {
            var hash = System.Security.Cryptography.SHA256.Create();
            using(var ms = new MemoryStream( Encoding.UTF8.GetBytes(password) )) {
                var ch = hash.ComputeHash(ms);

                return Encoding.UTF8.GetString(ch);
            }
        }

        private async Task<User> FindUserByUsername(string username, CancellationToken token) {
            var userRepo = await _users.FindAsync( u => u.Username.Equals(username.ToLower()), null, token);
            return userRepo.FirstOrDefault();
        }

        public async Task<string> Login(string username, string password, CancellationToken token = default(CancellationToken))
        {
            var pwd = HashPassword(password);
            var user = await this.FindUserByUsername(username, token); 

            if( user != null && user.Password.Equals(pwd) ) {
                return user.Id;
            }

            return null;
        }

        public async Task<string> Register(string username, string password, CancellationToken token = default(CancellationToken))
        {
            var pwd = HashPassword(password);
            var user = await this.FindUserByUsername(username, token);

            if( user != null ) return null;

            var newUser = new User{ 
                Username = username,
                Password = pwd
            };

            var options = new InsertOneOptions() {
                BypassDocumentValidation = false,
            };

            await _users.InsertOneAsync( newUser, options, token );

            return newUser.Id;
        }
    }
}