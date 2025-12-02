using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PlantCareMobile.Models;
using PlantCareMobile.Services;

namespace PlantCareMobile.ViewModels;

public class PlantsGalleryViewModel : INotifyPropertyChanged
{
    private readonly PlantDatabaseService _databaseService;
    private ObservableCollection<SavedPlant> _plants;
    private bool _isLoading;
    private bool _hasPlants;

    public ObservableCollection<SavedPlant> Plants
    {
        get => _plants;
        set
        {
            _plants = value;
            OnPropertyChanged();
            HasPlants = _plants?.Count > 0;
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public bool HasPlants
    {
        get => _hasPlants;
        set
        {
            _hasPlants = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoadPlantsCommand { get; }
    public ICommand DeletePlantCommand { get; }

    public PlantsGalleryViewModel()
    {
        _databaseService = new PlantDatabaseService();
        _plants = new ObservableCollection<SavedPlant>();
        LoadPlantsCommand = new Command(async () => await LoadPlantsAsync());
        DeletePlantCommand = new Command<SavedPlant>(async (plant) => await DeletePlantAsync(plant));

        // Suscribirse al evento de planta guardada
        PlantMessenger.Subscribe("PlantSaved", async (args) => await LoadPlantsAsync());
    }

    public async Task LoadPlantsAsync()
    {
        IsLoading = true;
        try
        {
            var plants = await _databaseService.GetPlantsAsync();
            Plants = new ObservableCollection<SavedPlant>(plants);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading plants: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeletePlantAsync(SavedPlant plant)
    {
        if (plant == null) return;

        try
        {
            await _databaseService.DeletePlantAsync(plant);
            Plants.Remove(plant);
            
            if (!string.IsNullOrEmpty(plant.ImagePath) && File.Exists(plant.ImagePath))
            {
                File.Delete(plant.ImagePath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting plant: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}