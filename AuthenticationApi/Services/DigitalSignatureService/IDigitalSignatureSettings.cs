namespace AuthenticationApi.Services
{
    public interface IDigitalSignatureSettings
    {
        string PublicKey { get; set; }
        string PrivateKey { get; set; }
    }
}