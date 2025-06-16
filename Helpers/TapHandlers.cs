using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models; 
using Microsoft.Maui.Media;
using MauiApp1.Services;

namespace MauiApp1.Helpers
{
    public static class TapHandlers
    {

        private static CancellationTokenSource _speechCancellationTokenSource;

        /// <summary>
        /// 這是新的、更通用的核心發音方法，它直接接收一個單字字串。
        /// </summary>
        public static async Task SpeakWordAsync(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            try
            {
                _speechCancellationTokenSource?.Cancel();
                _speechCancellationTokenSource = new CancellationTokenSource();

                var speechOptions = new SpeechOptions()
                {
                    Volume = AppSettings.Volume, // 使用儲存的音量
                    Pitch = AppSettings.Pitch    // 使用儲存的音調
                };

                // 口音 預設為美式 (US)
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                if (AppSettings.Accent == "UK")
                {
                    speechOptions.Locale = locales.FirstOrDefault(l => l.Language == "en" && l.Country == "GB");
                }else 
                {
                    speechOptions.Locale = locales.FirstOrDefault(l => l.Language == "en" && l.Country == "US");
                }

                await TextToSpeech.Default.SpeakAsync(word, speechOptions, _speechCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TTS Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 這是舊的、給 TapGestureRecognizer 用的方法。
        /// 我們修改它，讓它去呼叫新的核心方法。
        /// </summary>
        public static async void HandleWordTapped(object sender, TappedEventArgs e)
        {
            if (sender is BindableObject bindable && bindable.BindingContext is Vocabulary tappedWord)
            {
                // 呼叫新的核心方法
                await SpeakWordAsync(tappedWord.Word);
            }
        }
    }
}
