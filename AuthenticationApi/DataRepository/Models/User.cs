using AuthenticationApi.DataRepository.GenericRepository;

namespace AuthenticationApi.DataRepository.Models
{
    [BsonCollection("Users")]
    public class User : Document
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
