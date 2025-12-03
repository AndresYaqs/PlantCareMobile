using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlantCareMobile.Models;

[Table("plants")]
public class SavedPlant : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(250)]
    public string ScientificName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string CommonNames { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ImagePath { get; set; } = string.Empty; //MANTENER ASI

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public double Score { get; set; }

    // --- NUEVO CAMPO ---
    // Aquí guardaremos el UDID (ej: "ESP32-TEST")
    [MaxLength(100)]
    public string SensorId { get; set; } = string.Empty;

    // 1. NUEVO CAMPO: Para guardar el apodo en la base de datos
    [MaxLength(100)]
    public string Nickname { get; set; } = string.Empty;

    // 2. PROPIEDAD INTELIGENTE: Esto es lo que mostraremos en la pantalla
    // Si tiene Nickname, lo usa. Si no, usa el Científico.
    [Ignore]
    public string DisplayName => !string.IsNullOrEmpty(Nickname) ? Nickname : ScientificName;

    // --- DATOS EN VIVO (No se guardan en BD Local) ---
    
    private string _liveTemp = "--";
    [Ignore]
    public string LiveTemp 
    { 
        get => _liveTemp;
        set { _liveTemp = value; OnPropertyChanged(); }
    }

    private string _liveHumidity = "--";
    [Ignore]
    public string LiveHumidity 
    { 
        get => _liveHumidity;
        set { _liveHumidity = value; OnPropertyChanged(); }
    }

    // Implementación básica de INotifyPropertyChanged para que la UI se actualice sola
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    


    // --- NUEVOS CAMPOS PARA RECORDATORIOS ---
    
    // Cada cuántos días se debe regar (Por defecto 7 días)
    public int WateringFrequency { get; set; } = 7; 

    // Fecha del último riego (Por defecto, el día que la agregaste)
    public DateTime LastWateredDate { get; set; } = DateTime.Now;

    // --- PROPIEDADES CALCULADAS (No se guardan en BD, se calculan al vuelo) ---

    [Ignore]
    public DateTime NextWateringDate => LastWateredDate.AddDays(WateringFrequency);

    [Ignore]
    public bool IsWateringDue => DateTime.Now.Date >= NextWateringDate.Date;

    [Ignore]
    public string WateringStatus 
    {
        get
        {
            var daysRemaining = (NextWateringDate.Date - DateTime.Now.Date).TotalDays;
            
            if (daysRemaining < 0) return $"Atrasado por {Math.Abs((int)daysRemaining)} días ⚠️";
            if (daysRemaining == 0) return "¡Hoy toca riego! 💧";
            if (daysRemaining == 1) return "Riego mañana ⏳";
            
            return $"Riego en {(int)daysRemaining} días";
        }
    }

    [Ignore]
    public Color StatusColor => IsWateringDue ? Colors.Red : Colors.Green; // Para pintar el texto
}