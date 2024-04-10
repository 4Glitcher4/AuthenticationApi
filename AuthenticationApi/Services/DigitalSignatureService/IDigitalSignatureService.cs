namespace AuthenticationApi.Services
{
    public interface IDigitalSignatureService
    {
        Task GenerateDigitalSignature();
        Task<bool> ValidateDigitalSignature();
    }
}