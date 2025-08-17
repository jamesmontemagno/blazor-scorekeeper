using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyScoreBoard;
using MyScoreBoard.Services;
using MyScoreBoardShared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// App services and JS interop
// Register JS interop services and expose interfaces from shared project
builder.Services.AddScoped<IIndexedDbService, IndexedDbService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

await builder.Build().RunAsync();
