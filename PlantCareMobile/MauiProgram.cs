using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace PlantCareMobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Roboto-VariableFont_wdth_wght.ttf", "RobotoVar");

                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                // Esto quita la línea inferior en Android
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif WINDOWS
                // Esto quita el borde en Windows
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
            });
            return builder.Build();
        }
    }
}
