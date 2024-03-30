using Microsoft.Xna.Framework.Audio;

using System;
using System.IO;
using System.Threading.Tasks;

namespace CryosisEngine
{
    /// <summary>
    /// Holds a reference to a sound effect's metadata including base volume, pitch, and file path
    /// </summary>
    public class SoundFxReference
    {
        /// <summary>
        /// The sound pool to create <see cref="SoundFX"/> objects from
        /// </summary>
        public SoundPool SoundPool { get; }

        /// <summary>
        /// The file path of this sound
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The base volume of this sound for normalization purposes
        /// </summary>
        public float BaseVolume { get; }

        /// <summary>
        /// The base pitch of this sound for normalization purposes
        /// </summary>
        public float BasePitch { get; }

        public SoundFxReference(SoundPool soundPool, string filePath, float baseVolume, float basePitch)
        {
            SoundPool = soundPool;
            FilePath = filePath;
            BaseVolume = baseVolume;
            BasePitch = basePitch;
        }

        /// <summary>
        /// Creates a <see cref="SoundFX"/> object with the given meta data
        /// </summary>
        /// <returns></returns>
        public SoundFx CreateEffect()
        {
            return new SoundFx(SoundPool, FilePath, BaseVolume, BasePitch);
        }
    }

    /// <summary>
    /// Represents a dynamically-streamed sound effect
    /// </summary>
    public class SoundFx : IDisposable
    {
        static float _masterVolume;

        /// <summary>
        /// Controls the global volume of all sound effects
        /// </summary>
        public static float MasterVolume
        {
            get => _masterVolume;

            set
            {
                _masterVolume = value;
                VolumeChanged?.Invoke(null, null);
            }
        }

        /// <summary>
        /// Fires when any parameter of volume is changed
        /// </summary>
        public static EventHandler VolumeChanged { get; set; }


        float _volume;

        /// <summary>
        /// The audio stream of the sound effect
        /// </summary>
        public DynamicSoundEffectInstance SoundInstance { get; set; }

        /// <summary>
        /// The base volume of this sound effect for normalization purposes
        /// </summary>
        public float BaseVolume { get; }

        /// <summary>
        /// The variable volume of this sound effect. Should be modified to change this individual sound effect's volume.
        /// </summary>
        public float Volume
        {
            get => _volume;

            set
            {
                _volume = value;
                OnVolumeChange(null, null);
            }
        }

        /// <summary>
        /// The variable pitch of this sound effect. Should be modified to change this individual sound effect's pitch.
        /// </summary>
        public float Pitch
        {
            get => SoundInstance.Pitch;

            set => SoundInstance.Pitch = value;
        }

        protected long FilePosition { get; set; } = 44;

        /// <summary>
        /// The file path of the audio file being streamed
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The index occupied of a <see cref="SoundPool"/>
        /// </summary>
        public int PoolIndex { get; set; }

        /// <summary>
        /// Fires when the sound is disposed, releasing a sound object back to its pool
        /// </summary>
        public EventHandler<int> SoundReturned { get; set; }


        public Task LoadBufferTask { get; set; }

        /// <summary>
        /// Tells whether this sound effect is playing
        /// </summary>
        public bool IsSoundActive => SoundInstance != null && !SoundInstance.IsDisposed;

        public void Dispose()
        {
            SoundInstance.Dispose();
            SoundReturned?.Invoke(this, PoolIndex);
        }

        public SoundFx(SoundPool soundPool, string filePath, float baseVolume, float basePitch, float currentVolume = 1f)
        {
            FilePath = filePath;
            SoundInstance = soundPool.GetSoundInstance(out int poolIndex);
            PoolIndex = poolIndex;
            SoundInstance = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);

            SoundInstance.BufferNeeded += OnBufferNeeded;
            Pitch = basePitch;
            SoundReturned += soundPool.OnSoundReturn;
        }

        public void OnVolumeChange(object info, EventArgs e) => SoundInstance.Volume = MasterVolume * BaseVolume * Volume;

        public void Stop()
        {
            lock (SoundInstance)
            {
                if (IsSoundActive && SoundInstance.State == SoundState.Playing)
                    SoundInstance.Stop();
            }
        }

        public void Pause()
        {
            lock (SoundInstance)
            {
                if (IsSoundActive && SoundInstance.State == SoundState.Playing)
                    SoundInstance.Pause();
            }
        }

        public void Play()
        {
            lock (SoundInstance)
            {
                if (!IsSoundActive)
                    return;

                if (IsSoundActive && SoundInstance.State != SoundState.Playing)
                {
                    FilePosition = 44;
                    Stop();
                }

                OnBufferNeeded(null, null);
                SoundInstance.Play();
            }
        }

        public int exec = 0;

        public async void OnBufferNeeded(object info, EventArgs e)
        {
            await Task.Run(LoadBuffer);
        }

        public void LoadBuffer()
        {
            lock (SoundInstance)
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Position = FilePosition;
                    byte[] buffer = new byte[Math.Min(4096, Math.Max(0, fs.Length - FilePosition))];
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);
                    FilePosition += bytesRead;

                    // If there is no more data to read, stop the sound
                    if (bytesRead == 0)
                    {
                        Stop();
                        return;
                    }

                    // Submit the buffer to the sound instance
                    SoundInstance.SubmitBuffer(buffer);
                }
            }
        }
    }
}