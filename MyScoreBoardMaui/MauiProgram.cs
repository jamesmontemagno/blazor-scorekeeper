using Microsoft.Extensions.Logging;
using MyScoreBoardMaui.Services;
using MyScoreBoardShared.Services;

namespace MyScoreBoardMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		builder.Services.AddSingleton<IGameService, GameService>();

		// Register platform-specific LocalStorage implementation
		builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();

		// Register platform-specific IndexedDb implementation (sqlite)
		builder.Services.AddSingleton<IIndexedDbService, IndexedDbService>();
		
		// Register platform detection service
		builder.Services.AddSingleton<IPlatformService, PlatformService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
