using CommunityToolkit.Maui.Views;
using MauiApp1.Services; 

namespace MauiApp1.Views;

public partial class Settings : Popup
{
    public Settings()
    {
        InitializeComponent();

        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        // 讀取音量設定
        VolumeSlider.Value = AppSettings.Volume;
        VolumeLabel.Text = $"音量: {AppSettings.Volume:P0}";

        // 讀取音調設定
        PitchSlider.Value = AppSettings.Pitch;
        PitchLabel.Text = $"音調: {AppSettings.Pitch:F1}";

        // 讀取口音設定
        AccentPicker.SelectedItem = AppSettings.Accent;
    }

    private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        float newValue = (float)e.NewValue;
        AppSettings.Volume = newValue;
        VolumeLabel.Text = $"音量: {newValue:P0}";
    }

    private void PitchSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        float newValue = (float)e.NewValue;
        AppSettings.Pitch = newValue;
        PitchLabel.Text = $"音調: {newValue:F1}";
    }
    private void AccentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AccentPicker.SelectedItem is string selectedAccent)
        {
            AppSettings.Accent = selectedAccent;
        }
    }
    private void OnClose_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}