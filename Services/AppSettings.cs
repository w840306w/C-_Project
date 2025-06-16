using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    public static class AppSettings
    {
        // Volume: 音量 (0.0 to 1.0)
        public static float Volume
        {
            // 預設值為 1.0
            get => Preferences.Get(nameof(Volume), 1.0f); 
            set => Preferences.Set(nameof(Volume), value);
        }

        // Pitch: 音調 (0.0 to 2.0)
        public static float Pitch
        {
            // 預設值為 1.0
            get => Preferences.Get(nameof(Pitch), 1.0f); 
            set => Preferences.Set(nameof(Pitch), value);
        }

        // Accent: 口音 (US/GB)
        public static string Accent
        {
            // 預設為美式
            get => Preferences.Get(nameof(Accent), "US");          
            set => Preferences.Set(nameof(Accent), value);
        }
    }


}
