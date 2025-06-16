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
        // �N JSON ��Ƽg�J��Ʈw
        await _databaseService.SeedDataFromFileAsync();
        // ���J��ƨç�s�e��
        await LoadVocabulariesAsync();
    }

    private async Task LoadVocabulariesAsync()
    {
        // ��Ʈw���o��r�C��
        var itemsFromDb = await _databaseService.GetVocabulariesAsync();
        // await DisplayAlert("����", $"�q��ƮwŪ���� {itemsFromDb.Count} �ӳ�r�C", "OK");
        Vocabularies.Clear(); // ���M�šA�קK����

        // �NŪ���쪺��r�v�@�[�J�� UI ���X��
        if (itemsFromDb != null)
        {
            foreach (var item in itemsFromDb)
            {
                Vocabularies.Add(item);
            }
        }
    }

    // �B�z��ֳt�I���ɡA��@���n���|���_�e�@��
    private CancellationTokenSource _speechCancellationTokenSource;

    /// <summary>
    /// ��ϥΪ��I����r�d����Ĳ�o
    /// </summary>
    private async void OnWordTapped(object sender, TappedEventArgs e)
    {
        TapHandlers.HandleWordTapped(sender, e);
    }



}