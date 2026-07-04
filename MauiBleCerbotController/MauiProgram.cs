using Bluetooth.Maui;
using MauiBleCerbotController.Converters;
using MauiBleCerbotController.Interfaces;
using MauiBleCerbotController.Services;
using Microsoft.Extensions.Logging;

namespace MauiBleCerbotController
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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Register Bluetooth services
            builder.Services.AddBluetoothServices();
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ViewModels.MainPageViewModel>();
            builder.Services.AddSingleton<BoolToColorConverter>();
            builder.Services.AddSingleton<InverseBoolConverter>();
            builder.Services.AddBluetoothServices();
            //builder.Services.AddSingleton<ViewModels.ScannerViewModel>();

            return builder.Build();
        }
    }
}
