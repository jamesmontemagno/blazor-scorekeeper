namespace MyScoreBoard.Models;

public record Player(string Id, string Name);

public record RoundEntry(string PlayerId, int Score, DateTime TimestampUtc);

public record RoundScore(string PlayerId, int Score);

public class Round
{
    public int Number { get; set; }
    public List<RoundScore> Scores { get; set; } = new();
    public bool IsComplete { get; set; }

    public Round() { }
    public Round(int number, List<RoundScore> scores, bool isComplete)
    {
        Number = number;
        Scores = scores;
        IsComplete = isComplete;
    }
}

public class GameSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GameName { get; set; } = "";
    public DateTime StartedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndedUtc { get; set; }
    public List<Player> Players { get; set; } = new();
    public List<Round> Rounds { get; set; } = new();
    public int CurrentRound { get; set; } = 1;

    public Dictionary<string, int> Totals => Rounds
        .SelectMany(r => r.Scores)
        .GroupBy(s => s.PlayerId)
        .ToDictionary(g => g.Key, g => g.Sum(x => x.Score));

    public Round GetCurrentRoundData()
    {
        var currentRound = Rounds.FirstOrDefault(r => r.Number == CurrentRound);
        if (currentRound == null)
        {
            currentRound = new Round(CurrentRound, new List<RoundScore>(), false);
            Rounds.Add(currentRound);
        }
        return currentRound;
    }

    public bool AllPlayersScored()
    {
        var currentRound = GetCurrentRoundData();
        return Players.All(p => currentRound.Scores.Any(s => s.PlayerId == p.Id));
    }
}

// Flattened storage model for IndexedDB
public class GameStoreEntry
{
    public int? Key { get; set; } // Auto-increment key set by IndexedDB
    public string SessionId { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public DateTime StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }
    public List<Player> Players { get; set; } = new();
    public List<Round> Rounds { get; set; } = new();
    public int CurrentRound { get; set; }
}
