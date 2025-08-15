using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyScoreBoard;
using MyScoreBoard.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// App services and JS interop
builder.Services.AddScoped<IndexedDbService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();
