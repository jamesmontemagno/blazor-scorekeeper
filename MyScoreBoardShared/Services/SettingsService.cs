using MyScoreBoardShared.Models;

namespace MyScoreBoardShared.Services;

public class SettingsService : ISettingsService
{
    private const string StoreGames = "favoritegames";
    private const string StorePlayers = "favoriteplayers";

    private readonly IIndexedDbService _db;

    public SettingsService(IIndexedDbService db)
    {
        _db = db;
    }

    public Task<List<FavoriteGame>> GetFavoriteGamesAsync()
        => _db.GetAllAsync<FavoriteGame>(StoreGames);

    public async Task<int> AddFavoriteGameAsync(string name)
    {
        var entry = new FavoriteGame { Name = name };
        return await _db.AddAsync(StoreGames, entry);
    }

    public Task RemoveFavoriteGameAsync(int key)
        => _db.DeleteAsync(StoreGames, key);

    public Task<List<FavoritePlayer>> GetFavoritePlayersAsync()
        => _db.GetAllAsync<FavoritePlayer>(StorePlayers);

    public async Task<int> AddFavoritePlayerAsync(string name)
    {
        var entry = new FavoritePlayer { Name = name };
        return await _db.AddAsync(StorePlayers, entry);
    }

    public Task RemoveFavoritePlayerAsync(int key)
        => _db.DeleteAsync(StorePlayers, key);
}
