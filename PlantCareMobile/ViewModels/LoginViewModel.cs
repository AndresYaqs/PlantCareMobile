using Microsoft.Maui.Devices.Sensors;
using PlantCareMobile.Models;
using PlantCareMobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PlantCareMobile.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region "Properties"
        private readonly FirebaseAuthService _authService;
        private readonly ServerAPIService _apiService;
        private readonly PlantDatabaseService _localDBService;

        // CAMPOS PRIVADOS
        private string _email;
        private string _password;
        private bool _isBusy;
        private string _errorMessage;
        private bool _hasErrorMessage;

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

        // Nueva propiedad para indicar si hay un mensaje de error
        public bool HasErrorMessage
        {
            get => _hasErrorMessage;
            set
            {
                if (_hasErrorMessage != value)
                {
                    _hasErrorMessage = value;
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

                    if (!string.IsNullOrEmpty(_errorMessage))
                    {
                        _hasErrorMessage = true;
                    }
                    else
                    {
                        _hasErrorMessage = false;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasErrorMessage));
                }
            }
        }
        #endregion
        #region "Commands"
        // 3. COMANDOS
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        #endregion
        #region "Constructor"
        public LoginViewModel(FirebaseAuthService authService, ServerAPIService apiService, PlantDatabaseService localDBService)
        {
            _authService = authService;
            _apiService = apiService;
            _localDBService = localDBService;

            // Inicialización de Comandos
            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            ForgotPasswordCommand = new Command(async () => await ForgotPasswordAsync());
        }
        #endregion
        #region "Main Commands"
        // MÉTODOS DE LÓGICA (Comandos principales)
        private async Task LoginAsync()
        {
            if (!await _apiService.CheckHealth()) {
                ErrorMessage = "NO SE DETECTO CONEXION CON EL SERVIDOR";
                return;
            }

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

                    //Limpiar DB LOCAL
                    await _localDBService.ResetLocalDataAsync();

                    //Logica para conseguir plantas y recordatorios del usuario

                    // -- Traer datos del servidor a DB LOCAL
                    var serverplants = await _apiService.GetUserPlantsAsync();

                    if (serverplants != null && serverplants.Count > 0)
                    {
                        foreach (var remoteplant in serverplants)
                        {
                            var localPlant = new SavedPlant
                            {
                                ScientificName = remoteplant.ScientificName,
                                CommonNames = remoteplant.CommonNames,
                                Location = remoteplant.Location,
                                Nickname = remoteplant.Nickname ?? "", // <--- Aquí guardamos el apodo
                                ImagePath = remoteplant.ImagePath, // <--- Imagen hospedada publicamente en servidor
                                Score = remoteplant.Score,
                                DateAdded = remoteplant.DateAdded,
                                SensorId = remoteplant.SensorId,
                                LastWateredDate = remoteplant.LastWateredDate,

                            };

                            //Guardar planta en DB local (Descomentar)
                            await _localDBService.SavePlantAsync(localPlant);
                        }
                    }

                    Console.WriteLine($"Plantas del usuario: {serverplants.Count}");

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
            if (!await _apiService.CheckHealth())
            {
                ErrorMessage = "NO SE DETECTO CONEXION CON EL SERVIDOR";
                return;
            }
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
            if (!await _apiService.CheckHealth())
            {
                ErrorMessage = "NO SE DETECTO CONEXION CON EL SERVIDOR";
                return;
            }
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
                    await Shell.Current.GoToAsync("..");
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
        #endregion
        #region "Event Handlers"
        // IMPLEMENTACIÓN DE INotifyPropertyChanged BOILERPLATE
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region "Methods"
        // MÉTODOS AUXILIARES (Solo Si se necesitan)
        private void PrepareToLogin()
        {
            CleanLocalDB();
            GetDataFromDB();
        }
        private bool CleanLocalDB()
        {

            return false;
        }
        private bool GetDataFromDB()
        {
            return false;
        }

        #endregion
    }
}
