using System;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public static class CryosisMath
    {
        /// <summary>
        /// Rotates a <see cref="Vector2"/> about it's origin by an angle in radians.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 vector, float theta)
        {
            float cos = (float)Math.Cos(theta), sin = (float)Math.Sin(theta);

            return new Vector2((vector.X * cos) - (vector.Y * sin), (vector.X * sin) + (vector.Y * cos));
        }

        /// <summary>
        /// Rotates a <see cref="Vector2"/> about an origin by an angle in radians.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 vector, float theta, Vector2 origin)
        {
            Vector2 difference = vector - origin;

            return Rotate(difference, theta) + (origin - difference);
        }
    }
}
