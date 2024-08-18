using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static CryosisEngine.SceneManager;

namespace CryosisEngine
{
    public class Scene
    {
        /// <summary>
        /// The service provider to allow data or content loading within the scene
        /// </summary>
        public GameServiceContainer Services { get; set; }

        /// <summary>
        /// The main collection of <see cref="GameObject"/>s within the scene
        /// </summary>
        public GameObjectCollection GameObjectCollection { get; set; } = new GameObjectCollection();

        /// <summary>
        /// Fires when a change in <see cref="Scene"/> occurs from within this <see cref="Scene"/>'s control
        /// </summary>
        public EventHandler<SceneChangeEventArgs> SceneChangeRequested { get; set; }

        /// <summary>
        /// The main camera used for this scene. Will default to the world origin.
        /// </summary>
        public Camera MainCamera => GameObjectCollection.GetGameObject("MainCamera").GetComponent<Camera>();

        public Scene(GameServiceContainer services)
            => Services = services;

        public virtual void Awake(GameServiceContainer services)
        {
            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].Awake(services);
        }

        public virtual void Sleep(GameServiceContainer services)
        {
            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].Sleep(services);
        }

        public virtual void LoadContent(GameServiceContainer services)
        {
            for (int i = 0; i < GameObjectCollection.GameObjects.Count; i++)
                GameObjectCollection.GameObjects[i].LoadContent(services);
        }

        public virtual void UnloadContent(GameServiceContainer services)
        {
            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].UnloadContent(services);
        }

        public virtual void Update(GameTime gameTime)
        {
            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].EarlyUpdate(gameTime);

            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].Update(gameTime);

            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].LateUpdate(gameTime);
        }

        public virtual void Draw(float viewportScale = 1)
        {
            SpriteBatch spriteBatch = Services.GetService<SpriteBatch>();

            if (spriteBatch == null)
                return;

            Vector2 offset = MainCamera.Parent.Transform.GlobalPosition;
            Rectangle cameraBounds = MainCamera.Bounds;

            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].EarlyDraw(spriteBatch, offset, cameraBounds, viewportScale);

            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].Draw(spriteBatch, offset, cameraBounds, viewportScale);

            for (int i = 0; i < GameObjectCollection.RootObjects.Count; i++)
                GameObjectCollection.RootObjects[i].LateDraw(spriteBatch, offset, cameraBounds, viewportScale);
        }
    }
}