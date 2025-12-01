using PlantCareMobile.Models;
using PlantCareMobile.Services;

namespace PlantCareMobile.Views;

public partial class HomePage : ContentPage
{
    #region Properties
    private FileResult? selectedImage;
    private List<PlantResult>? identificationResults;
    private readonly PlantIdentificationService plantService;
    #endregion

    #region Constructor
    private readonly PlantDatabase _db;


    public HomePage(PlantDatabase db)
    {
        InitializeComponent();
        plantService = new PlantIdentificationService();
        _db = db;
    }
    #endregion

    private async Task CargarPlantasEnJardin()
    {
        var plantas = await _db.ObtenerPlantasAsync();
        ListaPlantas.ItemsSource = plantas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPlantasEnJardin();
    }

    private async void IrAGaleria(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new PlantsGalleryPage(_db));
    }

    private async void VerTodos_Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PlantsGalleryPage));
    }


    #region Event Handlers
    private async void OnUploadAreaTapped(object sender, EventArgs e)
    {
        try
        {
            var action = await DisplayActionSheet(
                "Seleccionar imagen",
                "Cancelar",
                null,
                "📷 Tomar foto",
                "📁 Seleccionar de galería");

            switch (action)
            {
                case "📷 Tomar foto":
                    await OnTakePhotoClicked();
                    break;
                case "📁 Seleccionar de galería":
                    await OnPickPhotoClicked();
                    break;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    private async void HelpUploadAreaTapped(object sender, EventArgs e)
    {
        await DisplayAlert("HELP", "VEN Y SANA MI DOLOOOOOOOOR, TU TIENES LA CURA DE ESTE AMOOO-OOOHR", "Pero que buena rola");
    }
    #endregion

        #region Photo Methods
    private async Task OnTakePhotoClicked()
    {
        try
        {
            var photo = await MediaPicker.CapturePhotoAsync();
            if (photo != null)
            {
                selectedImage = photo;
                await IdentifyPlant();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al tomar foto: {ex.Message}", "OK");
        }
    }

    private async Task OnPickPhotoClicked()
    {
        try
        {
            var photo = await MediaPicker.PickPhotoAsync();
            if (photo != null)
            {
                selectedImage = photo;
                await IdentifyPlant();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al seleccionar foto: {ex.Message}", "OK");
        }
    }
    #endregion

    #region Plant Identification
    private async Task IdentifyPlant()
    {
        if (selectedImage == null) return;

        try
        {
            // Mostrar loading indicator
            SetLoadingVisibility(true);

            // Llamar al servicio de identificación
            var result = await plantService.IdentifyPlantAsync(selectedImage);

            // Procesar resultados
            await ProcessIdentificationResults(result);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al identificar planta: {ex.Message}", "OK");
        }
        finally
        {
            // Ocultar loading indicator
            SetLoadingVisibility(false);
        }
    }

    private async Task ProcessIdentificationResults(PlantNetResponse? result)
    {
        if (result?.Results == null || result.Results.Count == 0)
        {
            await DisplayAlert("Sin resultados",
                "No se encontraron resultados. Intenta con otra imagen más clara.", "OK");
        }
        else
        {
            identificationResults = result.Results;
            await ShowResults();
        }
    }

    private async Task ShowResults()
    {
        if (identificationResults != null && identificationResults.Count > 0)
        {
            var mainResult = identificationResults.First();
            var message = BuildResultMessage(mainResult);

            var saveResult = await DisplayAlert("¡Planta Identificada!", message, "💾 Guardar", "Cerrar");

            if (saveResult)
            {
                await SavePlant(mainResult);
            }
        }
    }

    private string BuildResultMessage(PlantResult mainResult)
    {
        var commonNames = mainResult.Species?.CommonNames != null && mainResult.Species.CommonNames.Count > 0
            ? string.Join(", ", mainResult.Species.CommonNames)
            : "N/A";

        var message = $"🌱 {mainResult.Species?.ScientificNameWithoutAuthor ?? "Desconocido"}\n\n" +
                     $"Nombres comunes: {commonNames}\n\n" +
                     $"Puntuación: {(mainResult.Score * 100):F2}%";

        // Agregar otras coincidencias si las hay
        if (identificationResults != null && identificationResults.Count > 1)
        {
            message += "\n\n🌿 Otras posibles coincidencias:";
            for (int i = 1; i < Math.Min(4, identificationResults.Count); i++)
            {
                var otherResult = identificationResults[i];
                message += $"\n• {otherResult.Species?.ScientificNameWithoutAuthor} ({(otherResult.Score * 100):F2}%)";
            }
        }

        return message;
    }

    private async Task SavePlant(PlantResult plant)
    {
        if (plant?.Species == null)
            return;

        var nuevaPlanta = new Plant
        {
            NombreCientifico = plant.Species.ScientificNameWithoutAuthor ?? "Desconocido",
            NombreComun = plant.Species.CommonNames?.FirstOrDefault() ?? "Sin nombre",
            PuntajeIdentificacion = plant.Score,
            RutaImagen = selectedImage?.FullPath ?? string.Empty,
            FechaGuardado = DateTime.Now
        };

        await _db.AgregarPlantaAsync(nuevaPlanta);

        await DisplayAlert("✅ Éxito",
            $"'{nuevaPlanta.NombreCientifico}' guardada en tu jardín",
            "OK");

        await CargarPlantasEnJardin();
    }


    #endregion

    #region UI Helper Methods
    private void SetLoadingVisibility(bool isVisible)
    {
        var loadingIndicator = this.FindByName<StackLayout>("LoadingIndicator");
        if (loadingIndicator != null)
        {
            loadingIndicator.IsVisible = isVisible;
        }
    }
    #endregion


   


}