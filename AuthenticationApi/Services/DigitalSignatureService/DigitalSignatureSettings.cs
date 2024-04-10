namespace AuthenticationApi.Services
{
    public class DigitalSignatureSettings : IDigitalSignatureSettings
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
