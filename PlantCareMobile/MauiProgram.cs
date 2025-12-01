using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using PlantCareMobile.Services;


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
            // Ruta de la BD
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "plants.db3");

            // Registrar el servicio SQLite
            builder.Services.AddSingleton<PlantDatabase>(s => new PlantDatabase(dbPath));

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
