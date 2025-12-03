using PlantCareMobile.ViewModels;

namespace PlantCareMobile.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private async void forgotpasswordLabel_Tapped(object sender, TappedEventArgs e)
    {
        gotoForgotPasswordLabel.IsEnabled = false;
        try
        {
            await Shell.Current.GoToAsync("/ForgotPasswordPage");
        }
        finally
        {
            gotoForgotPasswordLabel.IsEnabled = true;
        }

    }
    private void YesNoShowPassImage_Tapped(object sender, TappedEventArgs e)
    {
        passwordEntry.IsPassword = !passwordEntry.IsPassword;
        YesNoShowPassImage.Source = passwordEntry.IsPassword ? "showpassword_icon.png" : "notshowpassword_icon.png";
    }
    private async void GoToRegisterLabel_Tapped(object sender, TappedEventArgs e)
    {
        gotoRegisterLabel.IsEnabled = false;
        try
        {
            await Shell.Current.GoToAsync("../RegisterPage");
        }
        finally
        {
            gotoRegisterLabel.IsEnabled = true;
        }
    }

    private async void loginButton_Clicked(object sender, EventArgs e)
    {
        //AUTENTICACION


        //IR A INICIO (Como si se hubiera iniciado sesion para pruebas)
        //Comentar de ser necesario
        await Shell.Current.GoToAsync("//HomePage");
    }
}