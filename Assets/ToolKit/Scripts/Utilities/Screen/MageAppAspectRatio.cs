namespace Bini.ToolKit.Core.Unity.Utilities.Screen
{
    /// <summary>
    /// аспект устройства
    /// </summary>
    [System.Serializable]
    public enum MageAppAspectRatio
    {
        /// <summary>
        /// аспект не задан или не определён, часто используется как значение по умолчанию
        /// </summary>
        AspectOthers = -2,

        /// <summary>
        /// Rare android device
        /// </summary>
        Aspect5by4 = -1,

        /// <summary>
        /// iPad Default - all designs made to this is aspect
        /// </summary>
        Aspect4by3 = 0, //

        /// <summary>
        /// iPhone 4/4s
        /// </summary>
        Aspect3by2 = 1,

        /// <summary>
        /// just android devices
        /// </summary>
        Aspect16by10 = 2,

        /// <summary>
        /// iPhone 5+
        /// </summary>
        Aspect16by9 = 3,

        /// <summary>
        /// iPhoneX
        /// </summary>
        Aspect195by90 = 4,
    };
}