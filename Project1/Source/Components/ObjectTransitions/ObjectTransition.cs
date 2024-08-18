using Microsoft.Xna.Framework;
using System;

namespace CryosisEngine
{
    public abstract class ObjectTransition : GameComponent
    {
        /// <summary>
        /// Determines the tolerance in proportion where corrected deltas can snap to interger values.
        /// </summary>
        public const float SNAPPING_TOLERANCE = 0.00625f;

        /// <summary>
        /// An underlying timer that handles time deltas.
        /// </summary>
        protected Timer Timer { get; set; }

        /// <summary>
        /// Controls the speed that the transition plays at.
        /// </summary>
        public float Speed
        {
            get => Timer.Speed;

            set => Timer.Speed = value;
        }

        /// <summary>
        /// Notifies when the transition completes, and any excess time that passed in between frames.
        /// </summary>
        public EventHandler<float> TransitionComplete => Timer.TimeExceeded;

        /// <summary>
        /// Determines whether this transition will retain its delta after completion.
        /// </summary>
        public bool IsContinuous
        {
            get => Options.HasFlag(TransitionOptions.Continuous);

            set
            {
                if (value)
                    Options |= TransitionOptions.Continuous;
                else
                    Options &= ~TransitionOptions.Continuous;
            }
        }

        /// <summary>
        /// Determines whether tis transition will repeat upon completion.
        /// </summary>
        public bool IsCyclic
        {
            get => Options.HasFlag(TransitionOptions.Cyclic);

            set
            {
                if (value)
                    Options |= TransitionOptions.Cyclic;
                else
                    Options &= ~TransitionOptions.Cyclic;
            }
        }

        /// <summary>
        /// Determines whether this transition will correct its delta is off by <see cref="SnappingTolerance"/>.
        /// </summary>
        public bool IsSnapping
        {
            get => Options.HasFlag(TransitionOptions.Snapping);

            set
            {
                if (value)
                    Options |= TransitionOptions.Snapping;
                else
                    Options &= ~TransitionOptions.Snapping;
            }
        }

        /// <summary>
        /// Controls the behavior of a transition upon completion.
        /// </summary>
        public enum TransitionOptions
        {
            Continuous = 1,
            Cyclic = 2,
            Snapping = 4
        }

        /// <summary>
        /// Contains the options for <see cref="IsContinuous"/>, <see cref="IsCyclic"/>, and <see cref="IsSnapping"/>.
        /// </summary>
        public TransitionOptions Options { get; set; }

        /// <summary>
        /// Determines the easing function used.
        /// </summary>
        public Easing Easing { get; set; }

        public ObjectTransition(int time, TransitionOptions options, EasingType easing, EasingDirection direction)
        {
            Timer = new Timer(time);

            Options = options;

            Easing = new Easing(easing, direction);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = -CalculateEasing();
            Timer?.Update(gameTime);

            if (Timer.IsExceeded)
            {
                if (IsCyclic)
                {
                    Timer.Reset();

                    if (!IsContinuous)
                        ApplyDelta(Timer.Speed >= 0 ? -1f : 1f);
                }

                if (IsSnapping)
                    CorrectDelta();
            }
            else
            {
                delta += CalculateEasing();
                ApplyDelta(delta);
            }
        }

        /// <summary>
        /// Calculates the easing proportion according to <see cref="EasingType"/> and <see cref="EasingDirection"/>.
        /// </summary>
        /// <returns></returns>
        public float CalculateEasing()
            => Easing.ApplyEasingFunction(Timer.Proportion);

        /// <summary>
        /// Makes changes to parent objects based upon time deltas.
        /// </summary>
        /// <param name="timeDelta"></param>
        public abstract void ApplyDelta(float timeDelta);

        /// <summary>
        /// Corrects deltas within tolerance to the nearest integer after completion.
        /// </summary>
        public abstract void CorrectDelta();
    }
}