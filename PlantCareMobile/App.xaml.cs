using PlantCareMobile.Services;

namespace PlantCareMobile
{
    public partial class App : Application
    {
        private readonly FirebaseAuthService _authService;
        public App(FirebaseAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(_authService));
        }
    }
}