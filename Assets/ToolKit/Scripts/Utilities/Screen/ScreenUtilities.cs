using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Screen
{
    public static class ScreenUtilities
    {
        public const int CountTypeAspect = 4;

        private static float baseAspect = -1.0f;

        public static float BaseAspect
        {
            get
            {
                if (baseAspect <= 0.0f)
                {
                    baseAspect = ((float)UnityEngine.Screen.width / UnityEngine.Screen.height > 1.0f)
                        ? (4.0f / 3.0f)
                        : (3.0f / 4.0f);
                }

                return baseAspect;
            }
        }

        private static float baseOrthographicSizeToUI = -1.0f;

        public static float BaseOrthographicSizeToUI
        {
            get
            {
                if (baseOrthographicSizeToUI <= 0.0f)
                {
                    baseOrthographicSizeToUI = ((float)UnityEngine.Screen.width / UnityEngine.Screen.height > 1.0f)
                        ? 768.0f
                        : 1024.0f;
                }

                return baseOrthographicSizeToUI;
            }
        }

        public static void ResetAspect()
        {
            baseAspect = -1.0f;
            baseOrthographicSizeToUI = -1.0f;
        }

        public static MageAppAspectRatio GetAspectRatio(Camera camera = null)
        {
            var aspect = (camera != null) ? camera.aspect : ((float)UnityEngine.Screen.width / UnityEngine.Screen.height);

            if (aspect > 1.0f)//landscape
            {
                if (aspect > 1.9f)
                    return MageAppAspectRatio.Aspect195by90;

                if (aspect >= 1.7f)
                    return MageAppAspectRatio.Aspect16by9;

                if (aspect >= 1.59f)
                    return MageAppAspectRatio.Aspect16by10;

                if (aspect >= 1.49f)
                    return MageAppAspectRatio.Aspect3by2;

                if (aspect >= 1.28f)
                    return MageAppAspectRatio.Aspect4by3;

                if (aspect >= 1.20f)
                    return MageAppAspectRatio.Aspect5by4;

            }
            else //portrait
            {
                if (aspect >= 0.79f)
                    return MageAppAspectRatio.Aspect5by4;

                if (aspect >= 0.74f)
                    return MageAppAspectRatio.Aspect4by3;

                if (aspect >= 0.65f)
                    return MageAppAspectRatio.Aspect3by2;

                if (aspect >= 0.61f)
                    return MageAppAspectRatio.Aspect16by10;

                if (aspect >= 0.545f)
                    return MageAppAspectRatio.Aspect16by9;
            }

            return MageAppAspectRatio.AspectOthers;
        }

        public static int GetAspectRatioToIndex(Camera camera = null)
        {
            var aspectRatio = GetAspectRatio(camera);
            var underlyingEnumValue = (int)aspectRatio;

            if (underlyingEnumValue >= 0 && underlyingEnumValue < CountTypeAspect)
                return underlyingEnumValue;

            if (Debug.isDebugBuild)
                Debug.LogError($"This aspect ratio is not supported: {aspectRatio} (index = {underlyingEnumValue}).");

            return 0;
        }

        public static float ScaleUnitToPoint(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;

            return camera.orthographicSize / BaseOrthographicSizeToUI;
        }

        public static float ScaleUnitToPointForLandscapeLargerThan4By3(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;

            var scale = camera.orthographicSize / BaseOrthographicSizeToUI;

            if ((camera.aspect > 1.0f) && (GetAspectRatio(camera) > MageAppAspectRatio.Aspect4by3))
                scale = scale * 9f / 8f;// (3:2/4:3)

            return scale;
        }

        public static float CalculateCameraSizeToUIView(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;

            var baseUICameraSize = BaseOrthographicSizeToUI;// 768 or 1024

            switch (GetAspectRatio(camera))
            {
                case MageAppAspectRatio.Aspect5by4:
                    {
                        var baseAsp = (camera.aspect < 1.0f) ? 0.75f : 1.33333f;
                        baseUICameraSize = (baseAsp / camera.aspect) * baseUICameraSize;
                        break;
                    }
                case MageAppAspectRatio.Aspect3by2:
                case MageAppAspectRatio.Aspect16by10:
                case MageAppAspectRatio.Aspect16by9:
                    {
                        baseUICameraSize = (camera.aspect < 1.0f) ? 1152.0f : 682.0f;
                        break;
                    }
                case MageAppAspectRatio.Aspect195by90:
                    {
                        baseUICameraSize = 900f;
                        break;
                    }
                case MageAppAspectRatio.AspectOthers:
                    {
                        if (camera.aspect < 0.55f)
                            baseUICameraSize = (0.5625f / camera.aspect) * baseUICameraSize;
                        else if (camera.aspect < 1.25f)
                            baseUICameraSize = (1.33333f / camera.aspect) * baseUICameraSize;

                        break;
                    }
            }

            return baseUICameraSize;
        }

        public static Vector3 CalculatePositionToUIView(Camera camera = null, float planeZ = 10.0f)
        {
            if (camera == null)
                camera = Camera.main;

            var position = camera.transform.position;
            position.y -= (BaseOrthographicSizeToUI - camera.orthographicSize);
            position.z += planeZ;
            return position;
        }

        public static Vector3 CalculatePositionToUIView(Vector3 basePosition, Camera camera = null, float planeZ = 10.0f)
        {
            if (camera == null)
                camera = Camera.main;

            var offsetY = BaseOrthographicSizeToUI - camera.orthographicSize;
            return new Vector3(basePosition.x, basePosition.y - offsetY, basePosition.z + planeZ);
        }

        public static void SetObjectPlane(GameObject plane, bool beforeCamera = true, Camera camera = null,
            float planeZ = 0.5f)
        {
            if (camera == null)
                camera = Camera.main;

            var position = plane.transform.position;

            if (beforeCamera)
                position.z = camera.transform.position.z + planeZ;
            else
                position.z += 2.0f * camera.farClipPlane;

            plane.transform.position = position;
        }
    }
}