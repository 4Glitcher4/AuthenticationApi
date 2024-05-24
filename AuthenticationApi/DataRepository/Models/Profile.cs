using AuthenticationApi.DataRepository.GenericRepository;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace AuthenticationApi.DataRepository.Models
{
    [BsonCollection("Profiles")]
    public class Profile : Document
    {
        [JsonIgnore]
        public ObjectId UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserIdentity { get; set; } = string.Empty;
        [JsonIgnore]
        public string Signature { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsVerify { get; set; }
    }
}
