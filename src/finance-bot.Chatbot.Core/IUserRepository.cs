using System.Threading;
using System.Threading.Tasks;

namespace finance_bot.Chatbot.Core {
    public interface IUserRepository {
        Task<string> Login(string username, string password, CancellationToken cancellationToken);
        Task<string> Register(string username, string password, CancellationToken cancellationToken);
    }
}