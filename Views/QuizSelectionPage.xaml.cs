namespace MauiApp1.Views;

public partial class QuizSelectionPage : ContentPage
{
    public QuizSelectionPage()
    {
        InitializeComponent();
    }

    private async void OnQuantitySelected_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string quantityStr)
        {
            // 硓筁琩高把计よΑ肚癳肈ヘ计
            // 摸呼 Ex:"QuizPage?quantity=10"
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}?quantity={quantityStr}");
        }
    }
}