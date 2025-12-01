using SQLite;
using PlantCareMobile.Models;

namespace PlantCareMobile.Services
{
    public class PlantDatabase
    {
        private readonly SQLiteAsyncConnection _conexion;

        public PlantDatabase(string rutaBD)
        {
            _conexion = new SQLiteAsyncConnection(rutaBD);
            _conexion.CreateTableAsync<Plant>().Wait();
        }

        public Task<int> AgregarPlantaAsync(Plant planta)
        {
            return _conexion.InsertAsync(planta);
        }

        public Task<List<Plant>> ObtenerPlantasAsync()
        {
            return _conexion.Table<Plant>()
                .OrderByDescending(p => p.FechaGuardado)
                .ToListAsync();
        }

        public Task<int> EliminarPlantaAsync(Plant planta)
        {
            return _conexion.DeleteAsync(planta);
        }
    }
}
