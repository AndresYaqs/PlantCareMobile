using PlantCareMobile.ViewModels;

namespace PlantCareMobile.Views;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage(LoginViewModel viewmodel)
	{
		InitializeComponent();
		BindingContext = viewmodel;
	}

    private async void GoBackLabel_Tapped(object sender, TappedEventArgs e)
    {
		await Shell.Current.GoToAsync("..");
    }
}