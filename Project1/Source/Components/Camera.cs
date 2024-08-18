using Microsoft.Xna.Framework;

using System;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class Camera : GameComponent
    {
        GameObject _focus;

        /// <summary>
        /// Gets the <see cref="GameObject"/> that the camera is currently focusing on.
        /// </summary>
        public GameObject Focus
        {
            get => _focus;

            set
            {
                _focus = value;
                TweenDistance = _focus == null ? Vector2.Zero : (_focus.Transform.GlobalPosition - Parent.Transform.GlobalPosition + FocusOffset);
                TweenTimer.Reset();
                LastFocusPosition = Focus.Transform.GlobalPosition;
            }
        }

        /// <summary>
        /// Specifies the distance the camera will be relative to its focus.
        /// </summary>
        public Vector2 FocusOffset { get; set; }

        private Timer TweenTimer { get; set; } = new Timer(500);

        /// <summary>
        /// Tells how fast the camera will react to a change in focus (in milliseconds).
        /// </summary>
        public int TweenSpeed
        {
            get => TweenTimer.TargetTime;

            set => TweenTimer.TargetTime = value;
        }

        private Vector2 TweenDistance { get; set; }

        private Vector2 LastFocusPosition { get; set; }

        /// <summary>
        /// The bounding rectangle of the camera.
        /// </summary>
        public Rectangle Bounds => new Rectangle(Parent.Transform.TopLeft.ToPoint(), Parent.Transform.GlobalDimensions.ToPoint());

        public Easing Easing { get; set; }

        public Camera(Vector2 focusOffset, int tweenSpeed)
        {
            FocusOffset = focusOffset;
            TweenSpeed = tweenSpeed;
        }

        public override void Update(GameTime gameTime)
        {
            float tweenDelta = -Easing.ApplyEasingFunction(TweenTimer.Proportion);
            TweenTimer.Update(gameTime);
            tweenDelta += Easing.ApplyEasingFunction(TweenTimer.Proportion);

            if (Focus != null)
            {
                Vector2 positionDelta = Focus.Transform.GlobalPosition - LastFocusPosition;
                Parent.Transform.Position += positionDelta + (tweenDelta * TweenDistance);

                if (TweenTimer.IsExceeded)
                    Parent.Transform.Position = Focus.Transform.GlobalPosition + FocusOffset;

                LastFocusPosition = Focus.Transform.Position;
            }
        }

        public static Camera FromXml(XElement element)
        {
            Vector2 focusOffset = XmlUtils.Vec2FromXml(element, "FocusOffsetX", "FocusOffsetY");
            int tweenSpeed = XmlUtils.IntFromAttribute(element, "TweenSpeed");

            return new Camera(focusOffset, tweenSpeed);
        }
    }
}