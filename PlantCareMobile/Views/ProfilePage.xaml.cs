using PlantCareMobile.ViewModels;

namespace PlantCareMobile.Views;

public partial class ProfilePage : ContentPage
{
    #region "properties"
    private readonly ProfileViewModel _viewmodel;
    #endregion
    public ProfilePage(ProfileViewModel viewmodel)
	{
		InitializeComponent();
		_viewmodel = viewmodel;
        BindingContext = _viewmodel;
	}



}