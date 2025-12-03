using PlantCareMobile.ViewModels;

namespace PlantCareMobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void YesNoShowPassImage_Tapped(object sender, TappedEventArgs e)
    {
        passwordEntry.IsPassword = !passwordEntry.IsPassword;
        YesNoShowPassImage.Source = passwordEntry.IsPassword ? "showpassword_icon.png" : "notshowpassword_icon.png";
    }
    private async void GoToLoginLabel_Tapped(object sender, TappedEventArgs e)
    {

        gotoLoginLabel.IsEnabled = false;
        try
        {
            await Shell.Current.GoToAsync("../LoginPage");
        }
        finally
        {
            gotoLoginLabel.IsEnabled = true;
        }
    }
}