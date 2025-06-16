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
        // 從資料庫取得所有錯題
        var words = await _databaseService.GetMistakeVocabulariesAsync();

        // 檢查錯題列表是否為空
        if (words == null || !words.Any())
        {
            bool goToQuiz = await DisplayAlert(
                "沒有錯題記錄!",
                "是否立即前往測驗？",
                "前往測驗",
                "暫時不要");

            if (goToQuiz)
            {
                // 使用者選擇(是)
                await Shell.Current.GoToAsync($"../{nameof(QuizSelectionPage)}");
            }
            else
            {
                // 使用者選擇(否)
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