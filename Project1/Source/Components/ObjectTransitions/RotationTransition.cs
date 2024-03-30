using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public class RotationTransition : ObjectTransition
    {
        public RotationTransition(int time, float delta, TransitionOptions options, EasingType easing, EasingDirection direction) : base(time, options, easing, direction)
        {
            Delta = delta;
        }

        /// <summary>
        /// The target change in position upon completion.
        /// </summary>
        public float Delta { get; set; }

        /// <inheritdoc/>
        public override void ApplyDelta(float timeDelta)
            => Parent.Transform.Rotation += Delta * timeDelta;

        /// <inheritdoc/>
        public override void CorrectDelta()
        {
            float rotation = Parent.Transform.Rotation;

            if ((rotation - (int)rotation) < SNAPPING_TOLERANCE)
                rotation = (int)rotation;
            else if ((rotation - (int)rotation) > (1f - SNAPPING_TOLERANCE))
                rotation = (int)rotation + 1f;

            Parent.Transform.Rotation = rotation;
        }

        /// <summary>
        /// Constructs a <see cref="RotationTransition"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static RotationTransition FromXml(XElement element)
        {
            int.TryParse(element.Attribute("Time").Value, out int time);
            float.TryParse(element.Attribute("Delta").Value, out float delta);
            int.TryParse(element.Attribute("Options").Value, out int options);
            Enum.TryParse(element.Attribute("Easing").Value, out EasingType type);
            Enum.TryParse(element.Attribute("Direction").Value, out EasingDirection direction);

            return new RotationTransition(time, delta, (TransitionOptions)options, type, direction);
        }
    }
}