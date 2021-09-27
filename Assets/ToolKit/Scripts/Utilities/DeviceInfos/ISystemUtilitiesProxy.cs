namespace Bini.ToolKit.Core.Unity.Utilities.DeviceInfos
{
    public interface ISystemUtilitiesProxy
    {
        /// <summary>
        /// ДОСТУПНОЕ СВОБОДНОЕ МЕСТО НА УСТРОЙСТВЕ
        /// </summary>
        long FreeDiskSpace { get; }

        /// <summary>
        /// API Level (SDK version )
        /// </summary>
        int ApiLevel { get; }


        /// <summary>
        /// It is physical pixels per inch of the screen
        /// </summary>
        float Dpi { get; }

        /// <summary>
        /// detect TV mode
        /// </summary>
        bool IsTV { get; }

        /// <summary>
        /// проверка установлено ли приложение
        /// </summary>
        /// <param name="urlApp"></param>
        /// <returns></returns>
        bool AppIsInstalled(string urlApp);

        /// <summary>
        /// запуск приложения
        /// </summary>
        /// <param name="urlApp"></param>
        /// <param name="checkIsInstalled"></param>
        void OpenApp(string urlApp, bool checkIsInstalled = true);

        /// <summary>
        /// Chooses user preferred localization based on options in the given array.
        /// Results may differ on devices with different OS versions.
        /// </summary>
        /// <param name="languageTags">BCP47 language tags to match device localizations with.
        /// First array item is used as fallback option.</param>
        /// <returns>BCP47 language tag. May not be included in the input array.</returns>
        string GetNativePreferredAppLanguage(string[] languageTags);
    }
}