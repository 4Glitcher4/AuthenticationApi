namespace AuthenticationApi.Services
{
    public class UserSettings : IUserSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Iv { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
