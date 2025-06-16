namespace MauiApp1.Views;
using MauiApp1.Resources.DB;
using System.Collections.ObjectModel;
using MauiApp1.Models;
using MauiApp1.Helpers;
public partial class VocabularyPage : ContentPage
{

    private readonly DatabaseService _databaseService;

    public ObservableCollection<Vocabulary> Vocabularies { get; set; } = new();

    public VocabularyPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // 將 JSON 資料寫入資料庫
        await _databaseService.SeedDataFromFileAsync();
        // 載入資料並更新畫面
        await LoadVocabulariesAsync();
    }

    private async Task LoadVocabulariesAsync()
    {
        // 資料庫取得單字列表
        var itemsFromDb = await _databaseService.GetVocabulariesAsync();
        // await DisplayAlert("偵錯", $"從資料庫讀取到 {itemsFromDb.Count} 個單字。", "OK");
        Vocabularies.Clear(); // 先清空，避免重複

        // 將讀取到的單字逐一加入到 UI 集合中
        if (itemsFromDb != null)
        {
            foreach (var item in itemsFromDb)
            {
                Vocabularies.Add(item);
            }
        }
    }

    // 處理當快速點擊時，後一個聲音會中斷前一個
    private CancellationTokenSource _speechCancellationTokenSource;

    /// <summary>
    /// 當使用者點擊單字卡片時觸發
    /// </summary>
    private async void OnWordTapped(object sender, TappedEventArgs e)
    {
        TapHandlers.HandleWordTapped(sender, e);
    }



}