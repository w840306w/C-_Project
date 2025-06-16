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
            // �z�L�d�߰Ѽƪ��覡�ǰe�D�ؼ�
            // �������} Ex:"QuizPage?quantity=10"
            await Shell.Current.GoToAsync($"{nameof(QuizPage)}?quantity={quantityStr}");
        }
    }
}