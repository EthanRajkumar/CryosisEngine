using System;

namespace CryosisEngine
{
    public enum EasingType
    {
        Linear = 0,
        Quadratic = 1,
        Cubic = 2,
        Quartic = 3,
        Quintic = 4,
        Sinusoidal = 5,
        Exponential = 6
    }

    public enum EasingDirection
    {
        In,
        Out,
        InOut
    }

    public static class Easing
    {
        public static float ApplyEasingFunction(float proportion, EasingType easingType, EasingDirection easingDirection)
        {
            switch (easingType)
            {
                default:
                case EasingType.Linear:
                    return proportion;

                case EasingType.Quadratic:
                    return GetQuadraticEasing(proportion, easingDirection);

                case EasingType.Cubic:
                    return GetCubicEasing(proportion, easingDirection);

                case EasingType.Quartic:
                    return GetQuarticEasing(proportion, easingDirection);

                case EasingType.Quintic:
                    return GetQuinticEasing(proportion, easingDirection);

                case EasingType.Exponential:
                    return GetExponentialEasing(proportion, easingDirection);

                case EasingType.Sinusoidal:
                    return GetSinusoidalEasing(proportion, easingDirection);
            }
        }

        public static float GetQuadraticEasing(float proportion, EasingDirection easingDirection)
        {
            float compliment = proportion;

            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportion * proportion;

                case EasingDirection.Out:
                    return 1f - (compliment * compliment);

                case EasingDirection.InOut:
                    {
                        if (proportion > 0.5f)
                            return 2f * proportion * proportion;
                        else
                            return 1f - (2f * compliment * compliment);
                    }
            }
        }

        public static float GetCubicEasing(float proportion, EasingDirection easingDirection)
        {
            float compliment = 1f - proportion;

            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportion * proportion * proportion;

                case EasingDirection.Out:
                    return 1f - (compliment * compliment * compliment);

                case EasingDirection.InOut:
                    {
                        if (proportion > 0.5f)
                            return 4f * proportion * proportion * proportion;
                        else
                            return 1f + (4f * compliment * compliment * compliment);
                    }
            }
        }

        public static float GetQuarticEasing(float proportion, EasingDirection easingDirection)
        {
            proportion *= proportion;

            float complimentSquared = 1f - proportion;
            complimentSquared *= complimentSquared;

            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportion * proportion;

                case EasingDirection.Out:
                    return 1f - (complimentSquared * complimentSquared);

                case EasingDirection.InOut:
                    {
                        if (proportion > 0.5f)
                            return 8f * proportion * proportion;
                        else
                            return 1f - (8f * complimentSquared * complimentSquared);
                    }
            }
        }

        public static float GetQuinticEasing(float proportion, EasingDirection easingDirection)
        {
            float complimentSquared = 1f - proportion;
            float proportionSquared = proportion * proportion;
            complimentSquared *= complimentSquared;

            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportionSquared * proportionSquared * proportion;

                case EasingDirection.Out:
                    return 1f - (complimentSquared * complimentSquared * (1f - proportion));

                case EasingDirection.InOut:
                    {
                        if (proportion > 0.5f)
                            return 16f * proportionSquared * proportionSquared * proportion;
                        else
                            return 1f + (16f * complimentSquared * complimentSquared * (1f - proportion));
                    }
            }
        }

        public static float GetExponentialEasing(float proportion, EasingDirection easingDirection)
        {
            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportion == 0f ? 0f : (float)Math.Pow(2f, (10f * proportion) - 10f);

                case EasingDirection.Out:
                    return proportion == 0f ? 0f : (float)Math.Pow(2f, (-10f * proportion)) + 1f;

                case EasingDirection.InOut:
                    {
                        if (proportion > 0.5f)
                            return (float)Math.Pow(2, (20f * proportion) - 12f);
                        else
                            return (float)Math.Pow(2, -20f * (proportion - 5f));
                    }
            }
        }

        public static float GetSinusoidalEasing(float proportion, EasingDirection easingDirection)
        {
            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return 1f - (float)Math.Cos(Math.PI * proportion * 0.5f);

                case EasingDirection.Out:
                    return (float)Math.Sin(proportion * Math.PI * 0.5f);

                case EasingDirection.InOut:
                    return -(float)(Math.Cos(Math.PI * proportion) - 1f) / 2f;
            }
        }
    }
}
