using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    public class GameComponent : IDraw, IUpdate
    {
        /// <summary>
        /// This component's parent <see cref="GameObject"/>, which exposes its transform and alpha, as well as access to sibling components.
        /// </summary>
        public GameObject Parent { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public bool IsVisible { get; set; }

        /// <inheritdoc/>
        public float Alpha { get; set; } = 1f;

        /// <summary>
        /// Stores the asset names to be used when calling <see cref="LoadContent(GameServiceContainer)"/>
        /// </summary>
        public string[] ContentPaths { get; set; }

        /// <inheritdoc/>
        public virtual void Awake(GameServiceContainer services) { }

        /// <inheritdoc/>
        public virtual void Sleep(GameServiceContainer services) { }

        /// <summary>
        /// Removes this <see cref="GameComponent"/> from its parent <see cref="GameObject"/>.
        /// </summary>
        public void Remove()
            => Parent?.RemoveComponent(this);

        /// <inheritdoc/>
        /// <param name="gameTime"></param>
        public virtual void EarlyUpdate(GameTime gameTime) { }

        /// <inheritdoc/>
        public virtual void Update(GameTime gameTime) { }
        
        /// <inheritdoc/>
        public virtual void LateUpdate(GameTime gameTime) { }

        /// <inheritdoc/>
        public virtual void EarlyDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1f) { }

        /// <inheritdoc/>
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1f) { }

        /// <inheritdoc/>
        public virtual void LateDraw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale, float alpha = 1f) { }

        /// <inheritdoc/>
        public virtual void LoadContent(GameServiceContainer services) { }

        /// <inheritdoc/>
        public virtual void UnloadContent(GameServiceContainer services) { }
    }
}