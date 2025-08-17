using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyScoreBoardShared.Services;

public interface IIndexedDbService : IAsyncDisposable
{
    Task InitAsync();

    Task<int> AddAsync<T>(string storeName, T value);

    Task<List<T>> GetAllAsync<T>(string storeName);

    Task DeleteAsync(string storeName, int key);

    Task UpsertAsync<T>(string storeName, T value);

    Task<T?> GetFirstAsync<T>(string storeName);

    Task ClearAsync(string storeName);
}
