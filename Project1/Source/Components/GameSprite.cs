using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml.Linq;

namespace CryosisEngine
{
    public class GameSprite : GameComponent
    {
        int _frameDisplayID;

        /// <summary>
        /// The main texture data for this <see cref="GameSprite"/>.
        /// </summary>
        public TextureAtlas Atlas { get; set; }

        /// <summary>
        /// Determines the tint of any texture data before drawing.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Determines the layering order when drawn.
        /// </summary>
        public float ZIndex { get; set; }

        /// <summary>
        /// Tells how texture data will be flipped before drawing.
        /// </summary>
        public SpriteEffects FlipMode { get; set; }

        /// <summary>
        /// Determines which frame from <see cref="Atlas"/> will be drawn.
        /// </summary>
        public int FrameDisplayID
        {
            get => _frameDisplayID;

            set
            {
                _frameDisplayID = value;

                // If the value we assign exceeds our atlas's number of frames, default to the 0th frame.
                if (Atlas != null && _frameDisplayID >= Atlas.Frames.Count)
                    _frameDisplayID = 0;
            }
        }

        /// <summary>
        /// Gets the current <see cref="TextureFrame"/> specified by <see cref="FrameDisplayID"/>
        /// </summary>
        public TextureFrame CurrentFrame => Atlas.Frames[FrameDisplayID];

        public SpriteAnimator SpriteAnimator { get; set; }

        public GameSprite(int frameDisplayID, Color color, SpriteEffects flipMode, float zIndex)
        {
            FrameDisplayID = frameDisplayID;
            Color = color;
            FlipMode = flipMode;
            ZIndex = zIndex;
        }

        ///inheritdocs
        public override void Awake(GameServiceContainer services)
        {
            base.Awake(services);
            SpriteAnimator = Parent.GetComponent<SpriteAnimator>();
        }

        /// <summary>
        /// Draws this <see cref="GameSprite"/>'s texture data to the screen based on its parent's <see cref="Transform2D"/>.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        /// <param name="camera"></param>
        /// <param name="viewportScale"></param>
        /// <param name="alpha"></param>
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1)
        {
            alpha *= Alpha;

            if (alpha == 0f || Atlas == null)
                return;

            Vector2 animationOffset = default;

            if (SpriteAnimator != null)
                animationOffset = SpriteAnimator.Animator.CurrentFrame.Offset * Parent.Transform.GlobalScale;

            // Scale our coordinates to match viewport scaling
            Point location = ((Parent.Transform.TopLeft - offset + animationOffset) * viewportScale).ToPoint();
            Point size = (Parent.Transform.GlobalDimensions * viewportScale).ToPoint();

            Rectangle destination = new Rectangle(location, size);

            // If our sprite will not appear onscreen, cancel its draw call to cull it.
            if (!destination.Intersects(camera) || alpha <= 0f)
                return;

            spriteBatch.Draw(Atlas.BaseTexture, destination, Atlas.Frames[FrameDisplayID].Bounds, Color, Parent.Transform.GlobalRotation, Vector2.Zero, FlipMode, ZIndex);
        }

        public override void LoadContent(GameServiceContainer services)
        {
            if (ContentPaths == null)
                return;

            Atlas = services.GetService<TextureAtlasLoader>().LoadContent(ContentPaths[0]);
        }

        public override void UnloadContent(GameServiceContainer services)
        {
            services.GetService<TextureAtlasLoader>().UnloadContent(Atlas);
        }

        public static GameSprite FromXml(XElement element)
        {
            int frameID = XmlUtils.IntFromAttribute(element, "FrameID");
            uint colorValue = XmlUtils.UIntFromAttribute(element, "ColorVal");
            int flipMode = XmlUtils.IntFromAttribute(element, "FlipMode");
            float zIndex = XmlUtils.FloatFromAttribute(element, "ZIndex");

            string[] contentPaths = XmlUtils.AttributeValue(element, "ContentPaths").Split('|');

            return new GameSprite(frameID, new Color(colorValue), (SpriteEffects)flipMode, zIndex) { ContentPaths = contentPaths };
        }
    }
}