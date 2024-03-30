using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public class ScaleTransition : ObjectTransition
    {
        public ScaleTransition(int time, float delta, TransitionOptions options, EasingType easing, EasingDirection direction) : base(time, options, easing, direction)
        {
            Delta = delta;
        }

        /// <summary>
        /// The target change in position upon completion.
        /// </summary>
        public float Delta { get; set; }

        /// <inheritdoc/>
        public override void ApplyDelta(float timeDelta)
            => Parent.Transform.Scale += Delta * timeDelta;

        /// <inheritdoc/>
        public override void CorrectDelta()
        {
            float scale = Parent.Transform.Scale;

            if ((scale - (int)scale) < SNAPPING_TOLERANCE)
                scale = (int)scale;
            else if ((scale - (int)scale) > (1f - SNAPPING_TOLERANCE))
                scale = (int)scale + 1f;

            Parent.Transform.Scale = scale;
        }

        /// <summary>
        /// Constructs a <see cref="ScaleTransition"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ScaleTransition FromXml(XElement element)
        {
            int.TryParse(element.Attribute("Time").Value, out int time);
            float.TryParse(element.Attribute("Delta").Value, out float delta);
            int.TryParse(element.Attribute("Options").Value, out int options);
            Enum.TryParse(element.Attribute("Easing").Value, out EasingType type);
            Enum.TryParse(element.Attribute("Direction").Value, out EasingDirection direction);

            return new ScaleTransition(time, delta, (TransitionOptions)options, type, direction);
        }
    }
}