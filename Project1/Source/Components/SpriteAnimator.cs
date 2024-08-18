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

                if (IsContinuing)
                {
                    int currentFrame = Animator.CurrentFrameIndex;
                    Animator.Animation = CurrentAnimation;
                    Animator.CurrentFrameIndex = Animator.Animation.Frames.Count <= currentFrame ? 0 : currentFrame;
                }
                else
                    Animator.Animation = CurrentAnimation;

                IsContinuing = false;
            }
        }

        private int QueuedAnimationIndex { get; set; } = -1;

        public float Speed
        {
            get => Animator.AnimationSpeed;

            set => Animator.AnimationSpeed = Math.Max(0, value);
        }

        public EventHandler AnimationFinished { get; set; }

        /// <summary>
        /// The main animator that handles frame ticking.
        /// </summary>
        public FrameAnimator Animator
        {
            get => _animator;

            private set
            {
                _animator = value;
                _animator.FrameUpdated += OnFrameChange;
                _animator.AnimationFinished += AnimationFinished;
            }
        }

        public FrameAnimation CurrentAnimation => Animations[AnimationIndex];

        /// <summary>
        /// Controls whether the next animation swap will try to carry over the progress.
        /// </summary>
        private bool IsContinuing { get; set; }

        public SpriteAnimator(List<FrameAnimation> animations, int animationIndex)
        {
            Animations = animations;
            Animator = new FrameAnimator();
            AnimationIndex = animationIndex;
            AnimationFinished += OnAnimationFinish;
        }

        public override void Awake(GameServiceContainer services)
        {
            Sprite = Parent.GetComponent<GameSprite>();
            Sprite.SpriteAnimator = this;
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

        public void OnAnimationFinish(object info, EventArgs e)
        {
            if (QueuedAnimationIndex == -1)
                return;

            AnimationIndex = QueuedAnimationIndex;
            QueuedAnimationIndex = -1;
        }

        public void SetAnimation(string animationName, bool isContinuing = false)
        {
            IsContinuing = isContinuing;
            AnimationIndex = Animations.FindIndex(x => x.Name == animationName);
        }

        public static SpriteAnimator FromXml(XElement element)
        {
            int animationIndex = XmlUtils.IntFromAttribute(element, "AnimationID");
            float animationSpeed = XmlUtils.FloatFromAttribute(element, "AnimationSpeed");

            List<FrameAnimation> animations = new List<FrameAnimation>();

            foreach(XElement elem in element.Elements())
                animations.Add(FrameAnimation.FromXml(elem));

            return new SpriteAnimator(animations, animationIndex) { Speed = animationSpeed };
        }
    }

    public class AnimationFrame
    {
        public int FrameID { get; set; }

        public int Duration { get; set; }

        public Vector2 Offset { get; set; }

        public AnimationFrame(int frameID, int duration, Vector2 offset)
        {
            FrameID = frameID;
            Duration = duration;
            Offset = offset;
        }

        public static AnimationFrame FromXml(XElement element)
            => new AnimationFrame(XmlUtils.IntFromAttribute(element, "ID"), XmlUtils.IntFromAttribute(element, "Span"), XmlUtils.Vec2FromXml(element));
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

        public static FrameAnimation FromXml(XElement element)
        {
            string name = XmlUtils.AttributeValue(element, "Name");
            int loops = XmlUtils.IntFromAttribute(element, "Loops");

            List<AnimationFrame> frames = new List<AnimationFrame>();

            foreach (XElement elem in element.Elements())
                frames.Add(AnimationFrame.FromXml(elem));

            return new FrameAnimation(frames, name, loops);
        }
    }

    public class FrameAnimator
    {
        FrameAnimation _animation;

        Timer _frameTimer;

        int _currentFrameIndex;

        private static int MAXIMUMLOOPCOUNT { get; set; } = 2048;

        public int CurrentFrameIndex
        {
            get => _currentFrameIndex;

            set
            {
                _currentFrameIndex = value;

                if (_currentFrameIndex >= Animation.Frames.Count)
                {
                    if (Animation.Loops == -1)
                    {
                        _currentFrameIndex = 0;
                        AnimationFinished?.Invoke(this, null);
                    }
                    else if (CurrentLoop + 1 >= Animation.Loops)
                    {
                        AnimationFinished?.Invoke(this, null);
                        FrameTimer.Speed = 0f;
                    }
                    else
                    {
                        CurrentLoop++;
                    }
                }

                FrameTimer = new Timer(Animation.Frames[CurrentFrameIndex].Duration);
            }
        }

        public int CurrentLoop { get; set; }

        public AnimationFrame CurrentFrame => Animation.Frames[CurrentFrameIndex];

        public FrameAnimation Animation
        {
            get => _animation;

            set
            {
                _animation = value;

                if (_animation != null)
                    FrameTimer = new Timer(_animation.Frames[0].Duration);

                CurrentFrameIndex = 0;
                CurrentLoop = 0;
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
