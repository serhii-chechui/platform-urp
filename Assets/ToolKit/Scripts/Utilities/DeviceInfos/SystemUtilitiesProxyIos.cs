#if UNITY_IOS || UNITY_TVOS

using System.Runtime.InteropServices;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.DeviceInfos
{
    internal class SystemUtilitiesProxyIos : ISystemUtilitiesProxy
    {
        [DllImport("__Internal")]
        private static extern long _GetFreeDiskspace();

        [DllImport("__Internal")]
        private static extern bool _ValidateUrlApp(string urlApp);

        [DllImport("__Internal")]
        private static extern string _GetNSBundlePreferredLocalization(string[] languages, int numberOfLanguages);

        /// <summary>
        /// ДОСТУПНОЕ СВОБОДНОЕ МЕСТО НА УСТРОЙСТВЕ
        /// </summary>
        public long FreeDiskSpace => _GetFreeDiskspace();

        /// <summary>
        /// проверка установлено ли приложение
        /// </summary>
        /// <param name="urlApp"></param>
        /// <returns></returns>
        public bool AppIsInstalled(string urlApp) => _ValidateUrlApp(urlApp);

        /// <summary>
        /// запуск приложения
        /// </summary>
        /// <param name="urlApp"></param>
        /// <param name="checkIsInstalled"></param>
        public void OpenApp(string urlApp, bool checkIsInstalled = true)
        {
            if (checkIsInstalled)
            {
                if (AppIsInstalled(urlApp))
                {
                    Application.OpenURL(urlApp);
                }
            }
            else
            {
                Application.OpenURL(urlApp);
            }
        }

        private static int? _apiLevel = null;

        /// <summary>
        /// API Level (SDK version )
        /// </summary>
        public int ApiLevel
        {
            get
            {
                if (_apiLevel.HasValue)
                {
                    return _apiLevel.Value;
                }

                _apiLevel = 0;

                var osMajorVer = 0;
                var osMinorVer = 0;
                var versionString = SystemInfo.operatingSystem;
                versionString = versionString.Replace(versionString.Contains("iPhone OS ") ? "iPhone OS " : "iOS ", "");

                var partVersion = versionString.Split('.');
                if (partVersion.Length > 0)
                {
                    int.TryParse(partVersion[0], out osMajorVer);
                    if (partVersion.Length > 1)
                    {
                        int.TryParse(partVersion[1], out osMinorVer);
                    }
                }

                _apiLevel = osMajorVer * 10 + osMinorVer;

                return _apiLevel.Value;
            }
        }


        private static float? _dpi = null;

        /// <summary>
        /// It is physical pixels per inch of the screen
        /// </summary>
        public float Dpi
        {
            get
            {
                if (_dpi.HasValue)
                {
                    return _dpi.Value;
                }
                // WARNING - NOT FOR INDIA & TV  (maybe have < 160dpi )

                _dpi = UnityEngine.Screen.dpi;
                if (_dpi.Value < 160f)
                {
                    _dpi = 160f;
                }

                return _dpi.Value;
            }
        }

        /// <summary>
        /// detect TV mode
        /// </summary>
        public bool IsTV
        {
            get
            {
#if UNITY_TVOS
                return true;
#endif
                return false;
            }
        }
        
        /// <summary>
        /// Chooses user preferred localization based on options in the given array.
        /// Results may differ on devices with different OS versions.
        /// </summary>
        /// <param name="languageTags">BCP47 language tags to match device localizations with.
        /// First array item is used as fallback option.</param>
        /// <returns>BCP47 language tag. May not be included in the input array.</returns>
        public string GetNativePreferredAppLanguage(string[] languageTags)
        {
            /* iOS returns one of the language tags in the given array.
             * Prior to iOS 9 dialects were not supported, only base languages.
             * If system can not match any of user preferred languages, it will either return
             * en or en-GB (if any is included in given array), or first array element. */

            return _GetNSBundlePreferredLocalization(languageTags, languageTags.Length);
        }
    }
}

#endif