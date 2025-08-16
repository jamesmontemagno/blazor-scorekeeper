using MyScoreBoardShared.Models;
using MyScoreBoardShared.Services; 

namespace MyScoreBoardShared.Services;

public class GameService : IGameService
{
    private readonly IIndexedDbService _db;
    private readonly ILocalStorageService _localStorage;

    public GameService(IIndexedDbService db, ILocalStorageService localStorage)
    {
        _db = db;
        _localStorage = localStorage;
    }

    public GameSession Current { get; private set; } = new();

    public void NewGame(string gameName)
    {
        Current = new GameSession
        {
            GameName = gameName,
            StartedUtc = DateTime.UtcNow,
        };
    }

    public void AddPlayer(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var id = Guid.NewGuid().ToString();
        if (Current.Players.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) return;
        Current.Players.Add(new Player(id, name.Trim()));
    }

    public void RemovePlayer(string id)
    {
        Current.Players.RemoveAll(p => p.Id == id);
        var roundsToUpdate = Current.Rounds.ToList();
        foreach (var round in roundsToUpdate)
        {
            round.Scores.RemoveAll(s => s.PlayerId == id);
        }
    }

    public void AddRound(string playerId, int score)
    {
        if (!Current.Players.Any(p => p.Id == playerId)) return;
        
        var currentRound = Current.GetCurrentRoundData();
        var existingScore = currentRound.Scores.FirstOrDefault(s => s.PlayerId == playerId);
        
        if (existingScore != null)
        {
            // Update existing score
            currentRound.Scores.Remove(existingScore);
        }
        
        currentRound.Scores.Add(new RoundScore(playerId, score));
    }

    public void NextRound()
    {
        var currentRound = Current.GetCurrentRoundData();
        currentRound.IsComplete = true;
        Current.CurrentRound++;
    }

    public int GetRoundScore(string playerId, int roundNumber)
    {
        var round = Current.Rounds.FirstOrDefault(r => r.Number == roundNumber);
        return round?.Scores.FirstOrDefault(s => s.PlayerId == playerId)?.Score ?? 0;
    }

    public int GetTotal(string playerId) => Current.Totals.TryGetValue(playerId, out var total) ? total : 0;

    public Dictionary<string, int> GetTotals() => Current.Totals;

    public async Task EndGameAsync()
    {
        Current.EndedUtc = DateTime.UtcNow;
        // Save to IndexedDB
        var entry = new GameStoreEntry
        {
            SessionId = Current.Id,
            GameName = Current.GameName,
            StartedUtc = Current.StartedUtc,
            EndedUtc = Current.EndedUtc,
            Players = Current.Players.ToList(),
            Rounds = Current.Rounds.ToList(),
            CurrentRound = Current.CurrentRound
        };
        await _db.InitAsync();
        await _db.AddAsync("games", entry);
        await ClearActiveAsync();
    }

    public async Task<List<GameStoreEntry>> GetHistoryAsync()
    {
        await _db.InitAsync();
        var all = await _db.GetAllAsync<GameStoreEntry>("games");
        return all.OrderByDescending(g => g.StartedUtc).ToList();
    }

    public async Task DeleteGameAsync(int key)
    {
        await _db.InitAsync();
        await _db.DeleteAsync("games", key);
    }

    // Active game persistence
    public async Task<bool> LoadActiveAsync()
    {
        await _db.InitAsync();
        var active = await _db.GetFirstAsync<GameSession>("active");
        if (active is not null && string.IsNullOrWhiteSpace(active.GameName) == false)
        {
            Current = active;
            await _localStorage.SetHasActiveGameAsync(true);
            return true;
        }
        await _localStorage.SetHasActiveGameAsync(false);
        return false;
    }

    public async Task SaveActiveAsync()
    {
        await _db.InitAsync();
        await _db.UpsertAsync("active", Current);
        await _localStorage.SetHasActiveGameAsync(true);
    }

    public async Task ClearActiveAsync()
    {
        await _db.InitAsync();
        await _db.ClearAsync("active");
        await _localStorage.SetHasActiveGameAsync(false);
    }

    public async Task<bool> HasActiveGameAsync()
    {
        // First check localStorage for quick response
        var hasActiveInStorage = await _localStorage.GetHasActiveGameAsync();
        if (!hasActiveInStorage)
        {
            return false;
        }
        
        // Double-check with IndexedDB if localStorage says we have an active game
        await _db.InitAsync();
        var active = await _db.GetFirstAsync<GameSession>("active");
        var hasActive = active is not null && !string.IsNullOrWhiteSpace(active.GameName);
        
        // Update localStorage if it's out of sync
        if (!hasActive)
        {
            await _localStorage.SetHasActiveGameAsync(false);
        }
        
        return hasActive;
    }
}
