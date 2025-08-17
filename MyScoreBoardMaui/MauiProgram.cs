using Microsoft.Extensions.Logging;

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

		// Register platform-specific LocalStorage implementation
		builder.Services.AddSingleton<MyScoreBoardShared.Services.ILocalStorageService, MyScoreBoardMaui.Services.LocalStorageService>();

		// Register platform-specific IndexedDb implementation (sqlite)
		builder.Services.AddSingleton<MyScoreBoardShared.Services.IIndexedDbService, MyScoreBoardMaui.Services.IndexedDbService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
