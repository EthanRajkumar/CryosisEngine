using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public class DimensionTransition : ObjectTransition
    {
        public DimensionTransition(int time, Vector2 delta, TransitionOptions options, EasingType easing, EasingDirection direction) : base(time, options, easing, direction)
        {
            Delta = delta;
        }

        /// <summary>
        /// The target change in dimensions upon completion.
        /// </summary>
        public Vector2 Delta { get; set; }

        /// <inheritdoc/>
        public override void ApplyDelta(float timeDelta)
            => Parent.Transform.Dimensions += Delta * timeDelta;

        /// <inheritdoc/>
        public override void CorrectDelta()
        {
            Vector2 dimensions = Parent.Transform.Dimensions;

            if ((dimensions.X - (int)dimensions.X) < SNAPPING_TOLERANCE)
                dimensions.X = (int)dimensions.X;
            else if ((dimensions.X - (int)dimensions.X) > (1f - SNAPPING_TOLERANCE))
                dimensions.X = (int)dimensions.X + 1f;

            if ((dimensions.Y - (int)dimensions.Y) < SNAPPING_TOLERANCE)
                dimensions.Y = (int)dimensions.Y;
            else if ((dimensions.Y - (int)dimensions.Y) > (1f - SNAPPING_TOLERANCE))
                dimensions.Y = (int)dimensions.Y + 1f;

            Parent.Transform.Dimensions = dimensions;
        }

        /// <summary>
        /// Constructs a <see cref="DimensionTransition"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static DimensionTransition FromXml(XElement element)
        {
            int.TryParse(element.Attribute("Time").Value, out int time);
            Vector2 delta = XmlUtils.Vec2FromXml(element.Element("Delta"));
            int.TryParse(element.Attribute("Options").Value, out int options);
            Enum.TryParse(element.Attribute("Easing").Value, out EasingType type);
            Enum.TryParse(element.Attribute("Direction").Value, out EasingDirection direction);

            return new DimensionTransition(time, delta, (TransitionOptions)options, type, direction);
        }
    }
}