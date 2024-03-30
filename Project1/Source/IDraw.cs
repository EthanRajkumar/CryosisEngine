using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    /// <summary>
    /// Represents all drawable/graphical objects.
    /// </summary>
    public interface IDraw
    {
        /// <summary>
        /// Represents if an object is drawing to the screen.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Represents an object's opacity when drawing.
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// Calls before the main pass of drawing (usually to handle backgrounds or shader-based effects.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        /// <param name="camera"></param>
        /// <param name="alpha"></param>
        public void EarlyDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f);

        /// <summary>
        /// Calls during the main pass of drawing. Handles most drawing logic.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        /// <param name="camera"></param>
        /// <param name="alpha"></param>
		public void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f);

        /// <summary>
        /// Calls after the main pass of drawing (usually to handle foregrounds or shader-based overlays.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="offset"></param>
        /// <param name="camera"></param>
        /// <param name="alpha"></param>
		public void LateDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale = 1f, float alpha = 1f);
	}
}
