namespace AuthenticationApi.Services
{
    public class SmtpSettings : ISmtpSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string AuthUser { get; set; } = string.Empty;
        public string AuthPassword { get; set; } = string.Empty;
    }
}
