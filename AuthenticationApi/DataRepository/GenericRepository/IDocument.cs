using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.DataRepository.GenericRepository
{
    public interface IDocument
    {
        [Key]
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        ObjectId Id { get; set; }
    }
}
