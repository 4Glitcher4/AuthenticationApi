namespace AuthenticationApi.Services
{
    public interface IDigitalSignatureService
    {
        Task GenerateDigitalSignature(string signature);
        Task<bool> ValidateDigitalSignature();
    }
}