using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.Json;

namespace AuthenticationApi.DataRepository.GenericRepository
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();
        IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression);
        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);
        TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);
        Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
        TDocument FindById(ObjectId id);
        Task<TDocument> FindByIdAsync(ObjectId id);
        void InsertOne(TDocument document);
        Task InsertOneAsync(TDocument document);
        void InsertMany(ICollection<TDocument> documents);
        Task InsertManyAsync(ICollection<TDocument> documents);
        void ReplaceOne(TDocument document);
        Task ReplaceOneAsync(TDocument document);
        void UpdateOne(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> updateDefinition);
        Task UpdateOneAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> updateDefinition);
        void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);
        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
        void DeleteById(ObjectId id);
        Task DeleteByIdAsync(ObjectId id);
        void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);
        Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
    }
}
