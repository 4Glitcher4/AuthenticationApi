namespace AuthenticationApi.DataRepository.GenericRepository
{
    public interface IMongoSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
