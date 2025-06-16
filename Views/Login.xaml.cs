using System.Diagnostics;

namespace MauiApp1.Views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}
    void LoginButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Clicked !");
    }
}