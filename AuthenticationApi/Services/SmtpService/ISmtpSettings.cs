namespace AuthenticationApi.Services
{
    public interface ISmtpSettings
    {
        public string ConnectionString { get; set; }
        public string AuthUser { get; set; }
        public string AuthPassword { get; set; }
    }
}