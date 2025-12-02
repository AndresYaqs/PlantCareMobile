using PlantCareMobile.ViewModels;

namespace PlantCareMobile.Views;

public partial class PlantsGalleryPage : ContentPage
{
    private readonly PlantsGalleryViewModel _viewModel;

    public PlantsGalleryPage()
    {
        InitializeComponent();
        _viewModel = (PlantsGalleryViewModel)BindingContext;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPlantsAsync();
    }
}