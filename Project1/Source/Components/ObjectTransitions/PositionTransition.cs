using System;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    public class PositionTransition : ObjectTransition
    {
        public PositionTransition(int time, Vector2 delta, TransitionOptions options, EasingType easing, EasingDirection direction) : base(time, options, easing, direction)
        {
            Delta = delta;
        }

        /// <summary>
        /// The target change in position upon completion.
        /// </summary>
        public Vector2 Delta { get; set; }

        /// <inheritdoc/>
        public override void ApplyDelta(float timeDelta)
            => Parent.Transform.Position += Delta * timeDelta;

        /// <inheritdoc/>
        public override void CorrectDelta()
        {
            Vector2 position = Parent.Transform.Position, snappedPosition = Vector2.Round(position);

            if (Math.Abs(position.X - snappedPosition.X) < SNAPPING_TOLERANCE)
                position.X = snappedPosition.X;

            if (Math.Abs(position.Y - snappedPosition.Y) < SNAPPING_TOLERANCE)
                position.Y = snappedPosition.Y;

            Parent.Transform.Position = position;
        }

        /// <summary>
        /// Constructs a <see cref="PositionTransition"/> from an <see cref="XElement"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static PositionTransition FromXml(XElement element)
        {
            int.TryParse(element.Attribute("Time").Value, out int time);
            Vector2 delta = XmlUtils.Vec2FromXml(element.Element("Delta"));
            int.TryParse(element.Attribute("Options").Value, out int options);
            Enum.TryParse(element.Attribute("Easing").Value, out EasingType type);
            Enum.TryParse(element.Attribute("Direction").Value, out EasingDirection direction);

            return new PositionTransition(time, delta, (TransitionOptions)options, type, direction);
        }
    }
}