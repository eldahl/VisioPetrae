using System.Diagnostics;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace backend.Persistence;

public interface IMongoDBContext
{
    IMongoCollection<T> Collection<T>() where T : class;
}

public class MongoDBContext : IMongoDBContext
{
    private readonly IMongoDatabase _database;

    public MongoDBContext(IConfiguration _configuration)
    {
        var connectionString = _configuration["MongoDB:ConnectionString"] ?? throw new Exception("MongoDB connection string is not set");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(_configuration["MongoDB:DatabaseName"]);
    }

    public IMongoCollection<T> Collection<T>() where T : class
    {
        return _database.GetCollection<T>(typeof(T).Name.ToLower());
    }
}