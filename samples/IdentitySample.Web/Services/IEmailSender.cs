using System.Threading.Tasks;

namespace IdentitySample.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}