using Microsoft.Extensions.Logging;
using QuantumTeleportMAUI.Services;

namespace QuantumTeleportMAUI;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddSingleton<QuantumService>();
        builder.Services.AddSingleton<DataService>(provider => {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "experiments.db3");
            return new DataService(dbPath);
        });

        builder.Services.AddTransient<Views.MainPage>();
        builder.Services.AddTransient<Views.HistoryPage>();

        return builder.Build();
    }
}
