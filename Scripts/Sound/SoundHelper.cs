using UnityEngine;

namespace Magia.Audio
{
    public static class SoundHelper
    {
        private static readonly int[] pentatonicSemitones = { 0, 2, 4 };

        public static float GetRandomPitch()
        {
            int rand = Random.Range(0, pentatonicSemitones.Length);
            int semitone = pentatonicSemitones[rand];
            return GetPitch(semitone);
        }

        public static float GetRandomPitch(float lastPitch)
        {
            int rand = Random.Range(0, pentatonicSemitones.Length);
            int semitone = pentatonicSemitones[rand];
            float pitch = GetPitch(semitone);

            if (Mathf.Approximately(pitch, lastPitch))
            {
                rand = (rand + 1) % pentatonicSemitones.Length;
                semitone = pentatonicSemitones[rand];
                pitch = GetPitch(semitone);
            }

            return pitch;
        }

        public static float GetPitch(int semitone)
        {
            return Mathf.Pow(1.059463f, semitone);
        }
    }
}
