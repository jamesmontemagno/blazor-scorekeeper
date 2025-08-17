using System.Text.Json;
using Microsoft.JSInterop;

namespace MyScoreBoard.Services;

public class IndexedDbService : MyScoreBoardShared.Services.IIndexedDbService
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public IndexedDbService(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/indexedDb.js").AsTask());
    }

    public async Task InitAsync()
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("initDb");
    }

    public async Task<int> AddAsync<T>(string storeName, T value)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<int>("addItem", storeName, value);
    }

    public async Task<List<T>> GetAllAsync<T>(string storeName)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<List<T>>("getAll", storeName);
    }

    public async Task DeleteAsync(string storeName, int key)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("deleteItem", storeName, key);
    }

    public async Task UpsertAsync<T>(string storeName, T value)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("putItem", storeName, value);
    }

    public async Task<T?> GetFirstAsync<T>(string storeName)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<T?>("getFirst", storeName);
    }

    public async Task ClearAsync(string storeName)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("clearStore", storeName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
