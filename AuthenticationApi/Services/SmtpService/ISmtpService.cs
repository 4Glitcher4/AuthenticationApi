using AuthenticationApi.ModelsDto;

namespace AuthenticationApi.Services
{
    public interface ISmtpService
    {
        bool Send(string url, SendDto smtp);
    }
}