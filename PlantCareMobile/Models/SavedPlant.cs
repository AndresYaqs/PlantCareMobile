using SQLite;

namespace PlantCareMobile.Models;

[Table("plants")]
public class SavedPlant
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
    public string ImagePath { get; set; } = string.Empty;

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public double Score { get; set; }
}