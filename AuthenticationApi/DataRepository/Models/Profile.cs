using AuthenticationApi.DataRepository.GenericRepository;

namespace AuthenticationApi.DataRepository.Models
{
    [BsonCollection("Profiles")]
    public class Profile : Document
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DigitalSignature { get; set; }
    }
}
