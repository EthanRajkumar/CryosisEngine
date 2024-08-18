using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace CryosisEngine
{
    /// <summary>
    /// Manages a collection of tiles, each of which can be animated from a <see cref="TextureAtlas"/>
    /// </summary>
    public class Tileset : GameComponent
    {
        /// <summary>
        /// The atlas to store tile graphics and/or animations
        /// </summary>
        public TextureAtlas Atlas { get; set; }

        /// <summary>
        /// A list of animators used to play each tile animation
        /// </summary>
        public List<FrameAnimator> Animators { get; set; }

        /// <summary>
        /// An array of values corresponding to tile graphics
        /// </summary>
        public int[,] TileValues { get; set; }

        /// <summary>
        /// The size of each tile (only rectangular, axis-aligned rectangles are supported)
        /// </summary>
        public Point TileSize { get; set; }

        public Tileset(int width, int height, List<FrameAnimation> animations)
        {
            TileValues = new int[width, height];

            for(int i = 0; i < animations.Count; i++)
                Animators.Add(new FrameAnimator() { Animation = animations[i] });
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1)
        {
            Vector2 basePosition = Parent.Transform.TopLeft;
            Vector2 tileSize = TileSize.ToVector2() * Parent.Transform.GlobalScale; 
        }
    }
}