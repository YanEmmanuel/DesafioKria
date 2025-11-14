using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioKria.Domain.Entities;
using DesafioKria.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DesafioKria.Infrastructure.Repositories;

public class TollTransactionRepository : ITollTransactionRepository
{
    private readonly IMongoCollection<TollTransaction> _collection;

    public TollTransactionRepository(IConfiguration configuration)
    {
        var databaseName = "candidato";
        var collectionName = "TabTransacoes";

        var client = new MongoClient("mongodb+srv://nelsonhilariaokria:uonQ8xiBByKLtKq7@kria.zzp95.mongodb.net/?retryWrites=true&w=majority&appName=Kria");
        var db = client.GetDatabase(databaseName);
        _collection = db.GetCollection<TollTransaction>(collectionName);
    }

    public async Task<List<TollTransaction>> GetTransactionsAsync(int limit)
    {
        var normalizedLimit = Math.Max(1, limit);
        return await _collection
            .Find(_ => true)
            .Limit(normalizedLimit)
            .ToListAsync();
    }
}