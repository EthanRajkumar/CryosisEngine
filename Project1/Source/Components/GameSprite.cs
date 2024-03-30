using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public GameSprite(int frameDisplayID, Color color, SpriteEffects flipMode, float zIndex)
        {
            FrameDisplayID = frameDisplayID;
            Color = color;
            FlipMode = flipMode;
            ZIndex = zIndex;
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
            Point position = (Parent.Transform.TopLeft - offset).ToPoint();
            Point size = Parent.Transform.TrueDimensions.ToPoint();

            Rectangle destination = new Rectangle(position, size);

            alpha *= Alpha;

            // If our sprite will not appear onscreen, cancel its draw call to cull it.
            if (!destination.Intersects(camera) || alpha <= 0f)
                return;

            // Scale our coordinates to match viewport scaling
            destination.Location = (Parent.Transform.TopLeft - offset * viewportScale).ToPoint();
            destination.Size = (Parent.Transform.TrueDimensions * viewportScale).ToPoint();

            spriteBatch.Draw(Atlas.BaseTexture, destination, Atlas.Frames[FrameDisplayID].Bounds, Color, Parent.Transform.GlobalRotation, Vector2.Zero, FlipMode, ZIndex);
        }

        public override void LoadContent(GameServiceContainer services)
        {
            if (ContentPaths == null)
                return;

            Atlas = services.GetService<TextureAtlasLoader>().LoadContent(ContentPaths[0]);
        }
    }
}