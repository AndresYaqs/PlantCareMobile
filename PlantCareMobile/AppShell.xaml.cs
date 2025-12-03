using PlantCareMobile.Services;
using PlantCareMobile.Views;

namespace PlantCareMobile
{
    public partial class AppShell : Shell
    {
        private readonly FirebaseAuthService _authservice;
        public AppShell(FirebaseAuthService authService)
        {
            InitializeComponent();
            _authservice = authService;
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("ForgotPasswordPage", typeof(ForgotPasswordPage));
        }

        private async void logoutButton_Clicked(object sender, EventArgs e)
        {
            //LOGICA PARA CERRAR SESION
            if (_authservice.IsAuthenticated())
                _authservice.Logout();

            Current.FlyoutIsPresented = false;
            //VOLVER a pagina de bienvenida
            await Shell.Current.GoToAsync("//WelcomePage");
        }
    }
}
