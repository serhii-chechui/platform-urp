using System;
using System.Linq;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.DeviceInfos
{
    /// <summary>
    /// Native methods call from UnityScript
    /// </summary>
    public static class SystemUtilitiesProxy
    {
        private static readonly ISystemUtilitiesProxy utilitiesProxy;

        static SystemUtilitiesProxy()
        {
#if UNITY_EDITOR
            utilitiesProxy = new SystemUtilitiesProxyEditor();
#elif UNITY_IOS || UNITY_TVOS
            utilitiesProxy = new SystemUtilitiesProxyIos();
#elif UNITY_ANDROID
            utilitiesProxy = new SystemUtilitiesProxyAndroid();
#endif
        }
        
        /// <summary>
        /// ДОСТУПНОЕ СВОБОДНОЕ МЕСТО НА УСТРОЙСТВЕ
        /// </summary>
        public static long FreeDiskSpace => utilitiesProxy.FreeDiskSpace;
        
        /// <summary>
        /// проверка установлено ли приложение
        /// </summary>
        /// <param name="urlApp"></param>
        /// <returns></returns>
        public static bool AppIsInstalled(string urlApp) => utilitiesProxy.AppIsInstalled(urlApp);

        /// <summary>
        /// запуск приложения
        /// </summary>
        /// <param name="urlApp"></param>
        /// <param name="checkIsInstalled"></param>
        public static void OpenApp(string urlApp, bool checkIsInstalled = true) =>
            utilitiesProxy.OpenApp(urlApp, checkIsInstalled);

        /// <summary>
        /// API Level (SDK version )
        /// </summary>
        public static int ApiLevel => utilitiesProxy.ApiLevel;

        private static float? _screenSize = null;

        /// <summary>
        /// Screen Size inch
        /// </summary>
        public static float ScreenSize
        {
            get
            {
                if (_screenSize.HasValue)
                {
                    return _screenSize.Value;
                }

                var w = (float) UnityEngine.Screen.width;
                var h = (float) UnityEngine.Screen.height;
                _screenSize = Mathf.Sqrt((w * w) + (h * h)) / Dpi;
                return _screenSize.Value;
            }
        }

        /// <summary>
        /// It is physical pixels per inch of the screen
        /// </summary>
        public static float Dpi => utilitiesProxy.Dpi;

        /// <summary>
        /// detect TV mode
        /// </summary>
        public static bool IsTV => utilitiesProxy.IsTV;
        
        /// <summary>
        /// Matches language with options in the given array.
        /// </summary>
        /// <param name="chosenLanguage">BCP47 language tag.</param>
        /// <param name="supportedLanguages">Array of supported BCP47 language tags. First array item is used
        /// as fallback option.</param>
        /// <returns>One of the supported language tags.</returns>
        public static string ApplyLanguageFallbackLogic(string chosenLanguage, string[] supportedLanguages)
        {
            var defaultLanguage = supportedLanguages[0];

            if (string.IsNullOrEmpty(chosenLanguage))
            {
                return defaultLanguage;
            }

            if (supportedLanguages.Contains(chosenLanguage))
            {
                return chosenLanguage;
            }

            var designators = chosenLanguage.Split('-'); // language-region, or language-script-region
            var baseLanguage = designators[0];

            // STEP 1 - Chinese dialects.
            // Chinese Traditional differs from Chinese Simplified, fallback is a complicated matter.
            if (baseLanguage == "zh")
            {
                if (designators.Length != 3)
                {
                    return defaultLanguage;
                }

                switch (designators[1])
                {
                    case "Hans":
                        return "zh-CN";
                    case "Hant":
                        return "zh-TW";
                    default:
                        return defaultLanguage;
                }
            }

            // STEP 2 - Stripping to base language
            if (supportedLanguages.Contains(baseLanguage))
            {
                return baseLanguage;
            }

            // STEP 3 - Enumerating supported dialects in alphabet order (Android does this)
            var dialects = supportedLanguages.Where(x =>
                x.StartsWith(baseLanguage, StringComparison.OrdinalIgnoreCase) && x != baseLanguage).ToList();

            switch (dialects.Count)
            {
                case 0:
                    return defaultLanguage;
                case 1:
                    return dialects[0];
                default:
                    dialects.Sort();
                    return dialects[0];
            }
        }

        /// <summary>
        /// Chooses user preferred localization based on options in the given array.
        /// Results may differ on devices with different OS versions.
        /// </summary>
        /// <param name="languageTags">BCP47 language tags to match device localizations with.
        /// First array item is used as fallback option.</param>
        /// <returns>BCP47 language tag. May not be included in the input array.</returns>
        public static string GetNativePreferredAppLanguage(string[] languageTags) =>
            utilitiesProxy.GetNativePreferredAppLanguage(languageTags);
    }
}