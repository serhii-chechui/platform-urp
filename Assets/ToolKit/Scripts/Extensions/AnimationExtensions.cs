using System;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class AnimationExtensions
    {
        /// <summary>
        /// Samples animation to the first frame of the clip.
        /// </summary>
        /// <param name="animation">Animation.</param>
        /// <param name="clipName">Clip name.</param>
        public static void SetFirstFrameOfAnimation(this Animation animation, string clipName) =>
            SampleAnimation(animation, clipName, 0f);

        /// <summary>
        /// Samples animation to the last frame of the clip.
        /// </summary>
        /// <param name="animation">Animation.</param>
        /// <param name="clipName">Clip name.</param>
        public static void SetLastFrameOfAnimation(this Animation animation, string clipName) =>
            SampleAnimation(animation, clipName, animation[clipName].length);

        private static void SampleAnimation(Animation animation, string clipName, float time)
        {
            try
            {
                animation.Play(clipName);
                animation[clipName].time = time;
                animation[clipName].enabled = true;
                animation.Sample();
                animation[clipName].enabled = false;
            }
            catch (Exception exception)
            {
                if (Debug.isDebugBuild)
                    Debug.LogError("SampleAnimation() failed! " + exception + " At " + Time.time);
            }
        }
    }
}