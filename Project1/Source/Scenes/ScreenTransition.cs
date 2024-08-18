using System;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryosisEngine
{
    public class ScreenTransition
    {
        protected ThreadStart ThreadStart { get; set; }

        public EventHandler TransitionStarted { get; set; }

        public EventHandler HoldingStarted { get; set; }

        public EventHandler HoldingEnded { get; set; }

        public EventHandler TransitionEnded { get; set; }

        public Timer HoldTimer { get; set; }

        public bool IsWorkFinished { get; set; }

        public bool IsActive { get; set; }

        public bool IsHolding { get; set; }

        public string[] ContentPaths { get; set; }

        public ScreenTransition(int minHoldTime, string[] contentPaths)
        {
            HoldTimer = new Timer(minHoldTime);
            HoldTimer.Speed = 0f;
            ContentPaths = contentPaths;
        }

        public virtual void Activate(Action task)
        {
            IsActive = true;
            TransitionStarted?.Invoke(this, null);

            if (task != null)
            {
                ThreadStart = new ThreadStart(task);
                ThreadStart += EndTask;
            }
            else
            {
                IsWorkFinished = true;
                ThreadStart = null;
            }
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            IsWorkFinished = false;
            TransitionEnded?.Invoke(this, null);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (HoldTimer.IsExceeded && IsWorkFinished)
                EndHolding();

            HoldTimer?.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset, Rectangle camera, float viewportScale)
        {
        }

        public virtual void LoadContent(GameServiceContainer services)
        {
        }

        public virtual void UnloadContent(GameServiceContainer services)
        {
        }

        protected virtual void BeginHolding()
        {
            HoldTimer.Reset();
            HoldTimer.Speed = 1f;
            IsHolding = true;
            HoldingStarted?.Invoke(this, null);

            if (ThreadStart != null)
            {
                Thread thread = new Thread(ThreadStart) { IsBackground = true };
                thread.Start();
            }
        }

        protected virtual void EndTask()
        {
            IsWorkFinished = true;
        }

        protected virtual void EndHolding()
        {
            HoldTimer.Reset();
            HoldTimer.Speed = 0f;
            IsHolding = false;
            HoldingEnded?.Invoke(this, null);
        }
    }
}
