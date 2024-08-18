using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    /// <summary>
    /// A <see cref="GameComponent"/> that scrolls and tiles a graphic across a desired area.
    /// </summary>
    public class ScrollingSprite : GameSprite
    {
        private Point FrameSize => CurrentFrame.Bounds.Size;

        /// <summary>
        /// Specifies how many pixels to scroll per second in each axis. Components can be negative to reverse scrolling, or 0 to halt it.
        /// </summary>
        public Vector2 ScrollSpeed { get; set; }

        /// <summary>
        /// How far within a <see cref="TextureFrame"/> the sprite has scrolled.
        /// </summary>
        private Vector2 ScrollOffset { get; set; }

        public ScrollingSprite(Color color, int frameDisplayID, SpriteEffects flipMode, float zIndex, Vector2 scrollSpeed) : base(frameDisplayID, color, flipMode, zIndex)
        {
            ScrollSpeed = scrollSpeed;
        }

        /// <inheritdoc/>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Atlas == null)
                return;

            Vector2 scroll = ScrollOffset + (ScrollOffset * (gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond));

            // Clamp our X and Y scroll coordinates betewen the top left and bottom right of our sprite frame
            scroll.X %= FrameSize.X;
            scroll.Y %= FrameSize.Y;

            if (scroll.X < 0)
                scroll.X += FrameSize.X;

            if (scroll.Y < 0)
                scroll.Y += FrameSize.Y;

            ScrollOffset = scroll;
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1)
        {
            if (Atlas == null)
                return;

            Rectangle frameBounds = CurrentFrame.Bounds, sourceRectangle = default;
            Vector2 drawOffset = default, drawSize = Parent.Transform.GlobalDimensions;

            Point sourceOrigin = default;
            float rotation = Parent.Transform.GlobalRotation, scale = Parent.Transform.GlobalScale;
            Vector2 position = Parent.Transform.TopLeft + CryosisMath.Rotate(drawOffset, rotation);

            alpha *= Alpha;

            sourceOrigin.Y = (int)(frameBounds.Y + ScrollOffset.Y);

            while (drawOffset.Y < drawSize.Y)
            {
                sourceOrigin.X = (int)(frameBounds.X + ScrollOffset.X);

                while (drawOffset.X < drawSize.X)
                {
                    sourceRectangle = Rectangle.Intersect(new Rectangle(sourceOrigin, frameBounds.Size), frameBounds);
                    sourceRectangle.Width = (int)Math.Min(drawSize.X - drawOffset.X, sourceRectangle.Width);
                    sourceRectangle.Height = (int)Math.Min(drawSize.Y - drawOffset.Y, sourceRectangle.Height);

                    spriteBatch.Draw(Atlas.BaseTexture, (position - offset) * viewportScale, sourceRectangle, Color * alpha, rotation, default, scale * viewportScale, CurrentFrame.FlipMode, 1f);
                    drawOffset.X += sourceRectangle.Width;

                    sourceOrigin.X = frameBounds.X;
                }

                drawOffset.Y += sourceRectangle.Height;
                drawOffset.X = 0;
                sourceOrigin.Y = frameBounds.Y;
            }
        }

        public static ScrollingSprite FromXml(XElement element)
        {
            int frameID = XmlUtils.IntFromAttribute(element, "FrameID");
            uint colorValue = XmlUtils.UIntFromAttribute(element, "ColorVal");
            int flipMode = XmlUtils.IntFromAttribute(element, "FlipMode");
            float zIndex = XmlUtils.FloatFromAttribute(element, "ZIndex");
            Vector2 scrollSpeed = XmlUtils.Vec2FromXml(element, "ScrollX", "ScrollY");

            string[] contentPaths = XmlUtils.AttributeValue(element, "ContentPaths").Split('|');

            return new ScrollingSprite(new Color(colorValue), frameID, (SpriteEffects)flipMode, zIndex, scrollSpeed) { ContentPaths = contentPaths };
        }
    }
}