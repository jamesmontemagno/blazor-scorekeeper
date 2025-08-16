using MyScoreBoardShared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyScoreBoardShared.Services;

public interface IGameService
{
    GameSession Current { get; }

    void NewGame(string gameName);

    void AddPlayer(string name);

    void RemovePlayer(string id);

    void AddRound(string playerId, int score);

    void NextRound();

    int GetRoundScore(string playerId, int roundNumber);

    int GetTotal(string playerId);

    Dictionary<string, int> GetTotals();

    Task EndGameAsync();

    Task<List<GameStoreEntry>> GetHistoryAsync();

    Task DeleteGameAsync(int key);

    Task<bool> LoadActiveAsync();

    Task SaveActiveAsync();

    Task ClearActiveAsync();

    Task<bool> HasActiveGameAsync();
}
