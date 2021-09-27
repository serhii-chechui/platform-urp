using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.DeviceInfos
{
    internal class SystemUtilitiesProxyEditor: ISystemUtilitiesProxy
    {
        /// <summary>
        /// ДОСТУПНОЕ СВОБОДНОЕ МЕСТО НА УСТРОЙСТВЕ
        /// </summary>
        public long FreeDiskSpace
        {
            get
            {
                long freeSpace = 100 * 1024 * 1024; //STUB 100MB;

#if MAGE_DEBUG
            Debug.Log("SysUtilsProxy.freeDiskSpace: ".WithTime() + freeSpace);
#endif
                return freeSpace;
            }
        }

        /// <summary>
        /// проверка установлено ли приложение
        /// </summary>
        /// <param name="urlApp"></param>
        /// <returns></returns>
        public bool AppIsInstalled(string urlApp)
        {
            return false;
        }

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

        /// <summary>
        /// API Level (SDK version )
        /// </summary>
        public int ApiLevel => 0;
        
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
#if MAGE_TV_DEBUG
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
            return languageTags[0];
        }
    }
}