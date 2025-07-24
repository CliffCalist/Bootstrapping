using System;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public static class BootSettingsProvider
    {
        private static BootSettings s_settings;



        public static IReadOnlyBootSettings Settings
        {
            get
            {
                if (s_settings == null)
                    LoadSettings();

                return s_settings;
            }
        }



        private static void LoadSettings()
        {
            if (s_settings == null)
                s_settings = Resources.Load<BootSettings>(BootSettings.FILE_NAME);

            if (s_settings == null)
                throw new InvalidOperationException($"{nameof(BootSettings)} not found in Resources folder.");
        }
    }
}
