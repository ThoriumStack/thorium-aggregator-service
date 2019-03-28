using System.Threading.Tasks;

namespace Thorium.Aggregator.Services
{
    public interface IEmailSender
    {
        Task SendEmail(string email, string subject, string message, string toUsername);
    }
}
