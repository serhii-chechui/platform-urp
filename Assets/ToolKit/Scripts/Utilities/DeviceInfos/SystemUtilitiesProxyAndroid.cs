#if UNITY_ANDROID
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.DeviceInfos
{
    internal class SystemUtilitiesProxyAndroid : ISystemUtilitiesProxy
    {
        /// <summary>
        /// ДОСТУПНОЕ СВОБОДНОЕ МЕСТО НА УСТРОЙСТВЕ
        /// </summary>
        public long FreeDiskSpace
        {
            get
            {
                AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", Application.persistentDataPath);
                var freeSpace = (long) statFs.Call<int>("getBlockSize") * (long) statFs.Call<int>("getAvailableBlocks");

                return freeSpace;
            }
        }

        private static AndroidJavaClass _unityPlayer = null;
        private static AndroidJavaObject _currentActivity = null;
        private static AndroidJavaObject _packageManager = null;

        /// <summary>
        /// инициализация экземпляра PackageManager для вызова других приложений на Android устройствах
        /// </summary>
        /// <returns></returns>
        private bool __InitAndroidPackageManager()
        {
            if (_packageManager != null)
                return true;

            if (_unityPlayer == null)
                _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (_unityPlayer == null)
                return false;

            if (_currentActivity == null)
                _currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (_currentActivity == null)
                return false;

            if (_packageManager == null)
                _packageManager = _currentActivity.Call<AndroidJavaObject>("getPackageManager");
            return (_packageManager != null);
        }

        /// <summary>
        /// проверка установлено ли приложение
        /// </summary>
        /// <param name="urlApp"></param>
        /// <returns></returns>
        public bool AppIsInstalled(string urlApp)
        {
            if (_packageManager == null && !__InitAndroidPackageManager())
                return false;

            try
            {
                using (AndroidJavaObject launchIntent =
                    _packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", urlApp))
                {
                    return launchIntent != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// запуск приложения
        /// </summary>
        /// <param name="urlApp"></param>
        /// <param name="checkIsInstalled"></param>
        public void OpenApp(string urlApp, bool checkIsInstalled = true)
        {
            if (!__InitAndroidPackageManager())
            {
                return;
            }

            try
            {
                //-------------------------------------------
                //FLAG_ACTIVITY_NEW_TASK			0x10000000
                //FLAG_ACTIVITY_CLEAR_TOP			0x04000000
                //FLAG_ACTIVITY_RESET_TASK_IF_NEEDE 0x00200000
                //FLAG_ACTIVITY_CLEAR_TASK			0x00008000
                //-------------------------------------------

                using (AndroidJavaObject launchIntent =
                    _packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", urlApp))
                {
                    try
                    {
                        var newLaunchIntent = launchIntent.Call<AndroidJavaObject>("addFlags", 0x14208000);
                        _currentActivity.Call("startActivity", newLaunchIntent);
                    }
                    catch
                    {
                        _currentActivity.Call("startActivity", launchIntent);
                    }
                }
            }
            catch
            {
                Debug.Log("SysUtilsProxy: OpenApp: can not open app: " + urlApp);
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

                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    _apiLevel = version.GetStatic<int>("SDK_INT");
                }


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

                if (__InitAndroidPackageManager())
                {
                    //... which is pretty much equivalent to the code on this page:
                    //http://developer.android.com/reference/android/util/DisplayMetrics.html
                    using
                    (
                        AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
                        windowManagerInstance = _currentActivity.Call<AndroidJavaObject>("getWindowManager"),
                        displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
                    )
                    {
                        displayInstance.Call("getMetrics", metricsInstance);
                        _dpi = 0.5f * (metricsInstance.Get<float>("xdpi") + metricsInstance.Get<float>("ydpi"));
                        if (_dpi.Value < 1.0f)
                        {
                            _dpi = 160f;
                        }
                    }
                }

                return _dpi.Value;
            }
        }

        private static bool? _isTV = null;

        /// <summary>
        /// detect TV mode
        /// </summary>
        public bool IsTV
        {
            get
            {
                // if cached field has value
                if (_isTV.HasValue)
                    return _isTV.Value;

                // Essentially this code is doing some java stuff to detect if the UI is in TV mode or not
                // What it does is it gets the Android Activity that is running Unity,
                // gets the value of android.content.Context.UI_MODE_SERVICE so we can call getSystemService on
                // the activity, passing in the UI_MODE_SERVICE, which gets us the UiModeManager.  Next we
                // call getCurrentModeType on our UiModeManager instance which gives us some integer that represents the UI mode.
                // We then have to get the value of android.content.res.Configuration.UI_MODE_TYPE_TELEVISION as an integer and then
                // finally we can compare that with our mode type and if they match, it is android tv.
                if (!__InitAndroidPackageManager())
                {
                    _isTV = false;
                    return false;
                }

                AndroidJavaClass contextJavaClass = new AndroidJavaClass("android.content.Context");
                AndroidJavaObject modeServiceConst = contextJavaClass.GetStatic<AndroidJavaObject>("UI_MODE_SERVICE");
                AndroidJavaObject uiModeManager =
                    _currentActivity.Call<AndroidJavaObject>("getSystemService", modeServiceConst);
                int currentModeType = uiModeManager.Call<int>("getCurrentModeType");
                AndroidJavaClass configurationAndroidClass = new AndroidJavaClass("android.content.res.Configuration");
                int modeTypeTelevisionConst = configurationAndroidClass.GetStatic<int>("UI_MODE_TYPE_TELEVISION");
                _isTV = (modeTypeTelevisionConst == currentModeType);

                return _isTV.Value;
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
            /* Android does not return localizations from the given array, it chooses the most fitting
             * localization from user preferences list.
             *
             * When the system cannot find an exact match:
             * - (prior to Android 7) it continues to look for resources by stripping the country code off the locale.
             * Finally, if no match is found, the system falls back to the default.
             * - (after Android 7) it strips country off, then looks for the match with any other language dialects.
             *
             * If none match is registered, we will get first locale in user preferences list.
             *
             * This way system can return localizations that are not supported by the app.
             * We're gonna check base language first, than dialects in alphabet order.*/

            AndroidJavaObject locale = null;

            try
            {
                using (var resources = new AndroidJavaClass("android.content.res.Resources"))
                using (var systemResources = resources.CallStatic<AndroidJavaObject>("getSystem"))
                using (var systemConfiguration = systemResources.Call<AndroidJavaObject>("getConfiguration"))
                {
                    if (
                        ApiLevel <
                        24) // Prior to Android 7.0 (Nougat) only one device language was available in Settings
                    {
                        locale = systemConfiguration.Get<AndroidJavaObject>("locale");
                    }
                    else // Android 7.0 introduced preferred language list
                    {
                        using (var locales = systemConfiguration.Call<AndroidJavaObject>("getLocales"))
                        using (var arrayClass = new AndroidJavaClass("java.lang.reflect.Array"))
                        using (var stringClass = new AndroidJavaClass("java.lang.String"))
                        using (var arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance",
                            stringClass,
                            languageTags.Length))
                        {
                            for (var i = 0; i < languageTags.Length; i++)
                            {
                                arrayClass.CallStatic("set",
                                    arrayObject,
                                    i,
                                    new AndroidJavaObject("java.lang.String", languageTags[i]));
                            }

                            locale = locales.Call<AndroidJavaObject>("getFirstMatch", arrayObject);

                            for (var i = 0; i < languageTags.Length; i++)
                            {
                                var arrayItem = arrayClass.CallStatic<AndroidJavaObject>("get", arrayObject, i);
                                if (arrayItem != null)
                                {
                                    arrayItem.Dispose();
                                }
                            }
                        }
                    }

                    if (locale != null)
                    {
                        var languageTag = locale.Call<string>("getLanguage");

                        if (!string.IsNullOrEmpty(languageTag))
                        {
                            if (ApiLevel >= 21)
                            {
                                var script = locale.Call<string>("getScript");
                                if (!string.IsNullOrEmpty(script))
                                {
                                    languageTag += "-" + script;
                                }
                            }

                            var country = locale.Call<string>("getCountry");
                            if (!string.IsNullOrEmpty(country))
                            {
                                languageTag += "-" + country;
                            }

                            return languageTag;
                        }
                    }
                }
            }
            finally
            {
                if (locale != null)
                {
                    locale.Dispose();
                }
            }

            return languageTags[0];
        }
    }
}
#endif