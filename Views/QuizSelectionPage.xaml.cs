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
            // 透過查詢參數的方式傳送題目數
            // 類似網址 Ex:"QuizPage?quantity=10"
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}?quantity={quantityStr}");
        }
    }
}