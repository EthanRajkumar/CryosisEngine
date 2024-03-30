using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CryosisEngine
{
    /// <summary>
    /// A <see cref="GameComponent"/> that animates a <see cref="GameSprite"/> by regularly changing its <see cref="GameSprite.FrameDisplayID"/>.
    /// </summary>
    public class SpriteAnimator : GameComponent
    {
        int _animationIndex;

        GameSprite _sprite;

        FrameAnimator _animator;

        /// <summary>
        /// A list of all sprite animations associated with this sprite.
        /// </summary>
        public List<FrameAnimation> Animations { get; set; }

        private GameSprite Sprite
        {
            get => _sprite;

            set
            {
                if (value == null)
                    return;

                _sprite = value;
                _sprite.FrameDisplayID = Animator.CurrentFrame.FrameID;
            }
        }

        private int AnimationIndex
        {
            get => _animationIndex;

            set
            {
                _animationIndex = value;
                Animator.Animation = CurrentAnimation;
            }
        }

        /// <summary>
        /// The main animator that handles
        /// </summary>
        public FrameAnimator Animator
        {
            get => _animator;

            private set
            {
                _animator = value;
                _animator.FrameUpdated += OnFrameChange;
            }
        }

        public FrameAnimation CurrentAnimation => Animations[AnimationIndex];

        public SpriteAnimator(List<FrameAnimation> animations, int animationIndex)
        {
            Animations = animations;
            Animator = new FrameAnimator();
            AnimationIndex = animationIndex;
        }

        public override void Awake(GameServiceContainer services)
        {
            Sprite = Parent.GetComponent<GameSprite>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Animator.Update(gameTime);
        }

        public void OnFrameChange(object info, int e)
        {
            if (Sprite != null)
                Sprite.FrameDisplayID = e;
        }
    }

    public class AnimationFrame
    {
        public int FrameID { get; set; }

        public int Duration { get; set; }

        public AnimationFrame(int frameID, int duration)
        {
            FrameID = frameID;
            Duration = duration;
        }

        public static AnimationFrame FromXml(XElement element)
            => new AnimationFrame(XmlUtils.IntFromAttribute(element, "ID"), XmlUtils.IntFromAttribute(element, "Span"));
    }

    public class FrameAnimation
    {
        public string Name { get; set; }

        public int Loops { get; set; }

        public List<AnimationFrame> Frames { get; set; }

        public FrameAnimation(List<AnimationFrame> frames, string name, int loops)
        {
            Frames = frames;
            Name = name;
            Loops = loops;
        }
    }

    public class FrameAnimator
    {
        FrameAnimation _animation;

        Timer _frameTimer;

        int _currentFrameIndex;

        int _currentLoop;

        private static int MAXIMUMLOOPCOUNT { get; set; } = 2048;

        public int CurrentFrameIndex
        {
            get => _currentFrameIndex;

            set
            {
                if (value == Animation.Frames.Count)
                    CurrentLoop++;
                else
                {
                    _currentFrameIndex = value;
                    FrameTimer = new Timer(Animation.Frames[CurrentFrameIndex].Duration);
                }
            }
        }

        public int CurrentLoop
        {
            get => _currentLoop;

            set
            {
                if (Animation.Loops != -1 && CurrentLoop == Animation.Loops - 1)
                    AnimationFinished?.Invoke(this, null);
                else
                {
                    _currentLoop++;
                    CurrentFrameIndex = 0;
                }
            }
        }

        public AnimationFrame CurrentFrame => Animation.Frames[CurrentFrameIndex];

        public FrameAnimation Animation
        {
            get => _animation;

            set
            {
                _animation = value;

                if (_animation != null)
                    FrameTimer = new Timer(_animation.Frames[0].Duration);
            }
        }

        private Timer FrameTimer
        {
            get => _frameTimer;

            set
            {
                _frameTimer = value;
                _frameTimer.TimeExceeded += OnFramePass;
            }
        }

        public float AnimationSpeed
        {
            get => FrameTimer.Speed;

            set => FrameTimer.Speed = value;
        }

        public EventHandler AnimationFinished { get; set; }

        public EventHandler<int> FrameUpdated { get; set; }

        public void Update(GameTime gameTime)
        {
            FrameTimer?.Update(gameTime);
        }

        public void OnFramePass(object info, float e)
        {
            CurrentFrameIndex++;
            FrameUpdated?.Invoke(this, CurrentFrame.FrameID);

            FrameTimer.CurrentTime = e;
        }
    }
}
