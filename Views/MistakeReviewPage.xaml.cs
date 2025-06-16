namespace MauiApp1.Views;
using MauiApp1.Resources.DB;
using System.Collections.ObjectModel;
using MauiApp1.Models;
using MauiApp1.Helpers;

public partial class MistakeReviewPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    public ObservableCollection<Vocabulary> MistakeWords { get; set; } = new();

    public MistakeReviewPage()
	{
		InitializeComponent();
	}

    public MistakeReviewPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMistakesAsync();
    }

    private async Task LoadMistakesAsync()
    {
        // �q��Ʈw���o�Ҧ����D
        var words = await _databaseService.GetMistakeVocabulariesAsync();

        // �ˬd���D�C��O�_����
        if (words == null || !words.Any())
        {
            bool goToQuiz = await DisplayAlert(
                "�S�����D�O��!",
                "�O�_�ߧY�e������H",
                "�e������",
                "�Ȯɤ��n");

            if (goToQuiz)
            {
                // �ϥΪ̿��(�O)
                await Shell.Current.GoToAsync($"../{nameof(QuizSelectionPage)}");
            }
            else
            {
                // �ϥΪ̿��(�_)
                await Shell.Current.GoToAsync("..");
            }
        }
        else
        {
            MistakeWords.Clear();
            foreach (var word in words)
            {
                MistakeWords.Add(word);
            }
        }
    }

    private void OnWordTapped(object sender, TappedEventArgs e)
    {
        TapHandlers.HandleWordTapped(sender, e);
    }

}