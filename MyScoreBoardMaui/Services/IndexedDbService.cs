using System.Collections.Generic;
using System.Threading.Tasks;
using MyScoreBoardShared.Services;
using SQLite;
using System.IO;
using MyScoreBoardShared.Models;

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
        // Ensure tables exist. We'll create a simple table for GameStoreEntry JSON storage.
        return _db.CreateTableAsync<GameStoreEntry>();
    }

    public async Task<int> AddAsync<T>(string storeName, T value)
    {
        await InitAsync();
        if (typeof(T) == typeof(GameStoreEntry) && value is GameStoreEntry entry)
        {
            // SQLite-net auto-increments integer primary key if property named 'Key' and decorated, but our model has nullable Key.
            // Use InsertAsync which will set the object's Key if it's an [PrimaryKey, AutoIncrement] property.
            await _db!.InsertAsync(entry);
            return entry.Key ?? 0;
        }

        // For other types, serialize into a generic table if needed (not implemented). Return -1 to indicate unsupported.
        return -1;
    }

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        await InitAsync();
        if (typeof(T) == typeof(GameStoreEntry))
        {
            var list = await _db!.Table<GameStoreEntry>().ToListAsync();
            return list.Cast<T>().ToList();
        }

        return new List<T>();
    }

    public async Task DeleteAsync(string storeName, int key)
    {
        await InitAsync();
        var item = await _db!.FindAsync<GameStoreEntry>(key);
        if (item != null)
        {
            await _db.DeleteAsync(item);
        }
    }

    public async Task UpsertAsync<T>(string storeName, T value)
    {
        await InitAsync();
        if (typeof(T) == typeof(GameSession) && value is GameSession session)
        {
            // active store uses a single entry with a fixed key concept; we'll store GameSession in GameStoreEntry with special SessionId 'active'
            // But existing code expects UpsertAsync("active", Current) where Current is a GameSession. We'll implement a simple active table.
            // For simplicity, upsert into GameStoreEntry table using SessionId as unique key.
            var existing = await _db!.Table<GameStoreEntry>().Where(g => g.SessionId == session.Id).FirstOrDefaultAsync();
            if (existing == null)
            {
                var entry = new GameStoreEntry
                {
                    SessionId = session.Id,
                    GameName = session.GameName,
                    StartedUtc = session.StartedUtc,
                    EndedUtc = session.EndedUtc,
                    Players = session.Players,
                    Rounds = session.Rounds,
                    CurrentRound = session.CurrentRound
                };
                await _db.InsertAsync(entry);
            }
            else
            {
                existing.GameName = session.GameName;
                existing.StartedUtc = session.StartedUtc;
                existing.EndedUtc = session.EndedUtc;
                existing.Players = session.Players;
                existing.Rounds = session.Rounds;
                existing.CurrentRound = session.CurrentRound;
                await _db.UpdateAsync(existing);
            }
            return;
        }

        if (typeof(T) == typeof(GameStoreEntry) && value is GameStoreEntry entryVal)
        {
            if (entryVal.Key.HasValue && entryVal.Key.Value > 0)
            {
                await _db!.UpdateAsync(entryVal);
            }
            else
            {
                await _db!.InsertAsync(entryVal);
            }
            return;
        }

        // unsupported types: no-op
    }

    public async Task<T?> GetFirstAsync<T>(string storeName)
    {
        await InitAsync();
        if (typeof(T) == typeof(GameSession))
        {
            // Look for an entry in GameStoreEntry with SessionId 'active' or return first
            var first = await _db!.Table<GameStoreEntry>().FirstOrDefaultAsync();
            if (first == null) return default;
            var session = new GameSession
            {
                Id = first.SessionId,
                GameName = first.GameName,
                StartedUtc = first.StartedUtc,
                EndedUtc = first.EndedUtc,
                Players = first.Players,
                Rounds = first.Rounds,
                CurrentRound = first.CurrentRound
            };
            return (T?)(object)session;
        }

        if (typeof(T) == typeof(GameStoreEntry))
        {
            var first = await _db!.Table<GameStoreEntry>().FirstOrDefaultAsync();
            return (T?)(object?)first;
        }

        return default;
    }

    public async Task ClearAsync(string storeName)
    {
        await InitAsync();
        // Clear GameStoreEntry table
        await _db!.DeleteAllAsync<GameStoreEntry>();
    }

    public ValueTask DisposeAsync()
    {
        // SQLiteAsyncConnection doesn't implement IAsyncDisposable; close DB if necessary
        if (_db != null)
        {
            _db.GetConnection().Close();
            _db = null;
        }
        return ValueTask.CompletedTask;
    }
}
