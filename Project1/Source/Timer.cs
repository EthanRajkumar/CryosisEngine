using Microsoft.Xna.Framework;
using System;

namespace CryosisEngine
{
    public class Timer
    {
        int _targetTime;

        float _currentTime, _speed = 1f;

        public int TargetTime
        {
            get => _targetTime;

            set => _targetTime = Math.Max(1, value);
        }

        public float CurrentTime
        {
            get => _currentTime;

            set
            {
                float temp = _currentTime;

                _currentTime = value;

                if(temp < TargetTime && IsExceeded)
                    TimeExceeded?.Invoke(this, CurrentTime - TargetTime);
            }
        }

        public bool IsExceeded => CurrentTime >= TargetTime;

        public float Proportion => CurrentTime / TargetTime;

        public float Speed
        {
            get => _speed;

            set => _speed = value;
        }

        public EventHandler<float> TimeExceeded { get; set; }

        /// <summary>
        /// Constructs a new <see cref="Timer"/> object with a duration measured in milliseconds.
        /// </summary>
        /// <param name="targetTime"></param>
        /// <param name="currentTime"></param>
        public Timer(int targetTime)
            => TargetTime = targetTime;

        public void Update(GameTime gameTime)
            => CurrentTime += Math.Max(0, Speed) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        public void Reset()
            => CurrentTime = Speed >= 0 ? 0f : TargetTime;
    }
}