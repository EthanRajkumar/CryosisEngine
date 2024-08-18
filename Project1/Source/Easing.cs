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

    public class Easing
    {
        public EasingType Function { get; set; }

        public EasingDirection Direction { get; set; }

        public Easing(EasingType function, EasingDirection direction)
        {
            Function = function;
            Direction = direction;
        }

        public float ApplyEasingFunction(float proportion)
        {
            switch (Function)
            {
                default:
                case EasingType.Linear:
                    return proportion;

                case EasingType.Quadratic:
                    return GetQuadraticEasing(proportion, Direction);

                case EasingType.Cubic:
                    return GetCubicEasing(proportion, Direction);

                case EasingType.Quartic:
                    return GetQuarticEasing(proportion, Direction);

                case EasingType.Quintic:
                    return GetQuinticEasing(proportion, Direction);

                case EasingType.Exponential:
                    return GetExponentialEasing(proportion, Direction);

                case EasingType.Sinusoidal:
                    return GetSinusoidalEasing(proportion, Direction);
            }
        }

        public float GetQuadraticEasing(float proportion, EasingDirection easingDirection)
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

        public float GetCubicEasing(float proportion, EasingDirection easingDirection)
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

        public float GetQuarticEasing(float proportion, EasingDirection easingDirection)
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

        public float GetExponentialEasing(float proportion, EasingDirection easingDirection)
        {
            switch (easingDirection)
            {
                default:
                case EasingDirection.In:
                    return proportion == 0f ? 0f : (float)Math.Pow(2f, (10f * proportion) - 10f);

                case EasingDirection.Out:
                    return proportion == 0f ? 0f : 1f - (float)Math.Pow(2f, (-10f * proportion));

                case EasingDirection.InOut:
                    {
                        if (proportion < 0.5f)
                            return (float)Math.Pow(2, (20f * proportion) - 10f) / 2;
                        else
                            return (float)Math.Pow(2, (-20f * proportion) + 10f) / 2;
                    }
            }
        }

        public float GetSinusoidalEasing(float proportion, EasingDirection easingDirection)
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
