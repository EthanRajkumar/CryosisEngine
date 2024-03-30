using Microsoft.Xna.Framework;

namespace CryosisEngine
{
    /// <summary>
    /// Represents all objects that require updating every frame.
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Determines whether an object is actively updating.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Calls when an object is first instantiated.
        /// </summary>
        public void Awake(GameServiceContainer services);

        /// <summary>
        /// Calls when an object is about to be destroyed / recycled.
        /// </summary>
        public void Sleep(GameServiceContainer services);

        /// <summary>
        /// Calls before the main pass of updating (usually to prime objects in response to input).
        /// </summary>
        /// <param name="gameTime"></param>
        public void EarlyUpdate(GameTime gameTime);

        /// <summary>
        /// Calls during the main pass of updating. Handles most logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime);

        /// <summary>
        /// Calls after the main pass of updating (usually to check for time-based events finishing).
        /// </summary>
        /// <param name="gameTime"></param>
        public void LateUpdate(GameTime gameTime);

        /// <summary>
        /// Calls when an object requires resources.
        /// </summary>
        /// <param name="services"></param>
        public void LoadContent(GameServiceContainer services);

        /// <summary>
        /// Calls when an object is releasing its resources.
        /// </summary>
        /// <param name="services"></param>
        public void UnloadContent(GameServiceContainer services);
    }
}
