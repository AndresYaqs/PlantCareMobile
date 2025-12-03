using PlantCareMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantCareMobile.Services
{
    public class ServerAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseAuthService _authService;
        private const string BaseUrl = "https://plantcarebackend-production-686b.up.railway.app"; // URL RAILWAY

        public ServerAPIService(HttpClient httpClient, FirebaseAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        //Metodo para conseguir token de autenticación
        private async Task<bool> SetAuthHeaderAsync()
        {
            string token = await _authService.GetCurrentTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        //Conseguir plantas del usuario
        public async Task<List<Plant>> GetUserPlantsAsync()
        {
            
            try
            {
                if (!await SetAuthHeaderAsync())
                    throw new Exception("Usuario no autenticado");

                var response = await _httpClient.GetAsync($"{BaseUrl}/plants");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Plant>>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo plantas del usuario: {ex.Message}");
                return null;
            }
        }

        // Obtener una planta específica
        public async Task<Plant> GetPlantByIdAsync(int plantId)
        {
            try
            {
                if (!await SetAuthHeaderAsync())
                    throw new Exception("Usuario no autenticado");

                var response = await _httpClient.GetAsync($"{BaseUrl}/plants/{plantId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Plant>(json);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener planta: {ex.Message}");
                return null;
            }
        }

        // Crear una nueva planta
        public async Task<int> CreatePlantAsync(
            Stream photoStream,
            string fileName,
            string sciName,
            string commonName = null,
            string personalName = null,
            string location = null,
            string watering = null,
            string light = null,
            string drainage = null,
            string notes = null)
        {
            try
            {
                if (!await SetAuthHeaderAsync())
                    throw new Exception("Usuario no autenticado");

                var content = new MultipartFormDataContent();

                // Agregar la foto
                var streamContent = new StreamContent(photoStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(streamContent, "photo", fileName);

                // Agregar campos
                content.Add(new StringContent(sciName), "sciName");
                if (!string.IsNullOrEmpty(commonName))
                    content.Add(new StringContent(commonName), "commonName");
                if (!string.IsNullOrEmpty(personalName))
                    content.Add(new StringContent(personalName), "personalName");
                if (!string.IsNullOrEmpty(location))
                    content.Add(new StringContent(location), "location");
                if (!string.IsNullOrEmpty(watering))
                    content.Add(new StringContent(watering), "watering");
                if (!string.IsNullOrEmpty(light))
                    content.Add(new StringContent(light), "light");
                if (!string.IsNullOrEmpty(drainage))
                    content.Add(new StringContent(drainage), "drainage");
                if (!string.IsNullOrEmpty(notes))
                    content.Add(new StringContent(notes), "notes");

                content.Add(new StringContent(DateTime.UtcNow.ToString("o")), "date");

                var response = await _httpClient.PostAsync($"{BaseUrl}/plants", content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CreatePlantResponse>(json);
                return result.Id;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al crear planta: {ex.Message}");
            }
        }

        // Actualizar una planta
        public async Task<bool> UpdatePlantAsync(
            int plantId,
            Stream photoStream = null,
            string fileName = null,
            string sciName = null,
            string commonName = null,
            string personalName = null,
            string location = null,
            string watering = null,
            string light = null,
            string drainage = null,
            string notes = null)
        {
            try
            {
                if (!await SetAuthHeaderAsync())
                    throw new Exception("Usuario no autenticado");

                var content = new MultipartFormDataContent();

                // Solo agregar foto si se proporciona
                if (photoStream != null && !string.IsNullOrEmpty(fileName))
                {
                    var streamContent = new StreamContent(photoStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Add(streamContent, "photo", fileName);
                }

                // Solo agregar campos que no sean null
                if (sciName != null) content.Add(new StringContent(sciName), "sciName");
                if (commonName != null) content.Add(new StringContent(commonName), "commonName");
                if (personalName != null) content.Add(new StringContent(personalName), "personalName");
                if (location != null) content.Add(new StringContent(location), "location");
                if (watering != null) content.Add(new StringContent(watering), "watering");
                if (light != null) content.Add(new StringContent(light), "light");
                if (drainage != null) content.Add(new StringContent(drainage), "drainage");
                if (notes != null) content.Add(new StringContent(notes), "notes");

                var response = await _httpClient.PutAsync($"{BaseUrl}/plants/{plantId}", content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al actualizar planta: {ex.Message}");
            }
        }

        // Eliminar una planta
        public async Task<bool> DeletePlantAsync(int plantId)
        {
            try
            {
                if (!await SetAuthHeaderAsync())
                    throw new Exception("Usuario no autenticado");

                var response = await _httpClient.DeleteAsync($"{BaseUrl}/plants/{plantId}");
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al eliminar planta: {ex.Message}");
            }
        }

        #region "Endpoints de RECORDATORIOS"
        // ENDPOINTS DE RECORDATORIOS
        // ============================================

        //public async Task<List<Reminder>> GetRemindersAsync(int plantId)
        //{
        //    try
        //    {
        //        if (!await SetAuthHeaderAsync())
        //            throw new Exception("Usuario no autenticado");

        //        var response = await _httpClient.GetAsync($"{BaseUrl}/reminders/{plantId}");
        //        response.EnsureSuccessStatusCode();

        //        var json = await response.Content.ReadAsStringAsync();
        //        return JsonSerializer.Deserialize<List<Reminder>>(json);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception($"Error al obtener recordatorios: {ex.Message}");
        //    }
        //}

        //public async Task<int> CreateReminderAsync(Reminder reminder)
        //{
        //    try
        //    {
        //        if (!await SetAuthHeaderAsync())
        //            throw new Exception("Usuario no autenticado");

        //        var json = JsonSerializer.Serialize(reminder);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PostAsync($"{BaseUrl}/reminders", content);
        //        response.EnsureSuccessStatusCode();

        //        var responseJson = await response.Content.ReadAsStringAsync();
        //        var result = JsonSerializer.Deserialize<CreateReminderResponse>(responseJson);
        //        return result.Id;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception($"Error al crear recordatorio: {ex.Message}");
        //    }
        //}

        //public async Task<bool> UpdateReminderAsync(int reminderId, bool completed)
        //{
        //    try
        //    {
        //        if (!await SetAuthHeaderAsync())
        //            throw new Exception("Usuario no autenticado");

        //        var data = new { completed = completed };
        //        var json = JsonSerializer.Serialize(data);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PutAsync($"{BaseUrl}/reminders/{reminderId}", content);
        //        response.EnsureSuccessStatusCode();

        //        return true;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception($"Error al actualizar recordatorio: {ex.Message}");
        //    }
        //}

        //public async Task<bool> DeleteReminderAsync(int reminderId)
        //{
        //    try
        //    {
        //        if (!await SetAuthHeaderAsync())
        //            throw new Exception("Usuario no autenticado");

        //        var response = await _httpClient.DeleteAsync($"{BaseUrl}/reminders/{reminderId}");
        //        response.EnsureSuccessStatusCode();

        //        return true;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception($"Error al eliminar recordatorio: {ex.Message}");
        //    }
        //}
        #endregion

        // Clases auxiliares para respuestas
        public class CreatePlantResponse
        {
            public int Id { get; set; }
        }

        public class CreateReminderResponse
        {
            public int Id { get; set; }
        }
    }
}
