namespace AuthenticationApi.ModelsDto
{
    public class SendDto
    {
        public string Login { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public SendType SendType { get; set; }
    }
    public enum SendType
    {
        VerifyUser,
        ResetPassword
    }
}
