using Microsoft.Xna.Framework.Audio;

using System;
using System.Collections.Generic;

namespace CryosisEngine
{
    public class SoundPool
    {
        public List<(DynamicSoundEffectInstance, bool)> Instances { get; }

        public SoundPool(int capacity)
        {
            Instances = new List<(DynamicSoundEffectInstance, bool)>();

            for (int i = 0; i < capacity; i++)
                Instances.Add((new DynamicSoundEffectInstance(44100, AudioChannels.Stereo), false));
        }

        public DynamicSoundEffectInstance GetSoundInstance(out int index)
        {
            for(int i = 0; i < Instances.Count; i++)
            {
                if (!Instances[i].Item2)
                {
                    index = i;
                    return Instances[i].Item1;
                }
            }

            throw new Exception("Sound limit reached! Capacity: " + Instances.Count + " sounds maximum.");
        }

        public void OnSoundReturn(object info, int index)
        {
            Instances[index].Item1.Stop();
            Instances[index] = (Instances[index].Item1, false);
        }
    }
}
