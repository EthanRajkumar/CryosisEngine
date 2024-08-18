using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using System.Threading;

namespace CryosisEngine
{
    /// <summary>
    /// Loads, unloads, and transitions between active scenes.
    /// </summary>
    public class SceneManager
    {
        ScreenTransition _transition;

        /// <summary>
        /// The <see cref="GameServiceContainer"/> used to create, load, and unload a <see cref="Scene"/>.
        /// </summary>
        public GameServiceContainer Services { get; set; }

        /// <summary>
        /// The current <see cref="Sc.ene"/> loaded in the game.
        /// </summary>
        public Scene CurrentScene { get; set; }

        /// <summary>
        /// The name of the current <see cref="Scene"/> loaded in the game.
        /// </summary>
        public string CurrentSceneKey { get; set; }

        /// <summary>
        /// The name of the <see cref="Scene"/> to be loaded next.
        /// </summary>
        public string QueuedSceneKey { get; set; }

        /// <summary>
        /// A delegate function to create a <see cref="Scene"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate Scene SceneCreator(GameServiceContainer services, string[] args);

        /// <summary>
        /// A collection of <see cref="Scene"/> objects and their string names.
        /// </summary>
        public static Dictionary<string, SceneCreator> SceneCreators { get; set; } = new Dictionary<string, SceneCreator>();

        /// <summary>
        /// The <see cref="ScreenTransition"/> used to swap between <see cref="Scene"/> objects.
        /// </summary>
        public ScreenTransition Transition
        {
            get => _transition;

            set
            {
                if (_transition != null)
                {
                    _transition.TransitionEnded -= OnTransitionEnd;
                    _transition.UnloadContent(Services);
                }
                if (value != null)
                {
                    _transition = value;
                    _transition.TransitionEnded += OnTransitionEnd;
                    _transition.LoadContent(Services);
                }
            }
        }

        /// <summary>
        /// Tells if a <see cref="Scene"/> is currently being hidden by the <see cref="ScreenTransition"/>.
        /// </summary>
        public bool IsCurrentSceneVisible => Transition != null ? !Transition.IsHolding : !IsChangingScene;

        /// <summary>
        /// Tells if this <see cref="SceneManager"/> is currently swapping <see cref="Scenes"/>s.
        /// </summary>
        public bool IsChangingScene { get; set; }

        /// <summary>
        /// The string arguments for the next <see cref="Scene"/> to be loaded with.
        /// </summary>
        public string[] QueuedSceneArgs { get; set; }

        public SceneManager(GameServiceContainer services, ScreenTransition transition, string sceneKey, string[] sceneArgs)
        {
            Services = services;
            Transition = transition;
            CurrentSceneKey = sceneKey;
        }

        /// <summary>
        /// Initiates the swap from one active <see cref="Scene"/> to another.
        /// </summary>
        /// <param name="sceneKey"></param>
        /// <param name="args"></param>
        public void ChangeScene(string sceneKey, string[] args)
        {
            IsChangingScene = true;
            QueuedSceneKey = sceneKey;
            QueuedSceneArgs = args;

            Transition?.Activate(InstantiateScene);
        }

        /// <summary>
        /// Fires when a transition ends, denoting a complete swap between <see cref="Scene"/>s.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="e"></param>
        public void OnTransitionEnd(object i, EventArgs e)
        {
            IsChangingScene = false;
            CurrentSceneKey = QueuedSceneKey;
            QueuedSceneKey = null;
            QueuedSceneArgs = null;
        }

        /// <summary>
        /// Performes the swap between two <see cref="Scene"/> objects.
        /// </summary>
        public void InstantiateScene()
        {
            CurrentScene?.UnloadContent(Services);
            CurrentScene = null;

            CurrentScene = SceneCreators[QueuedSceneKey].Invoke(Services, QueuedSceneArgs);
            CurrentScene.SceneChangeRequested += OnChangeRequest;
            CurrentScene.LoadContent(Services);
            CurrentScene.Awake(Services);
        }

        public void Update()
        {
            Transition?.Update(Services.GetService<GameTime>());

            if ((Transition == null && !IsChangingScene) || (Transition != null && !Transition.IsHolding))
                CurrentScene?.Update(Services.GetService<GameTime>());
        }

        public void Draw(float viewportScale)
        {
            if (IsCurrentSceneVisible)
                CurrentScene?.Draw(viewportScale);

            Transition?.Draw(Services.GetService<SpriteBatch>(), Vector2.Zero, new Rectangle(0, 0, 320, 180), viewportScale);
        }

        public class SceneChangeEventArgs : EventArgs
        {
            public string SceneKey { get; }

            public string[] Args { get; }

            public SceneChangeEventArgs(string sceneKey, string[] args) : base()
            {
                SceneKey = sceneKey;
                Args = args;
            }
        }

        public void OnChangeRequest(object info, SceneChangeEventArgs e)
        {
            ChangeScene(e.SceneKey, e.Args);
        }
    }
}
