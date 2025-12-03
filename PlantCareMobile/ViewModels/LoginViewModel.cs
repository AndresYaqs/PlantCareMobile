using PlantCareMobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PlantCareMobile.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly Services.FirebaseAuthService _authService;
        private readonly ServerAPIService _apiService;

        // CAMPOS PRIVADOS
        private string _email;
        private string _password;
        private bool _isBusy;
        private string _errorMessage;

        // 2. PROPIEDADES PÚBLICAS (Con notificación de cambios)
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(); // Notifica a la vista
                    // También notificamos al comando que su estado puede haber cambiado (para habilitar/deshabilitar botón)
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        // 3. COMANDOS
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel(FirebaseAuthService authService, ServerAPIService apiService)
        {
            _authService = authService;
            _apiService = apiService;

            // Inicialización de Comandos
            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            ForgotPasswordCommand = new Command(async () => await ForgotPasswordAsync());
        }

        // MÉTODOS DE LÓGICA
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor completa todos los campos";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _authService.LoginAsync(Email, Password);

                if (result.Success)
                {
                    Console.WriteLine($"Usuario autenticado: {result.UserId}");
                    Console.WriteLine($"Token: {result.Token}");

                    var plants = await _apiService.GetUserPlantsAsync();
                    Console.WriteLine($"Plantas del usuario: {plants.Count}");

                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor completa todos los campos";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _authService.RegisterAsync(Email, Password);

                if (result.Success)
                {
                    ErrorMessage = "Registro exitoso. Iniciando sesión...";
                    await LoginAsync();
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Por favor ingresa tu correo electrónico";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _authService.ResetPasswordAsync(Email);

                if (result.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Éxito",
                        "Se ha enviado un correo para restablecer tu contraseña",
                        "OK");
                }
                else
                {
                    ErrorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // IMPLEMENTACIÓN DE INotifyPropertyChanged BOILERPLATE
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
    