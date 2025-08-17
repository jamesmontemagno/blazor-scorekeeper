using SQLite;
using System.Text.Json;

namespace MyScoreBoardMaui.Models;

public class DatabaseEntity
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    
    public string StoreName { get; set; } = string.Empty;
    
    public string Key { get; set; } = string.Empty;
    
    public string DataJson { get; set; } = string.Empty;
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    public T? GetData<T>()
    {
        if (string.IsNullOrEmpty(DataJson))
            return default;
        
        try
        {
            return JsonSerializer.Deserialize<T>(DataJson);
        }
        catch
        {
            return default;
        }
    }

    public void SetData<T>(T data)
    {
        DataJson = JsonSerializer.Serialize(data);
        UpdatedUtc = DateTime.UtcNow;
    }
}
