using AuthenticationApi.DataRepository.GenericRepository;
using MongoDB.Bson;

namespace AuthenticationApi.DataRepository.Models
{
    [BsonCollection("Profiles")]
    public class Profile : Document
    {
        public ObjectId UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserIdentity { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public bool IsVerify { get; set; }
    }
}
