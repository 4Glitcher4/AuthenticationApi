namespace AuthenticationApi.Services
{
    public interface ISmtpService
    {
        Task SendRegister();
        Task SendResetPassword();
    }
}