namespace AuthenticationApi.DataRepository.GenericRepository
{
    public class MongoSettings : IMongoSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
