using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Screen
{
	public static class PlatformUtils
    {
		static int screenResolutionX = 0;
		static int screenResolutionY = 0;
        static float pixelsPerCm = 0f;

		static void GetScreenResolution() {
			//See also Display.main.systemWidth, Display.main.systemHeight?
			// Note: in editor, this seems to always be 640x480
			var resolutions = UnityEngine.Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++) {
				var resolution = resolutions[i];
				if (resolution.width > screenResolutionX) {
					if (resolution.height > screenResolutionY) {
						screenResolutionX = resolution.width;
						screenResolutionY = resolution.height;
					}
				}
			}
			// This actually happens (e.g. on Android):
			if (screenResolutionX == 0) screenResolutionX = UnityEngine.Screen.width;
			if (screenResolutionY == 0) screenResolutionY = UnityEngine.Screen.height;
		}
		public static int ScreenResolutionX {
			get {
				if (screenResolutionX == 0) GetScreenResolution();
				return screenResolutionX;
			}
		}
		public static int ScreenResolutionY {
			get {
				if (screenResolutionY == 0) GetScreenResolution();
				return screenResolutionY;
			}
		}

        public static float PixelsPerCm {
			get {
				if (pixelsPerCm > 0) return pixelsPerCm;

				float dpi = UnityEngine.Screen.dpi;

				if (dpi <= 0) {
					int width = ScreenResolutionX;
					int height = ScreenResolutionY;

					// take some guess
					switch (Application.platform) {
					case (RuntimePlatform.OSXEditor):
					case (RuntimePlatform.OSXPlayer):
						// http://en.wikipedia.org/wiki/Dots_per_inch
						dpi = 72f;
						break;
					case (RuntimePlatform.WindowsEditor):
					case (RuntimePlatform.WindowsPlayer):
						// http://en.wikipedia.org/wiki/Dots_per_inch
						dpi = 96f;
						break;
					case (RuntimePlatform.WSAPlayerARM):
					case (RuntimePlatform.WSAPlayerX64):
					case (RuntimePlatform.WSAPlayerX86):
						// Use the same value as for Windows?
						dpi = 96f;
						break;
					case (RuntimePlatform.LinuxPlayer):
						dpi = 96f; // ? see http://scanline.ca/dpi/
						break;
					case (RuntimePlatform.Android):
						// http://developer.android.com/guide/practices/screens_support.html
						// See also: http://forum.unity3d.com/threads/203017-Finding-*physical*-screen-size
						//dpi = 160f; // default "medium-size" dpi
						//float xdpi = DisplayMetricsAndroid.XDPI;
						//float ydpi = DisplayMetricsAndroid.YDPI;
						//if ((xdpi > 0) && (ydpi > 0)) dpi = Mathf.Sqrt(xdpi*xdpi + ydpi*ydpi);
						//dpi = 160f * DisplayMetricsAndroid.Density;
						dpi = DisplayMetricsAndroid.DensityDPI;
						if (dpi <= 0) {
							int size_min = Mathf.Min(width, height);
							int size_max = Mathf.Max(width, height);
							if ((size_max >= 960) && (size_min >= 720)) {
								dpi = 320f; // xlarge screens, xhdpi
							} else if ((size_max >= 640) && (size_min >= 480)) {
								dpi = 240f; // large screens, hdpi
							} else if ((size_max >= 470) && (size_min >= 320)) {
								dpi = 160f; // normal screens, mdpi
							} else { // expected to be at least 426 x 320
								dpi = 120f; // small screens, ldpi
							}
						}
						break;
					case (RuntimePlatform.IPhonePlayer):
						// http://stackoverflow.com/questions/1365112/what-dpi-resolution-is-used-for-an-iphone-app
						if (width == 640) {
							dpi = 326f;
						} else if (width == 1024) {
							dpi = 132f; // 163 for iPad mini
						} else if (width == 2048) {
							dpi = 264f; // 326 for iPad mini (retina)
						} else {
							dpi = 163f;
						}
						break;
					default:
						// Use the same value as for Windows?
						dpi = 96f;
						break;
					}
				}

				// DPI points fit in 1 inch (2.54 cm)
				pixelsPerCm = dpi / 2.54f; // inch -> cm

				return pixelsPerCm;
			}
		}
	}

	// Adapted from http://answers.unity3d.com/questions/161281/is-there-a-way-to-android-physical-screen-size.html
	public static class DisplayMetricsAndroid {
		// The logical density of the display
		public static float Density { get; private set; }

		// The screen density expressed as dots-per-inch
		public static int DensityDPI { get; private set; }

		// The absolute height of the display in pixels
		public static int HeightPixels { get; private set; }

		// The absolute width of the display in pixels
		public static int WidthPixels { get; private set; }

		// A scaling factor for fonts displayed on the display
		public static float ScaledDensity { get; private set; }

		// The exact physical pixels per inch of the screen in the X dimension
		public static float XDPI { get; private set; }

		// The exact physical pixels per inch of the screen in the Y dimension
		public static float YDPI { get; private set; }

		static DisplayMetricsAndroid() {
			// the following code is not compiled by iOS platform
			#if UNITY_ANDROID
			// Early out if we're not on an Android device
			if (Application.platform != RuntimePlatform.Android) return;

			// The following is equivalent to this Java code:
			//
			// metricsInstance = new DisplayMetrics();
			// UnityPlayer.currentActivity.getWindowManager().getDefaultDisplay().getMetrics(metricsInstance);
			//
			// ... which is pretty much equivalent to the code on this page:
			// http://developer.android.com/reference/android/util/DisplayMetrics.html

			using (
				AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
				metricsClass = new AndroidJavaClass("android.util.DisplayMetrics")
				) {
				using (
					AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
					activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
					windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
					displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
					) {
					displayInstance.Call("getMetrics", metricsInstance);
					Density = metricsInstance.Get<float>("density");
					DensityDPI = metricsInstance.Get<int>("densityDpi");
					HeightPixels = metricsInstance.Get<int>("heightPixels");
					WidthPixels = metricsInstance.Get<int>("widthPixels");
					ScaledDensity = metricsInstance.Get<float>("scaledDensity");
					XDPI = metricsInstance.Get<float>("xdpi");
					YDPI = metricsInstance.Get<float>("ydpi");
				}
			}
			#endif
		}
	}

}