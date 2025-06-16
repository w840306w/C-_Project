using Microsoft.Extensions.Logging;
using MauiApp1.Services; 
using MauiApp1.Views;   
using CommunityToolkit.Maui;
using MauiApp1.Resources.DB;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<DatabaseService>();

        builder.Services.AddTransient<IndexPage>();
        builder.Services.AddTransient<VocabularyPage>();
        builder.Services.AddTransient<QuizSelectionPage>();
        builder.Services.AddTransient<QuizPage>();
        builder.Services.AddTransient<MistakeReviewPage>();
        builder.Services.AddTransient<Settings>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}