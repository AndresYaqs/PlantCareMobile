using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using PlantCareMobile.Services;
using PlantCareMobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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
            
            // --- CONFIGURACIÓN DE LA API ---
            
            // Ya no necesitamos la lógica de localhost/10.0.2.2 porque usaremos el servidor externo
            
            builder.Services.AddHttpClient<ApiService>(client =>
            {
                // URL Real del servidor de desarrollo proporcionada por tu compañero
                client.BaseAddress = new Uri("http://vm.drcvault.dev/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // --- REGISTRO DE SERVICIOS ---
            builder.Services.AddSingleton<PlantDatabaseService>(); // Tu servicio de base de datos local
            builder.Services.AddSingleton<FirebaseAuthService>(); // Tu servicio de autenticación (FIREBASE)
            builder.Services.AddSingleton<ServerAPIService>(); // Tu servicio de API al servidor
            builder.Services.AddTransient<PlantsGalleryViewModel>(); // Tu ViewModel existente
            builder.Services.AddTransient<Views.PlantsGalleryPage>(); // Tu página existente
            builder.Services.AddTransient<ViewModels.HomeViewModel>();
            builder.Services.AddTransient<Views.HomePage>();
            builder.Services.AddTransient<ViewModels.RemindersViewModel>();
            builder.Services.AddTransient<Views.RemindersPage>();
            builder.Services.AddTransient<LoginViewModel>(); //PARA PAGINA DE LOGIN
            builder.Services.AddTransient<Views.LoginPage>(); //PARA PAGINA DE LOGIN
            builder.Services.AddTransient<ProfileViewModel>(); //PARA PAGINA DE Profile
            builder.Services.AddTransient<Views.ProfilePage>(); //PARA PAGINA DE Profile


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
