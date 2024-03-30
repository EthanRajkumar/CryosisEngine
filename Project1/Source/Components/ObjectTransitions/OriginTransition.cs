using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public class OriginTransition : ObjectTransition
    {
        public OriginTransition(int time, Vector2 delta, TransitionOptions options, EasingType easing, EasingDirection direction) : base(time, options, easing, direction)
        {
            Delta = delta;
        }

        /// <summary>
        /// The target change in origin upon completion.
        /// </summary>
        public Vector2 Delta { get; set; }

        /// <inheritdoc/>
        public override void ApplyDelta(float timeDelta)
            => Parent.Transform.Origin += Delta * timeDelta;

        /// <inheritdoc/>
        public override void CorrectDelta()
        {
            Vector2 origin = Parent.Transform.Origin;

            if ((origin.X - (int)origin.X) < SNAPPING_TOLERANCE)
                origin.X = (int)origin.X;
            else if ((origin.X - (int)origin.X) > (1f - SNAPPING_TOLERANCE))
                origin.X = (int)origin.X + 1f;

            if ((origin.Y - (int)origin.Y) < SNAPPING_TOLERANCE)
                origin.Y = (int)origin.Y;
            else if ((origin.Y - (int)origin.Y) > (1f - SNAPPING_TOLERANCE))
                origin.Y = (int)origin.Y + 1f;

            Parent.Transform.Origin = origin;
        }

        /// <summary>
        /// Constructs a <see cref="OriginTransition"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static OriginTransition FromXml(XElement element)
        {
            int.TryParse(element.Attribute("Time").Value, out int time);
            Vector2 delta = XmlUtils.Vec2FromXml(element.Element("Delta"));
            int.TryParse(element.Attribute("Options").Value, out int options);
            Enum.TryParse(element.Attribute("Easing").Value, out EasingType type);
            Enum.TryParse(element.Attribute("Direction").Value, out EasingDirection direction);

            return new OriginTransition(time, delta, (TransitionOptions)options, type, direction);
        }
    }
}