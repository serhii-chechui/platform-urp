using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Spline
{
    public class SplineInterpolator
    {
        public SplineType SplineType;
        private float bias;
        private float positiveWeight;
        private float negativeWeight;
        private float tension;

        public SplineInterpolator(SplineType splineType)
        {
            SplineType = splineType;
            CalculateWeights();
        }

        public float Tension
        {
            get => tension;
            set
            {
                tension = value;
                CalculateWeights();
            }
        }

        public float Bias
        {
            get => bias;
            set
            {
                bias = value;
                CalculateWeights();
            }
        }

        public Vector4 this[float time, float tension, float bias]
        {
            get
            {
                Tension = tension;
                Bias = bias;
                return this[time];
            }
        }

        public Vector4 this[float time, float tension]
        {
            get
            {
                Tension = tension;
                return this[time];
            }
        }

        public Vector4 this[float time]
        {
            get
            {
                float t2, t3;

                switch (SplineType)
                {
                    case SplineType.Cubic:
                        t2 = time * time;
                        t3 = time * t2;
                        var t23 = t2 - t3;

                        return new Vector4(
                            -time + t2 + t23,
                            1f - t2 - t23,
                            time + t23,
                            -t23);
                    case SplineType.Catmull:
                        t2 = time * time;
                        t3 = time * t2;
                        var t05 = 0.5f * time;
                        var t205 = 0.5f * t2;
                        var t220 = t2 + t2;
                        var t305 = 0.5f * t3;
                        var t315 = t3 + t305;

                        return new Vector4(
                            -t305 + t2 - t05,
                            t315 - (t220 + t205) + 1f,
                            -t315 + t220 + t05,
                            t305 - t205);
                    case SplineType.Hermite:
                        t2 = time * time;
                        t3 = time * t2;
                        var t3_t2 = t3 - t2;
                        var t32_t23 = t3_t2 + t3_t2 - t2;
                        var a0 = t32_t23 + 1f;
                        var a1 = t3_t2 - t2 + time;
                        var a2 = t3_t2;
                        var a3 = -t32_t23;
                        var a1p = a1 * positiveWeight;
                        var a1n = a1 * negativeWeight;
                        var a2p = a2 * positiveWeight;
                        var a2n = a2 * negativeWeight;

                        return new Vector4(
                            -a1p,
                            a0 + a1p - a1n - a2p,
                            a1n + a2p - a2n + a3,
                            a2n);
                    default:
                        return new Vector4(0f, 1f - time, time, 0f);
                }
            }
        }

        private void CalculateWeights()
        {
            var tensionWrk = (1f - tension) * 0.5f;
            positiveWeight = (1f + bias) * tensionWrk;
            negativeWeight = (1f - bias) * tensionWrk;
        }
    }
}