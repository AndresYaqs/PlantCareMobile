using SQLite;
using PlantCareMobile.Models;

namespace PlantCareMobile.Services;

public class PlantDatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        try
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "plants.db3");
            System.Diagnostics.Debug.WriteLine($"📁 Database path: {dbPath}");
            
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<SavedPlant>();
            
            System.Diagnostics.Debug.WriteLine("✅ Database initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error initializing database: {ex.Message}");
            throw;
        }
    }

    public async Task<List<SavedPlant>> GetPlantsAsync()
    {
        try
        {
            await InitAsync();
            var plants = await _database!.Table<SavedPlant>()
                .OrderByDescending(p => p.DateAdded)
                .ToListAsync();
            
            System.Diagnostics.Debug.WriteLine($"🌱 Retrieved {plants.Count} plants from database");
            return plants;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error getting plants: {ex.Message}");
            return new List<SavedPlant>();
        }
    }

    public async Task<SavedPlant> GetPlantAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<SavedPlant>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SavePlantAsync(SavedPlant plant)
    {
        try
        {
            await InitAsync();
            int result;
            
            if (plant.Id != 0)
            {
                result = await _database!.UpdateAsync(plant);
                System.Diagnostics.Debug.WriteLine($"✏️ Updated plant: {plant.ScientificName}");
            }
            else
            {
                result = await _database!.InsertAsync(plant);
                System.Diagnostics.Debug.WriteLine($"💾 Saved new plant: {plant.ScientificName}");
            }
            
            // Verificar que se guardó
            var count = await _database!.Table<SavedPlant>().CountAsync();
            System.Diagnostics.Debug.WriteLine($"📊 Total plants in database: {count}");
            
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error saving plant: {ex.Message}");
            throw;
        }
    }

    public async Task<int> DeletePlantAsync(SavedPlant plant)
    {
        try
        {
            await InitAsync();
            var result = await _database!.DeleteAsync(plant);
            System.Diagnostics.Debug.WriteLine($"🗑️ Deleted plant: {plant.ScientificName}");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error deleting plant: {ex.Message}");
            throw;
        }
    }
}