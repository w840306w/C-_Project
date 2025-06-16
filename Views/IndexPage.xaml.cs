using System.ComponentModel;
using MauiApp1.Models;
using MauiApp1.Resources.DB;
using MauiApp1.Helpers;
using CommunityToolkit.Maui.Views;
using MauiApp1.Views;
using Microsoft.Maui.Controls;


namespace MauiApp1.Views;

public partial class IndexPage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private List<Vocabulary> _allWords = new();
    private readonly Random _random = new();

    private CancellationTokenSource _speechCancellationTokenSource;

    private Vocabulary _dailyWord;
    public Vocabulary DailyWord { get => _dailyWord; set { _dailyWord = value; OnPropertyChanged(); } }

    public IndexPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await UpdateWordOfTheDay();

        StartBorderAnimation();

    }

    private async Task UpdateWordOfTheDay(bool forceNew = false)
    {
        //資料庫載入單字表
        if (!_allWords.Any())
        {
            await _databaseService.SeedDataFromFileAsync();
            _allWords = await _databaseService.GetVocabulariesAsync();
        }

        if (_allWords == null || !_allWords.Any())
        {
            DailyWord = new Vocabulary { Word = "錯誤", Definition = "資料庫為空或載入失敗" };
            return;
        }

        //取得最後更新時間和上一次的每日單字
        var lastUpdateDate = Preferences.Get("WordLastUpdateDate", DateTime.MinValue.Date);
        var savedWordId = Preferences.Get("DailyWordId", -1);
        var today = DateTime.Now.Date;

        //判斷是跨日更新還是手動更新
        if (!forceNew && lastUpdateDate == today && savedWordId != -1)
        {
            DailyWord = _allWords.FirstOrDefault(w => w.ID == savedWordId) ?? GetRandomWord();
        }
        else
        {
            DailyWord = GetRandomWord();
            Preferences.Set("DailyWordId", DailyWord.ID);
            Preferences.Set("WordLastUpdateDate", today);
        }
    }

    private void StartBorderAnimation()
    {
        this.Dispatcher.Dispatch(() =>
        {
            double cardWidth = CardBorder.Width;
            double cardHeight = CardBorder.Height;
            double dotWidth = LightDot.Width;
            double dotHeight = LightDot.Height;

            if (cardWidth <= 0 || cardHeight <= 0)
                return;


            // 計算邊框的總周長
            double perimeter = (cardWidth * 2) + (cardHeight * 2);

            // 計算每個邊的長度在總周長中所佔的比例
            double topBottomDuration = cardWidth / perimeter; // 上下邊框佔用的時間比例
            double leftRightDuration = cardHeight / perimeter; // 左右邊框佔用的時間比例

            // 根據比例計算出父動畫時間軸上的四個時間點
            double t1_endTop = topBottomDuration;                  // 上邊框結束的時間點
            double t2_endRight = t1_endTop + leftRightDuration;    // 右邊框結束的時間點
            double t3_endBottom = t2_endRight + topBottomDuration; // 下邊框結束的時間點
                                                                   // 左邊框結束的時間點會是 1.0

            // 建立父動畫
            var parentAnimation = new Animation();

            // 建立四個子動畫
            var topAnim = new Animation(x => LightDot.TranslationX = x, 0 - (dotWidth / 2), cardWidth - (dotWidth / 2));
            var rightAnim = new Animation(y => LightDot.TranslationY = y, 0 - (dotHeight / 2), cardHeight - (dotHeight / 2));
            var bottomAnim = new Animation(x => LightDot.TranslationX = x, cardWidth - (dotWidth / 2), 0 - (dotWidth / 2));
            var leftAnim = new Animation(y => LightDot.TranslationY = y, cardHeight - (dotHeight / 2), 0 - (dotHeight / 2));

            // 將子動畫安排在(按比例計算出)的時間軸上
            parentAnimation.Add(0, t1_endTop, topAnim);
            parentAnimation.Add(t1_endTop, t2_endRight, rightAnim);
            parentAnimation.Add(t2_endRight, t3_endBottom, bottomAnim);
            parentAnimation.Add(t3_endBottom, 1.0, leftAnim);

            // 提交並執行父動畫 
            parentAnimation.Commit(this, "BorderFullPathAnimation", length: 8000, repeat: () => true);
        });
    }

    private Vocabulary GetRandomWord()
    {
        //隨機取得單字
        return _allWords[_random.Next(_allWords.Count)];
    }

    /// <summary>
    /// 使用者點擊每日單字卡
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnWordOfTheDay_Tapped(object sender, TappedEventArgs e)
    {
        await UpdateWordOfTheDay(forceNew: true);
    }
    /// <summary>
    /// 使用者點擊每日單字卡聲音圖示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SpeakDailyWord_Clicked(object sender, EventArgs e)
    {
        await TapHandlers.SpeakWordAsync(DailyWord?.Word);
    }
    private async void Button_Clicked_Learn(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VocabularyPage));
    }

    private async void Button_Clicked_Test(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(QuizSelectionPage));
    }

    private async void Button_Clicked_Review(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MistakeReviewPage));
    }

    //UI更新
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}