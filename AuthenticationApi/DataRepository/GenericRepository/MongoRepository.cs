using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace AuthenticationApi.DataRepository.GenericRepository
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly IMongoSettings _settings;
        public MongoRepository(IMongoSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

            _settings = settings;
        }
        private protected string GetCollectionName(Type documentType)
        {
            try
            {
                return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute), true)
                .FirstOrDefault())?.CollectionName;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual IQueryable<TDocument> AsQueryable()
        {
            try
            {
                return _collection.AsQueryable();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<TReport> Aggregate<TReport>(BsonDocument[] pipeline)
        {
            try
            {
                return _collection.Aggregate<TReport>(pipeline).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual IEnumerable<TDocument> FilterBy(
            Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                return _collection.Find(filterExpression).ToEnumerable();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            try
            {
                return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                return _collection.Find(filterExpression).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual TDocument FindById(ObjectId id)
        {
            try
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                return _collection.Find(filter).SingleOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual Task<TDocument> FindByIdAsync(ObjectId id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }
        public virtual void InsertOne(TDocument document)
        {
            try
            {
                //var isValid = false;
                //foreach (var item in document.GetType().GetProperties())
                //{
                //    if (!Valid(document, item.Name, item.GetValue(document)))
                //        throw new Exception(item.Name);
                //}
                //if (isValid)
                _collection.InsertOne(document);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual Task InsertOneAsync(TDocument document)
        {
            try
            {
                return Task.Run(() => _collection.InsertOneAsync(document));
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void InsertMany(ICollection<TDocument> documents)
        {
            try
            {
                _collection.InsertMany(documents);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
        {
            try
            {
                await _collection.InsertManyAsync(documents);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void ReplaceOne(TDocument document)
        {
            try
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
                _collection.FindOneAndReplace(filter, document);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual async Task ReplaceOneAsync(TDocument document)
        {
            try
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
                await _collection.FindOneAndReplaceAsync(filter, document);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UpdateOne(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> updateDefinition)
        {
            try
            {
                _collection.UpdateOne(filterExpression, updateDefinition);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task UpdateOneAsync(Expression<Func<TDocument, bool>> filterExpression, UpdateDefinition<TDocument> updateDefinition)
        {
            try
            {
                return Task.Run(() => _collection.UpdateOneAsync(filterExpression, updateDefinition));
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                _collection.FindOneAndDelete(filterExpression);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteById(ObjectId id)
        {
            try
            {
                var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                _collection.FindOneAndDelete(filter);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Task DeleteByIdAsync(ObjectId id)
        {
            try
            {
                return Task.Run(() =>
                {
                    var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
                    _collection.FindOneAndDeleteAsync(filter);
                });

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                _collection.DeleteMany(filterExpression);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            try
            {
                return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
