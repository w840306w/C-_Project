using System.ComponentModel;
using System.Linq;
using MauiApp1.Models;
using MauiApp1.Resources.DB;
using MauiApp1.Helpers;

namespace MauiApp1.Views;

[QueryProperty(nameof(TotalQuestions), "quantity")]
public partial class QuizPage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private List<Vocabulary> _allWords = new();
    private List<Vocabulary> _sessionWords = new();
    private Vocabulary _currentWord;
    private int _currentQuestionIndex = 0;
    private readonly Random _random = new();
    private CancellationTokenSource _speechCancellationTokenSource;

    #region Bindable Properties
    private int _totalQuestions;
    public int TotalQuestions { get => _totalQuestions; set { _totalQuestions = value; } }

    private string _progressText;
    public string ProgressText { get => _progressText; set { _progressText = value; OnPropertyChanged(); } }

    private bool _isQuizInProgress = true;
    public bool IsQuizInProgress { get => _isQuizInProgress; set { _isQuizInProgress = value; OnPropertyChanged(); } }

    private string _hint;
    public string Hint { get => _hint; set { _hint = value; OnPropertyChanged(); } }

    private string _maskedWord;
    public string MaskedWord { get => _maskedWord; set { _maskedWord = value; OnPropertyChanged(); } }

    private string _userAnswer;
    public string UserAnswer { get => _userAnswer; set { _userAnswer = value; OnPropertyChanged(); } }

    private string _resultText;
    public string ResultText { get => _resultText; set { _resultText = value; OnPropertyChanged(); } }

    private Color _resultColor;
    public Color ResultColor { get => _resultColor; set { _resultColor = value; OnPropertyChanged(); } }

    private string _pageTitle = "單字測驗";
    public string PageTitle { get => _pageTitle; set { _pageTitle = value; OnPropertyChanged(); } }

    public List<Vocabulary> SessionWords => _sessionWords;

    private string _finalScoreText;
    public string FinalScoreText { get => _finalScoreText; set { _finalScoreText = value; OnPropertyChanged(); } }
    #endregion

    public QuizPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (TotalQuestions > 0 && !_sessionWords.Any())
        {
            await PrepareQuizSession();
        }
    }

    private async Task PrepareQuizSession()
    {
        if (!_allWords.Any())
        {
            _allWords = await _databaseService.GetVocabulariesAsync();
        }

        if (_allWords == null || _allWords.Any() == false)
        {
            await DisplayAlert("錯誤", "無法從資料庫載入任何單字。", "確定");
            await Shell.Current.GoToAsync("..");
            return;
        }

        _sessionWords = _allWords.OrderBy(x => _random.Next()).Take(TotalQuestions).ToList();
        _currentQuestionIndex = 0;

        // 重設狀態
        _currentState = QuizState.Answering;
        OnPropertyChanged(nameof(IsAnswering));
        OnPropertyChanged(nameof(IsReviewing));

        GenerateNewQuestion();
    }

    private void GenerateNewQuestion()
    {
        if (_sessionWords == null || _currentQuestionIndex >= _sessionWords.Count)
        {
            PageTitle = "測驗結果";
            Hint = "測驗完成！🎉";
            MaskedWord = "做得好！";
            ProgressText = $"總共 {TotalQuestions} 題";

            int correctCount = _sessionWords?.Count(w => w.IsCorrect) ?? 0;
            double accuracy = (TotalQuestions > 0) ? (double)correctCount / TotalQuestions * 100 : 0;
            FinalScoreText = $"正確率: {accuracy:F1}% ({correctCount} / {TotalQuestions})";

            IsQuizInProgress = false;
            return;
        }

        PageTitle = "單字測驗";
        IsQuizInProgress = true;
        ProgressText = $"第 {_currentQuestionIndex + 1} / {TotalQuestions} 題";
        _currentWord = _sessionWords[_currentQuestionIndex];

        if (!string.IsNullOrEmpty(_currentWord.PartOfSpeech))
        {
            Hint = $"{_currentWord.Definition} ({_currentWord.PartOfSpeech})";
        }
        else
        {
            Hint = _currentWord.Definition;
        }

        MaskedWord = GetMaskedWord(_currentWord.Word);
        UserAnswer = _currentWord.UserAnswer ?? string.Empty;
        ResultText = string.Empty;
    }

    private string GetMaskedWord(string word)
    {
        if (string.IsNullOrEmpty(word) || word.Length <= 2)
        {
            return word;
        }

        // 取得頭尾字母
        char firstLetter = word[0];
        char lastLetter = word[word.Length - 1];

        // 計算中間應有多少個底線
        int middleCount = word.Length - 2;

        // 建立一個包含多個底線的序列
        var underscores = Enumerable.Repeat("_", middleCount);

        // 將所有底線串連起來
        string middleWithSpaces = string.Join(" ", underscores);

        // 輸出組合後結果
        return $"{firstLetter} {middleWithSpaces} {lastLetter}";
    }

    private void SaveCurrentAnswer()
    {
        if (_currentWord != null)
        {
            _currentWord.UserAnswer = UserAnswer;
        }
    }

    private void PreviousButton_Clicked(object sender, EventArgs e)
    {
        if (!IsQuizInProgress || _currentQuestionIndex <= 0) return;
        SaveCurrentAnswer();
        _currentQuestionIndex--;
        GenerateNewQuestion();
    }

    private void NextButton_Clicked(object sender, EventArgs e)
    {
        if (!IsQuizInProgress || _currentQuestionIndex >= _sessionWords.Count - 1) return;
        SaveCurrentAnswer();
        _currentQuestionIndex++;
        GenerateNewQuestion();
    }

    private async void FinishQuizButton_Clicked(object sender, EventArgs e)
    {
        SaveCurrentAnswer();
        int correctCount = 0;
        foreach (var word in _sessionWords)
        {
            word.IsCorrect = string.Equals(word.UserAnswer, word.Word, StringComparison.OrdinalIgnoreCase);
            if (word.IsCorrect)
            {
                correctCount++;
            }else
            {
                word.MistakeCount++;

                // 更新資料庫
                if (_databaseService != null)
                {
                    await _databaseService.UpdateVocabularyAsync(word);
                }

                // 將錯題本身加入錯題本
                if (_databaseService != null)
                {
                    await _databaseService.AddMistakeAsync(word);
                }
            }
        }

        double accuracy = (double)correctCount / TotalQuestions * 100;
        FinalScoreText = $"正確率: {accuracy:F1}% ({correctCount} / {TotalQuestions})";
        PageTitle = "測驗結果";

        _currentState = QuizState.Reviewing;
        OnPropertyChanged(nameof(IsAnswering));
        OnPropertyChanged(nameof(IsReviewing));

        _currentState = QuizState.Reviewing;
        OnPropertyChanged(nameof(IsAnswering));
        OnPropertyChanged(nameof(IsReviewing));

        OnPropertyChanged(nameof(SessionWords));

    }

    private async void SpeakButton_Clicked(object sender, EventArgs e)
    {
        await TapHandlers.SpeakWordAsync(_currentWord?.Word);
    }

    private async void GoBackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//IndexPage");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 為字串中的每個字元之間加入空格
    /// </summary>
    /// <param name="text">原始字串</param>
    /// <returns>帶有空格的新字串</returns>
    private string AddSpacing(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // 將字串轉為字元陣列，然後用一個空格將它們重新組合
        return string.Join(" ", text.ToCharArray());
    }

    public enum QuizState { Answering, Reviewing }
    private QuizState _currentState = QuizState.Answering;
    public bool IsAnswering => _currentState == QuizState.Answering;
    public bool IsReviewing => _currentState == QuizState.Reviewing;
}