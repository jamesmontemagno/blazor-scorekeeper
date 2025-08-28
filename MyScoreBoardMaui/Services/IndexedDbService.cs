using System.Collections.Generic;
using System.Threading.Tasks;
using MyScoreBoardShared.Services;
using SQLite;
using System.IO;
using MyScoreBoardShared.Models;
using MyScoreBoardMaui.Models;

namespace MyScoreBoardMaui.Services;

public class IndexedDbService : IIndexedDbService
{
    private SQLiteAsyncConnection? _db;
    private readonly string _dbPath;

    public IndexedDbService()
    {
        var folder = FileSystem.AppDataDirectory;
        _dbPath = Path.Combine(folder, "myscoreboard.db3");
    }

    public Task InitAsync()
    {
        if (_db != null) return Task.CompletedTask;

        _db = new SQLiteAsyncConnection(_dbPath);
        return _db.CreateTableAsync<DatabaseEntity>();
    }

    public async Task<int> AddAsync<T>(string storeName, T value)
    {
        await InitAsync();
        
        var entity = new DatabaseEntity
        {
            StoreName = storeName,
            Key = string.Empty // Auto-increment store, no specific key
        };
        entity.SetData(value);
        
        await _db!.InsertAsync(entity);
        return entity.ID;
    }

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        await InitAsync();
        
        var entities = await _db!.Table<DatabaseEntity>()
            .Where(e => e.StoreName == storeName)
            .ToListAsync();
        
        var results = new List<T>();
        foreach (var entity in entities)
        {
            var data = entity.GetData<T>();
            if (data != null)
            {
                // For GameStoreEntry objects, set the Key property from database ID
                if (data is GameStoreEntry gameEntry)
                {
                    gameEntry.Key = entity.ID;
                }
                results.Add(data);
            }
        }
        
        return results;
    }

    public async Task DeleteAsync(string storeName, int key)
    {
        await InitAsync();
        
        // For auto-increment stores, key is the ID
        var entity = await _db!.Table<DatabaseEntity>()
            .Where(e => e.StoreName == storeName && e.ID == key)
            .FirstOrDefaultAsync();
            
        if (entity != null)
        {
            await _db.DeleteAsync(entity);
        }
    }

    public async Task UpsertAsync<T>(string storeName, T value)
    {
        await InitAsync();
        
        // For "active" store, use fixed key "current"
        string key = storeName == "active" ? "current" : string.Empty;
        
        var existing = await _db!.Table<DatabaseEntity>()
            .Where(e => e.StoreName == storeName && e.Key == key)
            .FirstOrDefaultAsync();
        
        if (existing != null)
        {
            existing.SetData(value);
            await _db.UpdateAsync(existing);
        }
        else
        {
            var entity = new DatabaseEntity
            {
                StoreName = storeName,
                Key = key
            };
            entity.SetData(value);
            await _db.InsertAsync(entity);
        }
    }

    public async Task<T?> GetFirstAsync<T>(string storeName)
    {
        await InitAsync();
        
        // For "active" store, look for "current" key
        string key = storeName == "active" ? "current" : string.Empty;
        
        var entity = await _db!.Table<DatabaseEntity>()
            .Where(e => e.StoreName == storeName && e.Key == key)
            .FirstOrDefaultAsync();
        
        if (entity == null)
            return default(T);
            
        return entity.GetData<T>();
    }

    public async Task ClearAsync(string storeName)
    {
        await InitAsync();
        
        var entities = await _db!.Table<DatabaseEntity>()
            .Where(e => e.StoreName == storeName)
            .ToListAsync();
        
        foreach (var entity in entities)
        {
            await _db.DeleteAsync(entity);
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_db != null)
        {
            _db.GetConnection().Close();
            _db = null;
        }
        return ValueTask.CompletedTask;
    }
}
