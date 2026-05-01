using MyScoreBoardShared.Models;

namespace MyScoreBoardShared.Services;

public interface ISettingsService
{
    Task<List<FavoriteGame>> GetFavoriteGamesAsync();
    Task<int> AddFavoriteGameAsync(string name);
    Task RemoveFavoriteGameAsync(int key);

    Task<List<FavoritePlayer>> GetFavoritePlayersAsync();
    Task<int> AddFavoritePlayerAsync(string name);
    Task RemoveFavoritePlayerAsync(int key);
}
