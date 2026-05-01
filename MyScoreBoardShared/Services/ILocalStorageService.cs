using System.Collections.Generic;
using System.Threading.Tasks;
using MyScoreBoardShared.Models;

namespace MyScoreBoardShared.Services;

public interface ILocalStorageService
{
    Task<bool> GetHasActiveGameAsync();

    Task SetHasActiveGameAsync(bool hasActive);

    Task<string?> GetItemAsync(string key);

    Task SetItemAsync(string key, string value);

    Task RemoveItemAsync(string key);

    // Game setup preferences
    Task<string?> GetLastGameNameAsync();
    Task SetLastGameNameAsync(string gameName);
    Task<List<string>?> GetLastPlayersAsync();
    Task SetLastPlayersAsync(List<string> playerNames);

    // Favorites
    Task<List<FavoriteGame>> GetFavoriteGamesAsync();
    Task SetFavoriteGamesAsync(List<FavoriteGame> games);
    Task<List<FavoritePlayer>> GetFavoritePlayersAsync();
    Task SetFavoritePlayersAsync(List<FavoritePlayer> players);
}
