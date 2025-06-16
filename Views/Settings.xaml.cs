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
        // Ū�����q�]�w
        VolumeSlider.Value = AppSettings.Volume;
        VolumeLabel.Text = $"���q: {AppSettings.Volume:P0}";

        // Ū�����ճ]�w
        PitchSlider.Value = AppSettings.Pitch;
        PitchLabel.Text = $"����: {AppSettings.Pitch:F1}";

        // Ū���f���]�w
        AccentPicker.SelectedItem = AppSettings.Accent;
    }

    private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        float newValue = (float)e.NewValue;
        AppSettings.Volume = newValue;
        VolumeLabel.Text = $"���q: {newValue:P0}";
    }

    private void PitchSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        float newValue = (float)e.NewValue;
        AppSettings.Pitch = newValue;
        PitchLabel.Text = $"����: {newValue:F1}";
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