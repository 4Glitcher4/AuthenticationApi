using MongoDB.Bson;

namespace AuthenticationApi.DataRepository.GenericRepository
{
    public class Document : IDocument
    {
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    }
}
