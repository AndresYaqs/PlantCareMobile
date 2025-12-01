using PlantCareMobile.Models;
using PlantCareMobile.Services;

namespace PlantCareMobile.Views
{
    public partial class PlantsGalleryPage : ContentPage
    {
        private readonly PlantDatabase _db;

        public PlantsGalleryPage(PlantDatabase db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarPlantas();
        }

        private async Task CargarPlantas()
        {
            var plantas = await _db.ObtenerPlantasAsync();
            ListaPlantas.ItemsSource = plantas;
        }

        private async void OnPlantaSeleccionada(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Plant planta)
            {
                await DisplayAlert("Planta seleccionada",
                    $"{planta.NombreCientifico}\n{planta.NombreComun}",
                    "OK");

                // TODO: Abrir página de detalle si la quieres crear
                ListaPlantas.SelectedItem = null; // Limpia selección
            }
        }

        private async void EliminarPlanta_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Plant planta)
            {
                bool confirm = await DisplayAlert(
                    "Eliminar planta",
                    $"¿Deseas eliminar '{planta.NombreCientifico}' de tu jardín?",
                    "Sí, eliminar",
                    "Cancelar");

                if (!confirm)
                    return;

                await _db.EliminarPlantaAsync(planta);

                await DisplayAlert("🗑️ Eliminada",
                    $"{planta.NombreCientifico} fue eliminada.",
                    "OK");

                await CargarPlantas(); // recargar lista
            }
        }


    }
}
