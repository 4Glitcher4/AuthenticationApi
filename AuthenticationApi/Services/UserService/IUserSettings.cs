namespace AuthenticationApi.Services
{
    public interface IUserSettings
    {
        string Key { get; set; }
        string Iv { get; set; }
        string SecretKey { get; set; }
    }
}