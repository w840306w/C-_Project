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
        //��Ʈw���J��r��
        if (!_allWords.Any())
        {
            await _databaseService.SeedDataFromFileAsync();
            _allWords = await _databaseService.GetVocabulariesAsync();
        }

        if (_allWords == null || !_allWords.Any())
        {
            DailyWord = new Vocabulary { Word = "���~", Definition = "��Ʈw���ũθ��J����" };
            return;
        }

        //���o�̫��s�ɶ��M�W�@�����C���r
        var lastUpdateDate = Preferences.Get("WordLastUpdateDate", DateTime.MinValue.Date);
        var savedWordId = Preferences.Get("DailyWordId", -1);
        var today = DateTime.Now.Date;

        //�P�_�O����s�٬O��ʧ�s
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


            // �p����ت��`�P��
            double perimeter = (cardWidth * 2) + (cardHeight * 2);

            // �p��C���䪺���צb�`�P�����Ҧ������
            double topBottomDuration = cardWidth / perimeter; // �W�U��ئ��Ϊ��ɶ����
            double leftRightDuration = cardHeight / perimeter; // ���k��ئ��Ϊ��ɶ����

            // �ھڤ�ҭp��X���ʵe�ɶ��b�W���|�Ӯɶ��I
            double t1_endTop = topBottomDuration;                  // �W��ص������ɶ��I
            double t2_endRight = t1_endTop + leftRightDuration;    // �k��ص������ɶ��I
            double t3_endBottom = t2_endRight + topBottomDuration; // �U��ص������ɶ��I
                                                                   // ����ص������ɶ��I�|�O 1.0

            // �إߤ��ʵe
            var parentAnimation = new Animation();

            // �إߥ|�Ӥl�ʵe
            var topAnim = new Animation(x => LightDot.TranslationX = x, 0 - (dotWidth / 2), cardWidth - (dotWidth / 2));
            var rightAnim = new Animation(y => LightDot.TranslationY = y, 0 - (dotHeight / 2), cardHeight - (dotHeight / 2));
            var bottomAnim = new Animation(x => LightDot.TranslationX = x, cardWidth - (dotWidth / 2), 0 - (dotWidth / 2));
            var leftAnim = new Animation(y => LightDot.TranslationY = y, cardHeight - (dotHeight / 2), 0 - (dotHeight / 2));

            // �N�l�ʵe�w�Ʀb(����ҭp��X)���ɶ��b�W
            parentAnimation.Add(0, t1_endTop, topAnim);
            parentAnimation.Add(t1_endTop, t2_endRight, rightAnim);
            parentAnimation.Add(t2_endRight, t3_endBottom, bottomAnim);
            parentAnimation.Add(t3_endBottom, 1.0, leftAnim);

            // ����ð�����ʵe 
            parentAnimation.Commit(this, "BorderFullPathAnimation", length: 8000, repeat: () => true);
        });
    }

    private Vocabulary GetRandomWord()
    {
        //�H�����o��r
        return _allWords[_random.Next(_allWords.Count)];
    }

    /// <summary>
    /// �ϥΪ��I���C���r�d
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnWordOfTheDay_Tapped(object sender, TappedEventArgs e)
    {
        await UpdateWordOfTheDay(forceNew: true);
    }
    /// <summary>
    /// �ϥΪ��I���C���r�d�n���ϥ�
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

    //UI��s
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}