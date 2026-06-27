using ChatYar.Models;
using ChatYar.Services;
using ChatYar.ViewModels;
using Microsoft.Extensions.Logging;
namespace ChatYar
{
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
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("Vazirmatn-Regular.ttf", "VazirmatnRegular");
					fonts.AddFont("Vazirmatn-Regular.ttf", "VazirmatnBold");

				});
			builder.Services.AddSingleton<LocalDbService>();
			builder.Services.AddHttpClient<IChatService, GroqChatService>();
			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<MainPage>();

			return builder.Build();
		}
	}
}
