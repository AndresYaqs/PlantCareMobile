using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlantCareMobile.Models;
using PlantCareMobile.Services;

namespace PlantCareMobile.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        #region "Properties"
        private readonly PlantDatabaseService _databaseService;

        // --- Propiedades para manejo de usuario y peticiones
        private string userName; // <-- Nombre de usuario
        private readonly FirebaseAuthService _authService; // <-- Servicio de autenticación
        private readonly ServerAPIService _serverapiService; // <-- Servicio SERVER BACKEND API


        // --- 1. Propiedades para "Mi Jardín" (Recientes) ---
        private ObservableCollection<SavedPlant> _recentPlants;
        private bool _hasPlants;

        public ObservableCollection<SavedPlant> RecentPlants
        {
            get => _recentPlants;
            set { _recentPlants = value; OnPropertyChanged(); }
        }
        public bool HasPlants
        {
            get => _hasPlants;
            set { _hasPlants = value; OnPropertyChanged(); }
        }

        // --- 2. NUEVAS Propiedades para "Recordatorios" (Riego) ---
        private ObservableCollection<SavedPlant> _plantsToWater;
        private bool _hasReminders;

        public ObservableCollection<SavedPlant> PlantsToWater
        {
            get => _plantsToWater;
            set { _plantsToWater = value; OnPropertyChanged(); }
        }
        public bool HasReminders
        {
            get => _hasReminders;
            set { _hasReminders = value; OnPropertyChanged(); }
        }
        public string UserName { get => userName; set => userName = value; }

        #endregion

        public HomeViewModel(PlantDatabaseService databaseService, FirebaseAuthService authService, ServerAPIService serverapiService)
        {
            _databaseService = databaseService;
            _recentPlants = new ObservableCollection<SavedPlant>();
            _plantsToWater = new ObservableCollection<SavedPlant>();
            _authService = authService;
            _serverapiService = serverapiService;

            LoadUsername();
            PlantMessenger.Subscribe("PlantSaved", async (args) => await LoadDataAsync());
            
            // Carga inicial
            Task.Run(async () => await LoadDataAsync());
        }

        #region "Methods"
        // Renombramos el método para que sea más general
        public async Task LoadDataAsync()
        {
            try
            {
                var plants = await _databaseService.GetPlantsAsync();
                
                // A. Lógica de "Mi Jardín" (Las 3 más recientes)
                var recent = plants.OrderByDescending(p => p.DateAdded).Take(3).ToList();

                // B. Lógica de "Recordatorios" (Las que necesitan agua YA)
                // Filtramos donde IsWateringDue es verdadero
                var urgent = plants.Where(p => p.IsWateringDue)
                                   .OrderBy(p => p.NextWateringDate)
                                   .Take(3) // Solo mostramos 3 para no saturar el inicio
                                   .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Actualizar Recientes
                    RecentPlants = new ObservableCollection<SavedPlant>(recent);
                    HasPlants = RecentPlants.Count > 0;

                    // Actualizar Recordatorios
                    PlantsToWater = new ObservableCollection<SavedPlant>(urgent);
                    HasReminders = PlantsToWater.Count > 0;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando home: {ex.Message}");
            }
        }

        private void LoadUsername()
        {
            UserName = _authService.GetCurrentUserDisplayName();
            if (!string.IsNullOrEmpty(UserName) && UserName.Contains("@"))
            {
                UserName = _authService.GetCurrentUserEmail();
                UserName = UserName.Split('@')[0];
            }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}